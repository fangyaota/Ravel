using Ravel.Values;

namespace Ravel.Binding
{
    public sealed class BoundListExpression : BoundExpression
    {
        public BoundListExpression(List<BoundExpression> expressions, RavelType type)
        {
            Expressions = expressions;
            Type = type;
        }
        public override RavelType Type { get; }

        public override bool IsConst => Expressions.All(x => x.IsConst);

        public override BoundNodeKind Kind => BoundNodeKind.ListExpression;

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
