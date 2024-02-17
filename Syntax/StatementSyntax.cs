using Ravel.Text;

namespace Ravel.Syntax
{
    public sealed class StatementSyntax : ExpressionSyntax
    {
        public StatementSyntax(ExpressionSyntax expression, SyntaxToken endOfLine)
        {
            Expression = expression;
            EndOfLine = endOfLine;
        }

        public ExpressionSyntax Expression { get; }
        public SyntaxToken EndOfLine { get; }

        public override SyntaxKind Kind => SyntaxKind.StatementExpression;

        public override TextSpan Span => Expression.Span;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
            yield return EndOfLine;
        }
    }
}
