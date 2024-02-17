
using Ravel.Text;

namespace Ravel.Syntax
{
    internal class AssignmentExpressionSyntax : ExpressionSyntax
    {

        public AssignmentExpressionSyntax(SyntaxToken variable, SyntaxToken equal, ExpressionSyntax expression)
        {
            Variable = variable;
            Equal = equal;
            Expression = expression;
            Span = new(Equal.Span.Start, expression.Span.End);
        }

        public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;

        public override TextSpan Span { get; }
        public SyntaxToken Variable { get; }
        public SyntaxToken Equal { get; }
        public ExpressionSyntax Expression { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Variable;
            yield return Equal;
            yield return Expression;
        }
    }
}