using Ravel.Text;

namespace Ravel.Syntax
{
    public sealed class BlockSyntax : ExpressionSyntax
    {
        public BlockSyntax(SyntaxToken open, List<ExpressionSyntax> statements, SyntaxToken close)
        {
            Span = new(open.Span.Start, close.Span.End);
            Open = open;
            Statements = statements;
            Close = close;
        }


        public override SyntaxKind Kind => SyntaxKind.BlockExpression;

        public override TextSpan Span { get; }
        public SyntaxToken Open { get; }
        public List<ExpressionSyntax> Statements { get; }
        public SyntaxToken Close { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Open;
            foreach (ExpressionSyntax parameter in Statements)
            {
                yield return parameter;
            }
            yield return Close;
        }
    }
}
