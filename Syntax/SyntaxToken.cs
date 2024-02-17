using Ravel.Text;
using Ravel.Values;

namespace Ravel.Syntax
{
    public class SyntaxToken : SyntaxNode
    {
        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public RavelObject Value { get; }
        public override TextSpan Span { get; }

        public SyntaxToken(SyntaxKind kind, int position, string text, RavelObject value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
            Span = new(position, position + text.Length);
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}
