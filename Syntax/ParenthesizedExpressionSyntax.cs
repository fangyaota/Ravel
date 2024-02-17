using Ravel.Text;

namespace Ravel.Syntax
{
    public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(SyntaxToken open, ExpressionSyntax expression, SyntaxToken close)
        {
            Open = open;
            Expression = expression;
            Close = close;
            Span = new(Open.Span.Start, Close.Span.End);
        }

        public SyntaxToken Open { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken Close { get; }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

        public override TextSpan Span { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Open;
            yield return Expression;
            yield return Close;
        }
    }
}
