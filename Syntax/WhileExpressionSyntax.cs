using Ravel.Text;

namespace Ravel.Syntax
{
    internal class WhileExpressionSyntax : ExpressionSyntax
    {
        public WhileExpressionSyntax(SyntaxToken @while, ExpressionSyntax condition, ExpressionSyntax expr)
        {
            While = @while;
            Condition = condition;
            Expression = expr;
            Span = new(@while.Span.Start, expr.Span.End);
        }

        public SyntaxToken While { get; }
        public ExpressionSyntax Condition { get; }
        public ExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.WhileExpression;

        public override TextSpan Span { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return While;
            yield return Condition;
            yield return Expression;
        }
    }
}