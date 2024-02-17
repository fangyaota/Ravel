using Ravel.Values;

namespace Ravel.Binding
{
    internal class BoundDotExpression : BoundExpression
    {

        public BoundDotExpression(BoundExpression owner, string son, RavelType type)
        {
            Owner = owner;
            Son = son;
            Type = type;
        }

        public BoundExpression Owner { get; }
        public string Son { get; }

        public override RavelType Type { get; }

        public override bool IsConst => Owner.IsConst;//?

        public override BoundNodeKind Kind => BoundNodeKind.DotExpression;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Owner;
        }
    }
}