using Ravel.Values;

namespace Ravel.Binding
{
    public sealed class BoundErrorExpression : BoundExpression
    {
        public BoundErrorExpression(RavelType errorType)
        {
            Type = errorType;
        }
        public override RavelType Type { get; }

        public override bool IsConst => false;

        public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression;

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}
