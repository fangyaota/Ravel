using Ravel.Values;

namespace Ravel.Binding
{
    public sealed class BoundBlockExpression : BoundExpression
    {
        public BoundBlockExpression(List<BoundExpression> expressions)
        {
            Expressions = expressions;
            Type = Expressions.Last().Type;
        }
        public override RavelType Type { get; }

        public override bool IsConst => Expressions.All(x => x.IsConst);

        public override BoundNodeKind Kind => BoundNodeKind.BlockExpression;

        public List<BoundExpression> Expressions { get; }

        public override IEnumerable<BoundNode> GetChildren()
        {
            foreach (BoundExpression expression in Expressions)
            {
                yield return expression;
            }
        }
    }
}
