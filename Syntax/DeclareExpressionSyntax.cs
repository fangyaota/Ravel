using Ravel.Text;

namespace Ravel.Syntax
{
    public sealed class DeclareExpressionSyntax : ExpressionSyntax
    {
        public DeclareExpressionSyntax(SyntaxToken identifier, SyntaxToken colonToken, List<SyntaxToken> tokens, ExpressionSyntax type)
            : this(identifier, colonToken, tokens)
        {
            Type = type;
            Span = new(Identifier.Span.Start, Type.Span.End);
        }
        public DeclareExpressionSyntax(SyntaxToken identifier, SyntaxToken colonToken, List<SyntaxToken> tokens)
        {
            Identifier = identifier;
            ColonToken = colonToken;
            Tokens = tokens;
            IsReadonly = tokens.Any(x => x.Kind is SyntaxKind.Readonly);
            IsConst = tokens.Any(x => x.Kind == SyntaxKind.Const);
            Span = new(Identifier.Span.Start, ColonToken.Span.End);
        }
        public SyntaxToken Identifier { get; }
        public SyntaxToken ColonToken { get; }
        public List<SyntaxToken> Tokens { get; }
        public ExpressionSyntax? Type { get; }
        public bool IsReadonly { get; }
        public bool IsConst { get; }

        public override SyntaxKind Kind => SyntaxKind.DeclareExpression;

        public override TextSpan Span { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
            yield return ColonToken;
            if (Type != null) yield return Type;
        }
    }
}
