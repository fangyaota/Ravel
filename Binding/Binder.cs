using Ravel.Syntax;
using Ravel.Text;
using Ravel.Values;

namespace Ravel.Binding
{
    public sealed class Binder
    {
        private readonly DiagnosticList _diagnostics;
        private BoundScope _scope;
        public IEnumerable<Diagnostic> Diagnostics
        {
            get
            {
                return _diagnostics;
            }
        }

        public SourceText Text { get; }
        public SyntaxTree Tree { get; }
        public RavelGlobal Global { get; }

        public Binder(SourceText text, SyntaxTree tree, RavelGlobal global, RavelScope? scope = null)
        {
            Text = text;
            _diagnostics = new(Text);
            Tree = tree;
            Global = global;
            _scope = scope?.ToBound() ?? new BoundScope(global.Variables);
        }
        public BoundProgram Bind()
        {
            if (Tree.Diagnostics.Any())
            {
                return new( new BoundLiteralExpression(Global.TypePool.Unit));
            }
            return new( BindExpression(Tree.Root.Expression));
        }
        private BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            return syntax switch
            {
                LiteralExpressionSyntax literal => BindLiteralExpression(literal),
                UnaryExpressionSyntax unary => BindUnaryExpression(unary),
                BinaryExpressionSyntax binary => BindBinaryExpression(binary),
                ParenthesizedExpressionSyntax parenthesized => BindParenthesizedExpression(parenthesized),
                NameExpressionSyntax name => BindNameExpression(name),
                AssignmentExpressionSyntax assignment => BindAssignmentExpression(assignment),
                DefiningExpressionSyntax defining => BindDefiningExpression(defining),
                FunctionCallExpressionSyntax function => BindFunctionCallExpression(function),
                DotExpresionSyntax dot => BindDotExpression(dot),
                StatementSyntax statement => BindStatementExpression(statement),
                BlockSyntax block => BindBlockExpression(block),
                FunctionDefiningExpressionSyntax functionDefining => BindFunctionDefiningExpression(functionDefining),
                IfExpressionSyntax @if => BindIfExpression(@if),
                LambdaDefiningExpressionSyntax lambda => BindLambdaDefining(lambda),
                WhileExpressionSyntax @while => BindWhileExpression(@while),
                DeclareExpressionSyntax declare => BindDeclareExpression(declare),
                _ => BindUnexpected(syntax),
            };
        }

        private BoundExpression BindDeclareExpression(DeclareExpressionSyntax declare)
        {
            _diagnostics.ReportNotSupportedYet(declare.Span);
            if (!BindDefining(declare, out var ravelType))
            {
                return new BoundErrorExpression();
            }
            if (!_scope.TryDeclare(declare.Identifier.Text, ravelType!, declare.IsReadonly, declare.IsConst))
            {
                _diagnostics.ReportVariableAlreadyExists(declare);
            }
            return new BoundDeclareExpression(declare.Identifier.Text, ravelType!);
        }

        private BoundExpression BindWhileExpression(WhileExpressionSyntax @while)
        {
            BoundExpression condition = BindExpression(@while.Condition);
            if (condition is not BoundErrorExpression)
            {
                if (!condition.Type.IsSonOrEqual(Global.TypePool.BoolType))
                {
                    _diagnostics.ReportTypeNotMatching(@while.Condition.Span, condition.Type, Global.TypePool.BoolType);
                }
            }
            BoundScope s = _scope;
            _scope = new(_scope);
            BoundExpression expr = BindExpression(@while.Expression);
            _scope = s;

            if(expr is BoundErrorExpression)
            {
                return expr;
            }

            return new BoundWhileExpression(condition, expr);
        }

        private BoundExpression BindUnexpected(ExpressionSyntax syntax)
        {
            _diagnostics.ReportNotSupportedToken(syntax);
            return new BoundErrorExpression();
        }

        private BoundExpression BindLambdaDefining(LambdaDefiningExpressionSyntax lambda)
        {
            BoundExpression function = BindExpression(lambda.Function);

            BoundScope s = _scope;
            _scope = new(_scope);

            Dictionary<int, RavelDefining> parameters = new();

            if(function is BoundErrorExpression)
            {
                return function;
            }

            if (!function.Type.IsFunction)
            {
                _diagnostics.ReportNotAFunction(lambda.Span, function);
                return new BoundErrorExpression();
            }

            if (lambda.Parameters.Count > function.Type.GetMaxParameters())
            {
                _diagnostics.ReportParametersTooMany(lambda.Function, lambda.Parameters.Count, function.Type.GetMaxParameters());
                return function;
            }

            for (int i = 0; i < lambda.Parameters.Count; i++)
            {
                if (lambda.Parameters[i] is NameExpressionSyntax name && name.Identifier.Text == "_")
                {
                    parameters.Add(i, new(function.Type.GetFuncParametersIndex(i), $"_{i}", false, false));
                }
            }

            foreach (RavelDefining i in parameters.Values)
            {
                _scope[i.Name] = i;
            }

            RavelType ret = function.Type.GetTypeWhenCall(lambda.Parameters.Count);
            RavelType[] types = parameters.Select(x => x.Value.Type).ToArray();
            RavelType ty = Global.TypePool.GetFuncType(ret, types);

            _scope = s;

            List<BoundExpression> variables = new();

            for (int i = 0; i < lambda.Parameters.Count; i++)
            {
                if (parameters.TryGetValue(i, out RavelDefining? defining))
                {
                    variables.Add(new BoundVariableExpression(defining));
                }
                else
                {
                    BoundExpression item = BindExpression(lambda.Parameters[i]);
                    if (item is BoundErrorExpression)
                    {
                        return item;
                    }
                    variables.Add(item);
                }
            }

            BoundFunctionCallExpression sentence = new(function, variables);
            return new BoundFunctionDefiningExpression(parameters.Values.ToList(), sentence, ty);
        }
        private BoundExpression BindIfExpression(IfExpressionSyntax @if)
        {
            BoundExpression condition = BindExpression(@if.Condition);
            if (condition is not BoundErrorExpression)
            {
                if (!condition.Type.IsSonOrEqual(Global.TypePool.BoolType))
                {
                    _diagnostics.ReportTypeNotMatching(@if.Condition.Span, condition.Type, Global.TypePool.BoolType);
                }
            }

            BoundScope s = _scope;
            _scope = new(_scope);
            BoundExpression expTrue = BindExpression(@if.ExpTrue);
            BoundExpression expFalse = BindExpression(@if.ExpFalse);
            _scope = s;

            if(expTrue is BoundErrorExpression)
            {
                return expTrue;
            }
            if (expFalse is BoundErrorExpression)
            {
                return expFalse;
            }
            if (!expTrue.Type.IsSonOrEqual(expFalse.Type))
            {
                _diagnostics.ReportTypeNotMatching(@if.ExpFalse.Span, expTrue.Type, expFalse.Type);
            }
            return new BoundIfExpression(condition, expTrue, expFalse);
        }

        private bool BindDefining(DeclareExpressionSyntax defining, out RavelType? ravelType, RavelType? matchType = null)
        {
            ravelType = null;
            if (defining.Type == null)
            {
                return true;
            }
            if (defining.Tokens.Any(x => x.Kind is SyntaxKind.Dynamic))
            {
                ravelType = matchType;
                return true;
            }
            BoundExpression type = BindExpression(defining.Type);
            if(type is BoundErrorExpression or BoundDeclareExpression)
            {
                return false;
            }
            if (!type.IsConst)
            {
                _diagnostics.ReportTypeNotConst(defining);
                return false;
            }
            if (!type.Type.IsSonOrEqual(Global.TypePool.TypeType))
            {
                _diagnostics.ReportNotAType(defining.Type.Span, type.Type);
                return false;
            }

            

            RavelObject result = new Evaluator(type, Global).Evaluate();//?

            ravelType = result.GetValue<RavelType>();
            if (matchType == null)
            {
                return true;
            }
            
            if (!ravelType.IsSonOrEqual(matchType))
            {
                _diagnostics.ReportTypeNotMatching(defining.Span, matchType, ravelType);
                return false;
            }
            return true;
        }
        private BoundExpression BindFunctionDefiningExpression(FunctionDefiningExpressionSyntax defining)
        {
            BoundScope s = _scope;
            _scope = new(_scope);

            List<RavelDefining> parameters = new();
            foreach (DeclareExpressionSyntax i in defining.ParamList)
            {
                if (BindDefining(i, out RavelType? t))
                {
                    parameters.Add(new(t!, i.Identifier.Text, false, false));
                }
            }
            foreach (RavelDefining i in parameters)
            {
                _scope[i.Name] = i;
            }
            if (defining.TypeIdentifier != null)
            {
                if (!Global.Variables.TryGetVariable(defining.TypeIdentifier.Text, out RavelVariable? variable))
                {
                    _diagnostics.ReportUndefinedName(defining.TypeIdentifier);
                    goto r;
                }
                if (variable.Type != Global.TypePool.TypeType)
                {
                    _diagnostics.ReportNotAType(defining.TypeIdentifier.Span, variable.Type);
                    goto r;
                }
                RavelType o = variable.Object.GetValue<RavelType>();
                RavelType type = Global.TypePool.GetFuncType(o, parameters.Select(x => x.Type).ToArray());
                _scope.TryDeclare("self", type, true, false);
                BoundExpression sent = BindExpression(defining.Sentence);
                _scope = s;
                return new BoundFunctionDefiningExpression(parameters, sent, type);

            }

        r:
            BoundExpression sentence = BindExpression(defining.Sentence);
            _scope = s;
            if (sentence is BoundErrorExpression)
            {
                return sentence;
            }
            RavelType ty = Global.TypePool.GetFuncType(sentence.Type, parameters.Select(x => x.Type).ToArray());
            return new BoundFunctionDefiningExpression(parameters, sentence, ty);
        }

        private BoundExpression BindDotExpression(DotExpresionSyntax dot)
        {
            BoundExpression left = BindExpression(dot.Left);
            RavelType type = left.Type;
            if (type.TryGetSonVariable(dot.Right.Text, out RavelVariable? variable))
            {
                type = variable!.Type;
                return new BoundDotExpression(left, dot.Right.Text, type);
            }
            _diagnostics.ReportUndefinedName(dot.Right);
            return left;
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax assignment)
        {
            BoundExpression expression = BindExpression(assignment.Expression);
            if (!_scope.TryGetVariable(assignment.Variable.Text, out lDeclare? variable))
            {
                _diagnostics.ReportUndefinedName(assignment.Variable);
                return expression;
            }
            if (!expression.Type.IsSonOrEqual(variable.Type))
            {
                _diagnostics.ReportTypeNotMatching(assignment.Variable.Span, expression.Type, variable.Type);
                return expression;
            }
            if (variable.IsReadOnly)
            {
                _diagnostics.ReportReadOnlyVariableChange(variable, assignment.Expression.Span);
                return expression;
            }
            return new BoundAssignmentExpression(assignment.Variable.Text, expression);
        }

        private BoundExpression BindBlockExpression(BlockSyntax syntax)
        {
            List<BoundExpression> l = new List<BoundExpression>();
            BoundScope p = _scope;
            _scope = new(_scope);
            foreach (ExpressionSyntax i in syntax.Statements)
            {
                l.Add(BindExpression(i));
            }
            _scope = p;
            if(l.Count == 0)
            {
                return new BoundLiteralExpression(Global.TypePool.Unit);
            }
            return new BoundBlockExpression(l);
        }

        private BoundExpression BindStatementExpression(StatementSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        private BoundExpression BindFunctionCallExpression(FunctionCallExpressionSyntax syntax)
        {
            BoundExpression func = BindExpression(syntax.Function);
            List<BoundExpression> paramList = syntax.Parameters.Select(p => BindExpression(p)).ToList();

            if (!func.Type.IsFunction)
            {
                _diagnostics.ReportNotAFunction(syntax.Function.Span, func);
                return func;
            }

            int max = func.Type.GetMaxParameters();
            if (paramList.Count > max)
            {
                _diagnostics.ReportParametersTooMany(syntax.Function, paramList.Count, max);
                return func;
            }

            for (int i = 0; i < paramList.Count; i++)
            {
                RavelType paramType = paramList[i].Type;
                RavelType needType = func.Type.GetFuncParametersIndex(i);
                if (!paramType.IsSonOrEqual(needType))
                {
                    _diagnostics.ReportTypeNotMatching(syntax.Parameters[i].Span, paramType, needType);
                    return func;
                }
            }

            return new BoundFunctionCallExpression(func, paramList);
        }

        private BoundExpression BindDefiningExpression(DefiningExpressionSyntax syntax)
        {
            string name = syntax.Declare.Identifier.Text;
            BoundExpression expression = BindExpression(syntax.Expression);
            if(expression is BoundErrorExpression)
            {
                return expression;
            }
            if (syntax.Declare.Type == null)
            {
                RavelDefining define = new RavelDefining(expression.Type, name, false, false);
                if (!_scope.TryDeclare(name, expression.Type, false, false))
                {
                    _diagnostics.ReportVariableAlreadyExists(syntax.Declare);
                }
                return new BoundDefiningExpression(define, expression);
            }

            if (!BindDefining(syntax.Declare, out RavelType? real, expression.Type))
            {
                return expression;
            }
            RavelDefining defined = new RavelDefining(real!, name, false, false);
            if (!_scope.TryDeclare(name, expression.Type, false, false))
            {
                _diagnostics.ReportVariableAlreadyExists(syntax.Declare);
            }
            return new BoundDefiningExpression(defined, expression);
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            string name = syntax.Identifier.Text;
            if (_scope.TryGetVariable(name, out lDeclare? ob))
            {
                return new BoundVariableExpression(ob);
            }
            if (Global.Variables.TryGetVariable(name, out RavelVariable? obj))
            {
                return new BoundVariableExpression(obj);
            }
            _diagnostics.ReportUndefinedName(syntax.Identifier);
            return new BoundErrorExpression();
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            BoundExpression left = BindExpression(syntax.Left);
            BoundExpression right = BindExpression(syntax.Right);
            if(left is BoundErrorExpression || right is BoundErrorExpression)
            {
                return new BoundErrorExpression();
            }
            RavelBinaryOperator? oper = left.Type.GetOperator(syntax.OperatorToken.Kind, right.Type);
            if (oper == null)
            {
                _diagnostics.ReportOperatorNotDefined(left, syntax, right);
                return left;
            }
            return new BoundBinaryExpression(left, oper, right);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            BoundExpression operand = BindExpression(syntax.Operand);
            if(operand is BoundErrorExpression)
            {
                return operand;
            }
            RavelUnaryOperator? oper = operand.Type.GetOperator(syntax.OperatorToken.Kind);
            if (oper == null)
            {
                _diagnostics.ReportOperatorNotDefined(syntax, operand);
                return operand;
            }
            return new BoundUnaryExpression(oper, operand);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            return new BoundLiteralExpression(syntax.LiteralToken.Value);
        }
    }
}
