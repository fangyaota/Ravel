using Ravel.Text;

namespace Ravel.Syntax
{
    internal class IfExpressionSyntax : ExpressionSyntax
    {
        public IfExpressionSyntax(SyntaxToken @if, ExpressionSyntax condition, ExpressionSyntax expTrue, ExpressionSyntax expFalse)
        {
            If = @if;
            Condition = condition;
            ExpTrue = expTrue;
            ExpFalse = expFalse;
            Span = new(@if.Span.Start, expFalse.Span.End);
        }

        public SyntaxToken If { get; }
        public ExpressionSyntax Condition { get; }
        public ExpressionSyntax ExpTrue { get; }
        public ExpressionSyntax ExpFalse { get; }

        public override SyntaxKind Kind => SyntaxKind.IfExpression;

        public override TextSpan Span { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return If;
            yield return Condition;
            yield return ExpTrue;
            yield return ExpFalse;
        }
    }
}