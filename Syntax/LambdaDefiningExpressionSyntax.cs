using Ravel.Text;

namespace Ravel.Syntax
{
    public sealed class LambdaDefiningExpressionSyntax : ExpressionSyntax
    {
        public LambdaDefiningExpressionSyntax(ExpressionSyntax function, List<ExpressionSyntax> parameters)
        {
            Function = function;
            Parameters = parameters;
            Span = new(function.Span.Start, parameters.Last().Span.End);
        }

        public ExpressionSyntax Function { get; }
        public List<ExpressionSyntax> Parameters { get; }

        public override SyntaxKind Kind => SyntaxKind.LambdaDefiningExpression;

        public override TextSpan Span { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Function;
            foreach (ExpressionSyntax parameter in Parameters)
            {
                yield return parameter;
            }
        }
    }
}
