using Ravel.Text;
using Ravel.Values;

namespace Ravel.Syntax
{
    public class Parser
    {
        private int _position;
        public List<SyntaxToken> Tokens { get; }
        private readonly DiagnosticList _diagnostics;
        public Parser(SourceText text, SyntaxToken[] tokens, RavelGlobal global)
        {
            Tokens = new();
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].Kind is SyntaxKind.Sharp)
                {
                    while (i < tokens.Length && !tokens[i].Kind.IsEndOrLine())
                    {
                        i++;
                    }
                }
                if (tokens[i].Kind is SyntaxKind.WhiteSpace)
                {
                    continue;
                }
                Tokens.Add(tokens[i]);
            }

            Text = text;
            _diagnostics = new(Text);
            Global = global;
        }
        private SyntaxToken Peek(int offset)
        {
            int index = _position + offset;
            if (index >= Tokens.Count)
            {
                //_diagnostics.Add($"bad position {index} of Peek() in Parser");
                return Tokens[^1];
            }
            else
            {
                return Tokens[index];
            }
        }
        private SyntaxToken Current => Peek(0);
        private SyntaxToken Last => Peek(-1);
        public IEnumerable<Diagnostic> Diagnostics
        {
            get
            {
                return _diagnostics;
            }
        }

        public SourceText Text { get; }
        public RavelGlobal Global { get; }

        private SyntaxToken NextToken()
        {
            _position++;
            return Last;
        }
        private SyntaxToken MatchToken(params SyntaxKind[] kinds)
        {
            bool temp = false;
            return MatchToken(ref temp, kinds);
        }
        private SyntaxToken MatchToken(ref bool alreadyError, params SyntaxKind[] kinds)
        {
            if (kinds.Contains(Current.Kind))
            {
                return NextToken();
            }
            if (Current.Kind != SyntaxKind.BadToken)
            {
                if (_position < Tokens.Count && !alreadyError)
                {
                    _diagnostics.ReportMissingToken(Current.Span, Current, kinds.First());
                    alreadyError = true;
                }
            }
            return new(kinds.First(), _position++, "", default!);
        }
        public SyntaxTree Parse()
        {
            CompilationUnitSyntax unit = ParseCompilationUnit();
            return new(Diagnostics, unit);
        }
        /// <summary>
        /// 解析编译单元语句，包括EOF
        /// </summary>
        /// <returns></returns>
        private CompilationUnitSyntax ParseCompilationUnit()
        {
            ExpressionSyntax expression = ParseStatement();
            while (Current.Kind == SyntaxKind.EndOfLine)
            {
                NextToken();
            }
            SyntaxToken eof = MatchToken(SyntaxKind.EndOfFile);
            return new(expression, eof);
        }
        /// <summary>
        /// 解析函数调用表达式，不包括左右括号，如
        /// func a b
        /// </summary>
        /// <returns></returns>
        private ExpressionSyntax ParseFunction()
        {
            ExpressionSyntax func = ParseSingleton();
            if (Current.Kind.IsEndOrLine())
            {
                return func;
            }
            List<ExpressionSyntax> paramList = new();
            do
            {
                paramList.Add(ParseSingleton());
            } while (!Current.Kind.IsEndOrLine());
            if (paramList.Any(e => e is NameExpressionSyntax name && name.Identifier.Text == "_"))
            {
                return new LambdaDefiningExpressionSyntax(func, paramList);
            }
            return new FunctionCallExpressionSyntax(func, paramList);
        }
        /// <summary>
        /// 解析单目运算符表达式，如
        /// -a
        /// </summary>
        /// <param name="parentPrecedence"></param>
        /// <returns></returns>
        private ExpressionSyntax ParseUnary(int parentPrecedence)
        {
            int unaryPrecedence = Global.SyntaxFacts.GetUnaryOperatorPrecedence(Current.Kind);
            if (unaryPrecedence != 0 && unaryPrecedence >= parentPrecedence)
            {
                SyntaxToken unaryOperator = NextToken();
                ExpressionSyntax operand = ParseSingleton(unaryPrecedence);
                return new UnaryExpressionSyntax(operand, unaryOperator);
            }
            else
            {
                return ParseDot();
            }
        }
        /// <summary>
        /// 解析子变量表达式，如
        /// a.b
        /// </summary>
        /// <returns></returns>
        private ExpressionSyntax ParseDot()
        {
            ExpressionSyntax left = ParsePrimary();
            while (Current.Kind is SyntaxKind.Dot)
            {
                SyntaxToken dot = MatchToken(SyntaxKind.Dot);
                SyntaxToken right = MatchToken(SyntaxKind.Variable);
                left = new DotExpresionSyntax(left, dot, right);
            }
            return left;
        }
        /// <summary>
        /// 解析单例表达式，包括
        /// if、while、双目表达式
        /// </summary>
        /// <param name="parentPrecedence"></param>
        /// <returns></returns>
        private ExpressionSyntax ParseSingleton(int parentPrecedence = 0)//?
        {
            return Current.Kind switch
            {
                SyntaxKind.If => ParseIf(),
                SyntaxKind.While => ParseWhile(),
                _ => ParseBinary(parentPrecedence),
            };
        }
        /// <summary>
        /// 解析while表达式
        /// while cond singleton
        /// </summary>
        /// <returns></returns>
        private ExpressionSyntax ParseWhile()
        {
            SyntaxToken @while = MatchToken(SyntaxKind.While);
            ExpressionSyntax condition = ParseSingleton();
            ExpressionSyntax expr = ParseSingleton();
            return new WhileExpressionSyntax(@while, condition, expr);
        }
        /// <summary>
        /// 解析双目表达式，如
        /// a + b
        /// </summary>
        /// <param name="parentPrecedence"></param>
        /// <returns></returns>
        private ExpressionSyntax ParseBinary(int parentPrecedence = 0)
        {

            ExpressionSyntax left = ParseUnary(parentPrecedence);
            List<ExpressionSyntax> expressions = new();
            List<SyntaxToken> operators = new();
            expressions.Add(left);
            OperatorDirection lastDirection = OperatorDirection.None;
            while (true)
            {
                int binaryPrecedence = Global.SyntaxFacts.GetBinaryOperatorPrecedence(Current.Kind);
                if (binaryPrecedence == 0)
                {
                    break;
                }
                if (binaryPrecedence < parentPrecedence)
                {
                    break;
                }
                if (binaryPrecedence > parentPrecedence)
                {
                    SyntaxToken bi = NextToken();
                    ExpressionSyntax rig = ParseSingleton(binaryPrecedence);
                    expressions[^1] = new BinaryExpressionSyntax(expressions[^1], bi, rig);
                    continue;
                }
                SyntaxToken binaryOperator = NextToken();
                ExpressionSyntax right = ParseSingleton(binaryPrecedence);
                expressions.Add(right);
                operators.Add(binaryOperator);
                OperatorDirection nowDirection = Global.SyntaxFacts.GetBinaryOperatorDirection(binaryOperator.Kind);

                if (lastDirection != nowDirection && lastDirection != OperatorDirection.None)
                {
                    _diagnostics.ReportDirectionNotSame(binaryOperator, lastDirection);
                }
                lastDirection = nowDirection;
            }
            switch (lastDirection)
            {
                case OperatorDirection.Left:
                    {
                        ExpressionSyntax main = expressions.First();
                        for (int i = 1; i < expressions.Count; i++)
                        {
                            main = new BinaryExpressionSyntax(main, operators[i - 1], expressions[i]);
                        }
                        return main;
                    }
                case OperatorDirection.Right:
                    {
                        ExpressionSyntax main = expressions.Last();
                        for (int i = expressions.Count - 2; i >= 0; i--)
                        {
                            main = new BinaryExpressionSyntax(expressions[i], operators[i], main);
                        }
                        return main;
                    }
                default:
                    return expressions[0];
            }

        }
        /// <summary>
        /// 解析声明变量表达式，如
        /// a : int
        /// </summary>
        /// <param name="mustHaveType"></param>
        /// <returns></returns>
        private DeclareExpressionSyntax ParseDeclare(bool mustHaveType = false)
        {
            bool haserror = false;
            SyntaxToken variable = MatchToken(ref haserror, SyntaxKind.Variable);
            SyntaxToken colon = MatchToken(ref haserror, SyntaxKind.Colon);
            List<SyntaxToken> tokens = new();
            while (Current.Kind.IsVariableKeyword())
            {
                tokens.Add(NextToken());
            }
            if (Current.Kind != SyntaxKind.Equal && !Current.Kind.IsEndOrLine())
            {
                ExpressionSyntax expr = ParseSingleton();
                return new DeclareExpressionSyntax(variable, colon, tokens, expr);
            }
            if (mustHaveType && !haserror)
            {
                _diagnostics.ReportFunctionParameterNoType(variable);
            }
            return new DeclareExpressionSyntax(variable, colon, tokens);
        }
        /// <summary>
        /// 解析变量定义表达式，如
        /// a : int = 666
        /// </summary>
        /// <returns></returns>
        private ExpressionSyntax ParseDefining()
        {
            DeclareExpressionSyntax declare = ParseDeclare();
            if (Current.Kind is not SyntaxKind.Equal)
            {
                return declare;
            }
            SyntaxToken equal = MatchToken(SyntaxKind.Equal);
            ExpressionSyntax expression = ParseSentence();
            return new DefiningExpressionSyntax(declare, equal, expression);

        }
        /// <summary>
        /// 解析变量赋值表达式，如
        /// a = 6
        /// </summary>
        /// <returns></returns>
        private ExpressionSyntax ParseAssignment()
        {
            SyntaxToken name = MatchToken(SyntaxKind.Variable);
            SyntaxToken e = MatchToken(SyntaxKind.Equal);
            ExpressionSyntax expression = ParseSentence();
            return new AssignmentExpressionSyntax(name, e, expression);
        }
        /// <summary>
        /// 解析语句，包括表达式与结束符
        /// </summary>
        /// <returns></returns>
        private ExpressionSyntax ParseStatement()
        {
            ExpressionSyntax left;
            left = ParseSentence();
            SyntaxToken eol;
            if (Current.Kind is SyntaxKind.CloseHesis)
            {
                eol = Current;
            }
            else
            {
                eol = MatchToken(SyntaxKind.EndOfLine, SyntaxKind.SemiColon, SyntaxKind.EndOfFile);
            }
            return new StatementSyntax(left, eol);
        }
        /// <summary>
        /// 解析if表达式，如
        /// if cond true false
        /// </summary>
        /// <returns></returns>
        private ExpressionSyntax ParseIf()
        {
            SyntaxToken @if = MatchToken(SyntaxKind.If);
            ExpressionSyntax condition = ParseSingleton();
            ExpressionSyntax expTrue = ParseSingleton();
            ExpressionSyntax expFalse = ParseSingleton();
            return new IfExpressionSyntax(@if, condition, expTrue, expFalse);
        }
        /// <summary>
        /// 解析表达式，包括变量定义、赋值、函数调用，不包括结束符
        /// </summary>
        /// <returns></returns>
        private ExpressionSyntax ParseSentence()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.Variable:
                    {
                        if (Peek(1).Kind == SyntaxKind.Colon)
                        {
                            return ParseDefining();
                        }
                        else if (Peek(1).Kind == SyntaxKind.Equal)
                        {
                            return ParseAssignment();
                        }
                        else
                        {
                            return ParseFunction();
                        }
                    }
                default:
                    {
                        return ParseFunction();
                    }
            }
        }
        /// <summary>
        /// 解析括号表达式，包括左右括号，如
        /// (1 + 2) * 3
        /// </summary>
        /// <returns></returns>
        private ExpressionSyntax ParseParenthesized()
        {
            List<ExpressionSyntax> l = new();
            SyntaxToken open = MatchToken(SyntaxKind.OpenHesis);
            while (Current.Kind is SyntaxKind.EndOfLine)
            {
                NextToken();
            }
            while (!Current.Kind.IsEnd())
            {
                while (Current.Kind is SyntaxKind.EndOfLine)
                {
                    NextToken();
                }
                l.Add(ParseStatement());
            }
            while (Current.Kind is SyntaxKind.EndOfLine)
            {
                NextToken();
            }
            SyntaxToken close = MatchToken(SyntaxKind.CloseHesis);
            return new BlockSyntax(open, l, close);
        }
        /// <summary>
        /// 解析列表表达式，如
        /// [a b c]
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private ExpressionSyntax ParseList()
        {
            List<ExpressionSyntax> l = new();
            SyntaxToken open = MatchToken(SyntaxKind.OpenBracket);
            while (Current.Kind is SyntaxKind.EndOfLine)
            {
                NextToken();
            }
            while (!Current.Kind.IsEnd())
            {
                while (Current.Kind is SyntaxKind.EndOfLine)
                {
                    NextToken();
                }
                l.Add(ParsePrimary());
            }
            while (Current.Kind is SyntaxKind.EndOfLine)
            {
                NextToken();
            }
            SyntaxToken close = MatchToken(SyntaxKind.CloseBracket);
            return new ListSyntax(open, l, close);
        }
        /// <summary>
        /// 解析括号或函数定义表达式
        /// </summary>
        /// <returns></returns>
        private ExpressionSyntax ParseParenthesizedOrFunctionDefining()
        {
            if (Peek(1).Kind == SyntaxKind.CloseHesis)
            {
                int start = _position;
                _position += 2;
                return new LiteralExpressionSyntax(new(SyntaxKind.None, start, "()", Global.TypePool.Unit));
            }

            int offset = 1;
            int cost_of_open_close = 1;
            while (cost_of_open_close > 0 && _position < Tokens.Count)
            {
                SyntaxToken token = Peek(offset);
                if (token.Kind == SyntaxKind.CloseHesis)
                {
                    cost_of_open_close--;
                }
                else if (token.Kind == SyntaxKind.OpenHesis)
                {
                    cost_of_open_close++;
                }
                offset++;
            }
            if (cost_of_open_close != 0)
            {
                _diagnostics.ReportOpenCloseNotMatch(new TextSpan(_position, _position + offset));
            }
            
            if (Peek(offset).Kind is SyntaxKind.EqualLarge or SyntaxKind.Colon)
            {
                return ParseFunctionDefining();
            }
            return ParseParenthesized();
        }
        /// <summary>
        /// 解析函数定义表达式
        /// </summary>
        /// <returns></returns>
        private ExpressionSyntax ParseFunctionDefining()
        {

            SyntaxToken left = MatchToken(SyntaxKind.OpenHesis);
            List<DeclareExpressionSyntax> paramList = new();
            while (!Current.Kind.IsEnd())
            {
                DeclareExpressionSyntax declare = ParseDeclare(true);
                paramList.Add(declare);
            }
            bool hasError = false;
            SyntaxToken right = MatchToken(ref hasError, SyntaxKind.CloseHesis);
            if (paramList.Count == 0)
            {
                _diagnostics.ReportFunctionNoParameters(right.Span);
            }
            SyntaxToken? colon = null;
            SyntaxToken? typeIdentifier = null;
            if (Current.Kind is SyntaxKind.Colon)
            {
                colon = MatchToken(ref hasError, SyntaxKind.Colon);
                typeIdentifier = MatchToken(ref hasError, SyntaxKind.Variable);
            }
            SyntaxToken equalLarge = MatchToken(ref hasError, SyntaxKind.EqualLarge);
            ExpressionSyntax sentence = ParseSentence();
            return new FunctionDefiningExpressionSyntax(left, paramList, right, colon, typeIdentifier, equalLarge, sentence);
        }
        /// <summary>
        /// 解析原子表达式，不包括单、双运算符，如：
        /// (a)、[a]、a
        /// </summary>
        /// <returns></returns>
        private ExpressionSyntax ParsePrimary()
        {
            int start = _position;
        primary:
            switch (Current.Kind)
            {
                case SyntaxKind.OpenHesis:
                    {
                        return ParseParenthesizedOrFunctionDefining();
                    }
                case SyntaxKind.OpenBracket:
                    {
                        return ParseList();
                    }
                case SyntaxKind.Integer:
                case SyntaxKind.Boolean:
                case SyntaxKind.Void:
                case SyntaxKind.None:
                case SyntaxKind.String:
                    {
                        return new LiteralExpressionSyntax(NextToken());
                    }
                case SyntaxKind.Variable:
                    {
                        if (Peek(1).Kind == SyntaxKind.Colon)
                        {
                            return ParseDefining();
                        }
                        SyntaxToken variable = MatchToken(SyntaxKind.Variable);
                        return new NameExpressionSyntax(variable);
                    }
                case SyntaxKind.BadToken:
                    {
                        SyntaxToken next = NextToken();
                        return new LiteralExpressionSyntax(next);
                    }
                case SyntaxKind.EndOfFile:
                    {
                        _diagnostics.ReportMissingToken(new(start, _position), Current, SyntaxKind.Variable);
                        SyntaxToken next = NextToken();
                        return new LiteralExpressionSyntax(next);
                    }
                case SyntaxKind.EndOfLine:
                    {
                        SyntaxToken next = NextToken();
                        goto primary;
                    }
                default:
                    {
                        _diagnostics.ReportNotSupportedToken(Current);
                        SyntaxToken next = NextToken();
                        return new LiteralExpressionSyntax(next);
                    }
            }
        }

        
    }
}
