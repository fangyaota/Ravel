using Ravel.Text;

namespace Ravel.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitSyntax(ExpressionSyntax expression, SyntaxToken endOfFileToken)
        {
            Expression = expression;
            EndOfFileToken = endOfFileToken;
        }
        public override SyntaxKind Kind => SyntaxKind.CompilationUnitExpression;

        public override TextSpan Span => new(Expression.Span.Start, EndOfFileToken.Span.End);

        public ExpressionSyntax Expression { get; }
        public SyntaxToken EndOfFileToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
            yield return EndOfFileToken;
        }
    }
}
