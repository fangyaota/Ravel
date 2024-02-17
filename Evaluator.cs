using Ravel.Binding;
using Ravel.Values;

namespace Ravel
{
    public class Evaluator
    {
        public BoundExpression Root { get; }
        public RavelGlobal Global { get; }
        public RavelScope Scope { get; private set; }

        public Evaluator(BoundProgram root, RavelGlobal global, RavelScope? scope = null)
        {
            Root = root.Expression;
            Global = global;
            Scope = scope ?? new(Global.Variables);
        }
        internal Evaluator(BoundExpression root, RavelGlobal global, RavelScope? scope = null)
        {
            Root = root;
            Global = global;
            Scope = scope ?? new(Global.Variables);
        }
        public RavelObject Evaluate()
        {
            return EvaluateExpression(Root);
        }
        internal RavelObject EvaluateExpression(BoundExpression root)
        {
            return root switch
            {
                BoundLiteralExpression literal => EvaluateLiteral(literal),
                BoundAssignmentExpression assignment => EvaluateAssignment(assignment),
                BoundDefiningExpression Defining => EvaluateDefining(Defining),
                BoundVariableExpression variable => EvaluateVariable(variable),
                BoundUnaryExpression unary => EvaluateUnary(unary),
                BoundBinaryExpression binary => EvaluateBinary(binary),
                BoundFunctionCallExpression call => EvaluateFunctionCall(call),
                BoundBlockExpression block => EvaluateBlock(block),
                BoundDotExpression dot => EvaluateDot(dot),
                BoundFunctionDefiningExpression functionDefining => EvaluateFunctionDefining(functionDefining),
                BoundIfExpression @if => EvaluateIf(@if),
                BoundWhileExpression @while => EvaluateWhile(@while),
                _ => throw new InvalidOperationException($"Unexpected node '{root.Kind}'"),
            };
        }

        private RavelObject EvaluateWhile(BoundWhileExpression @while)
        {
            RavelObject last = Global.TypePool.Unit;
            while (EvaluateExpression(@while.Condition).GetValue<bool>())
            {
                last = EvaluateExpression(@while.Expression);
            }
            return last;
        }

        private RavelObject EvaluateIf(BoundIfExpression @if)
        {
            RavelObject condition = EvaluateExpression(@if.Condition);
            return condition.GetValue<bool>() ? EvaluateExpression(@if.ExpTrue) : EvaluateExpression(@if.ExpFalse);
        }

        private RavelObject EvaluateFunctionDefining(BoundFunctionDefiningExpression functionDefining)
        {
            RavelLambda function = new RavelLambda(functionDefining, Scope, Global);
            return RavelObject.GetFunction(function, Global.TypePool);
        }

        private RavelObject EvaluateDot(BoundDotExpression dot)
        {
            RavelObject owner = EvaluateExpression(dot.Owner);
            if (!owner.TryGetSonValue(dot.Son, out RavelObject obj))
            {
                throw new InvalidOperationException(dot.Son);
            }
            return obj;
        }

        private RavelObject EvaluateAssignment(BoundAssignmentExpression assignment)
        {
            RavelObject exp = EvaluateExpression(assignment.Expression);
            if (!Scope.TryGetVariable(assignment.Name, out RavelVariable? value))
            {
                throw new InvalidOperationException();
            }
            if (value.IsReadOnly)
            {
                throw new InvalidOperationException();
            }
            value.Object = exp;
            return exp;
        }

        private RavelObject EvaluateBlock(BoundBlockExpression block)
        {
            RavelScope s = Scope;
            Scope = new(Scope);
            RavelObject result = Global.TypePool.Unit;
            foreach (BoundExpression e in block.Expressions)
            {
                result = EvaluateExpression(e);
            }
            Scope = s;
            return result;
        }

        private RavelObject EvaluateFunctionCall(BoundFunctionCallExpression call)
        {
            RavelObject func = EvaluateExpression(call.Function);
            RavelObject[] parameters = call.Parameters.Select(p => EvaluateExpression(p)).ToArray();
            return func.Call(parameters);
        }

        private RavelObject EvaluateBinary(BoundBinaryExpression binary)
        {
            RavelObject left = EvaluateExpression(binary.Left);
            RavelObject right;
            switch (binary.Op.Kind)
            {
                case RavelBinaryOperatorKind.ShortCutAnd:
                    {
                        if (!left.GetValue<bool>())
                        {
                            return Global.TypePool.False;
                        }
                        right = EvaluateExpression(binary.Right);
                        return right.GetValue<bool>() ? Global.TypePool.True : Global.TypePool.False;
                    }
                case RavelBinaryOperatorKind.ShortCutOr:
                    {
                        if (left.GetValue<bool>())
                        {
                            return Global.TypePool.True;
                        }
                        right = EvaluateExpression(binary.Right);
                        return right.GetValue<bool>() ? Global.TypePool.True : Global.TypePool.False;
                    }
            }
            right = EvaluateExpression(binary.Right);
            RavelObject l = binary.Op.Function.Invoke(left, right);
            return l;
            throw new InvalidOperationException($"Unexpected binary operator '{binary.Op}'");
        }

        private RavelObject EvaluateUnary(BoundUnaryExpression unary)
        {
            RavelObject operand = EvaluateExpression(unary.Operand);
            RavelUnaryOperator? op = unary.Operand.Type.GetOperator(unary.Op.SyntaxKind);
            if (op == null)
            {
                throw new InvalidOperationException($"Unexpected unary operator '{unary.Op}'");
            }
            return op.Function.Invoke(operand);
        }

        private RavelObject EvaluateVariable(BoundVariableExpression variable)
        {
            if (Scope.TryGetVariable(variable.Name, out RavelVariable? value))
            {
                var obj= value!.Object;
                if(value.IsFunctionSelf)
                {
                    obj = obj.Call(Global.TypePool.Unit);
                }
                return obj;
            }
            throw new NotSupportedException();
        }

        private RavelObject EvaluateDefining(BoundDefiningExpression Defining)
        {
            RavelObject exp = EvaluateExpression(Defining.Expression);
            if (!Scope.TryDeclare(Defining.Declare.Name, exp, false, false))
            {
                throw new InvalidOperationException();
            }
            return exp;
        }

        private static RavelObject EvaluateLiteral(BoundLiteralExpression num)
        {
            return num.Value;
        }
    }
}
