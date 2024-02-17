using Ravel.Text;

namespace Ravel.Syntax
{
    public sealed class DefiningExpressionSyntax : ExpressionSyntax
    {
        public DefiningExpressionSyntax(DeclareExpressionSyntax declare, SyntaxToken equalToken, ExpressionSyntax expression)
        {
            Declare = declare;
            EqualToken = equalToken;
            Expression = expression;
            Span = new(declare.Span.Start, expression.Span.End);
        }
        public override SyntaxKind Kind => SyntaxKind.DefiningExpression;

        public override TextSpan Span { get; }
        public DeclareExpressionSyntax Declare { get; }
        public SyntaxToken EqualToken { get; }
        public ExpressionSyntax Expression { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Declare;
            yield return EqualToken;
            yield return Expression;
        }
    }
}
