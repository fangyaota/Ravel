using Ravel.Values;

namespace Ravel.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(RavelObject value)
        {
            Value = value;
        }

        public RavelObject Value { get; }

        public override RavelType Type => Value.Type;

        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;

        public override bool IsConst => true;

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}
