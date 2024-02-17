using Ravel.Text;

namespace Ravel.Syntax
{
    public sealed class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryExpressionSyntax(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            Operand = operand;
            OperatorToken = operatorToken;
        }

        public ExpressionSyntax Operand { get; }
        public SyntaxToken OperatorToken { get; }

        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

        public override TextSpan Span => OperatorToken.Span;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperatorToken;
            yield return Operand;
        }
    }
}
