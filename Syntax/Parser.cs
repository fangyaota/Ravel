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
            for(int i = 0; i < tokens.Length; i++)
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
            if (kinds.Contains(Current.Kind))
            {
                return NextToken();
            }
            if (Current.Kind != SyntaxKind.BadToken)
            {
                if (_position < Tokens.Count)
                {
                    _diagnostics.ReportMissingToken(Current.Span, Current, kinds.First());
                }
            }
            return new(kinds.First(), _position++, "", default!);
        }

        public SyntaxTree Parse()
        {
            CompilationUnitSyntax unit = ParseCompilationUnit();
            return new(Diagnostics, unit);
        }
        private CompilationUnitSyntax ParseCompilationUnit()
        {
            ExpressionSyntax expression = ParseStatement();
            SyntaxToken eof = MatchToken(SyntaxKind.EndOfFile);
            return new(expression, eof);
        }
        private ExpressionSyntax ParseFunction()
        {
            ExpressionSyntax func = ParseSingleton();
            if (Current.Kind.IsEndOrLine())
            {
                return func;
            }
            List<ExpressionSyntax> paramList = new List<ExpressionSyntax>();
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
        private ExpressionSyntax ParseUnary(int parentPrecedence)
        {
            int unaryPrecedence = Global.SyntaxFacts.GetUnaryOperatorPrecedence(Current.Kind);
            if (unaryPrecedence != 0 && unaryPrecedence >= parentPrecedence)
            {
                SyntaxToken unaryOperator = NextToken();
                ExpressionSyntax operand = ParseUnary(parentPrecedence);
                return new UnaryExpressionSyntax(operand, unaryOperator);
            }
            else
            {
                return ParseDot();
            }
        }
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
        private ExpressionSyntax ParseSingleton(int parentPrecedence = 0)//?
        {
            switch (Current.Kind)
            {
                case SyntaxKind.If:
                    return ParseIf();
                case SyntaxKind.While:
                    return ParseWhile();
                default:
                    return ParseBinary(parentPrecedence);
            }
        }

        private ExpressionSyntax ParseWhile()
        {
            SyntaxToken @while = MatchToken(SyntaxKind.While);
            ExpressionSyntax condition = ParseSingleton();
            ExpressionSyntax expr = ParseSingleton();
            return new WhileExpressionSyntax(@while, condition, expr);
        }

        private ExpressionSyntax ParseBinary(int parentPrecedence = 0)
        {

            ExpressionSyntax left = ParseUnary(parentPrecedence);
            List<ExpressionSyntax> expressions = new ();
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
                    var rig = ParseSingleton(binaryPrecedence);
                    expressions[^1] = new BinaryExpressionSyntax(expressions[^1], bi, rig);
                    continue;
                }
                SyntaxToken binaryOperator = NextToken();
                ExpressionSyntax right = ParseSingleton(binaryPrecedence);
                expressions.Add(right);
                operators.Add(binaryOperator);
                var nowDirection = Global.SyntaxFacts.GetBinaryOperatorDirection(binaryOperator.Kind);

                if(lastDirection != nowDirection && lastDirection != OperatorDirection.None)
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
                        for (var i = 1; i < expressions.Count; i++)
                        {
                            main = new BinaryExpressionSyntax(main, operators[i - 1], expressions[i]);
                        }
                        return main;
                    }
                case OperatorDirection.Right:
                    {
                        ExpressionSyntax main = expressions.Last();
                        for (var i = expressions.Count - 2; i >= 0; i--)
                        {
                            main = new BinaryExpressionSyntax(expressions[i], operators[i], main);
                        }
                        return main;
                    }
                default:
                    return expressions[0];
            }

        }
        private DeclareExpressionSyntax ParseDeclare(bool mustHaveType = false)
        {
            SyntaxToken variable = MatchToken(SyntaxKind.Variable);
            SyntaxToken colon = MatchToken(SyntaxKind.Colon);
            List<SyntaxToken> tokens = new();
            while (Current.Kind.IsVariableKeyword())
            {
                tokens.Add(NextToken());
            }
            if (Current.Kind != SyntaxKind.Equal && !Current.Kind.IsEndOrLine())
            {
                var expr = ParseSingleton();
                return new DeclareExpressionSyntax(variable, colon, tokens, expr);
            }
            if(mustHaveType)
            {
                _diagnostics.ReportFunctionParameterNoType(variable);
            }
            return new DeclareExpressionSyntax(variable, colon, tokens);
        }
        private ExpressionSyntax ParseDefining()
        {
            DeclareExpressionSyntax declare = ParseDeclare();
            if(Current.Kind is not SyntaxKind.Equal)
            {
                return declare;
            }
            SyntaxToken equal = MatchToken(SyntaxKind.Equal);
            ExpressionSyntax expression = ParseSentence();
            return new DefiningExpressionSyntax(declare, equal, expression);

        }
        private ExpressionSyntax ParseAssignment()
        {
            SyntaxToken name = MatchToken(SyntaxKind.Variable);
            SyntaxToken e = MatchToken(SyntaxKind.Equal);
            ExpressionSyntax expression = ParseSentence();
            return new AssignmentExpressionSyntax(name, e, expression);
        }
        private ExpressionSyntax ParseStatement()
        {
            ExpressionSyntax left;
            left = ParseSentence();
            SyntaxToken eol;
            if (Current.Kind is SyntaxKind.CloseBrace)
            {
                eol = Current;
            }
            else
            {
                eol = MatchToken(SyntaxKind.EndOfLine, SyntaxKind.SemiColon, SyntaxKind.EndOfFile);
            }
            return new StatementSyntax(left, eol);
        }
        private ExpressionSyntax ParseIf()
        {
            SyntaxToken @if = MatchToken(SyntaxKind.If);
            ExpressionSyntax condition = ParseSingleton();
            ExpressionSyntax expTrue = ParseSingleton();
            ExpressionSyntax expFalse = ParseSingleton();
            return new IfExpressionSyntax(@if, condition, expTrue, expFalse);
        }
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
        private ExpressionSyntax ParseBlock()
        {
            List<ExpressionSyntax> l = new List<ExpressionSyntax>();
            SyntaxToken open = MatchToken(SyntaxKind.OpenBrace);
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
            SyntaxToken close = MatchToken(SyntaxKind.CloseBrace);
            return new BlockSyntax(open, l, close);
        }
        private ExpressionSyntax ParseParenthesized()
        {
            SyntaxToken left = MatchToken(SyntaxKind.OpenHesis);
            ExpressionSyntax expression = ParseSentence();
            SyntaxToken right = MatchToken(SyntaxKind.CloseHesis);
            return new ParenthesizedExpressionSyntax(left, expression, right);
        }
        private ExpressionSyntax ParseParenthesizedOrFunctionDefining()
        {
            int offset = 1;
            int cost = 1;
            bool empty = true;
            while (cost > 0 && _position < Tokens.Count)
            {
                SyntaxToken token = Peek(offset);
                if (token.Kind.IsEnd())
                {
                    cost--;
                }
                else if (token.Kind.IsStart())
                {
                    cost++;
                }
                else if (token.Kind is not SyntaxKind.EndOfLine)
                {
                    empty = false;
                }
                offset++;
            }
            if (cost != 0)
            {
                _diagnostics.ReportOpenCloseNotMatch(new TextSpan(_position, _position + offset));
            }
            if (empty)
            {
                int start = _position;
                _position += offset;
                return new LiteralExpressionSyntax(new(SyntaxKind.None, start, "()", Global.TypePool.None));
            }
            if (Peek(offset).Kind is SyntaxKind.EqualLarge or SyntaxKind.MinusLarge)
            {
                return ParseFunctionDefining();
            }
            return ParseParenthesized();
        }
        private ExpressionSyntax ParseFunctionDefining()
        {

            SyntaxToken left = MatchToken(SyntaxKind.OpenHesis);
            List<DeclareExpressionSyntax> paramList = new List<DeclareExpressionSyntax>();
            while (!Current.Kind.IsEnd())
            {
                DeclareExpressionSyntax declare = ParseDeclare(true);
                paramList.Add(declare);
            }
            SyntaxToken right = MatchToken(SyntaxKind.CloseHesis);
            if (paramList.Count == 0)
            {
                _diagnostics.ReportFunctionNoParameters(right.Span);
            }
            SyntaxToken? minusLarge = null;
            SyntaxToken? typeIdentifier = null;
            if (Current.Kind is SyntaxKind.MinusLarge)
            {
                minusLarge = MatchToken(SyntaxKind.MinusLarge);
                typeIdentifier = MatchToken(SyntaxKind.Variable);
            }
            SyntaxToken equalLarge = MatchToken(SyntaxKind.EqualLarge);
            ExpressionSyntax sentence = ParseSentence();
            return new FunctionDefiningExpressionSyntax(left, paramList, right, minusLarge, typeIdentifier, equalLarge, sentence);
        }
        private ExpressionSyntax ParsePrimary()
        {
            var start = _position;
        primary:
            switch (Current.Kind)
            {
                case SyntaxKind.OpenHesis:
                    {
                        return ParseParenthesizedOrFunctionDefining();
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
                case SyntaxKind.OpenBrace:
                    {
                        return ParseBlock();
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
