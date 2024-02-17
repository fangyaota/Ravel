using Ravel.Text;

namespace Ravel.Syntax
{
    public sealed class NameExpressionSyntax : ExpressionSyntax
    {
        public NameExpressionSyntax(SyntaxToken identifier)
        {
            Identifier = identifier;
            Span = identifier.Span;
        }
        public override SyntaxKind Kind => SyntaxKind.NameExpression;

        public override TextSpan Span { get; }

        public SyntaxToken Identifier { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
        }
    }
}
