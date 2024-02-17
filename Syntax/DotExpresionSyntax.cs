using Ravel.Text;

namespace Ravel.Syntax
{
    internal class DotExpresionSyntax : ExpressionSyntax
    {
        public DotExpresionSyntax(ExpressionSyntax left, SyntaxToken dot, SyntaxToken right)
        {
            Left = left;
            Dot = dot;
            Right = right;
            Span = new(Left.Span.Start, Right.Span.End);
        }

        public ExpressionSyntax Left { get; }
        public SyntaxToken Dot { get; }
        public SyntaxToken Right { get; }

        public override SyntaxKind Kind => SyntaxKind.DotExpression;

        public override TextSpan Span { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return Dot;
            yield return Right;
        }
    }
}