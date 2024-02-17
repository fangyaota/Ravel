using Ravel.Values;

namespace Ravel.Binding
{
    public sealed class BoundErrorExpression : BoundExpression
    {
        public override RavelType Type => throw new InvalidOperationException();

        public override bool IsConst => false;

        public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression;

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}
