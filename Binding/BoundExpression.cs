using Ravel.Values;

namespace Ravel.Binding
{
    public abstract class BoundExpression : BoundNode
    {
        public abstract RavelType Type { get; }
        public abstract bool IsConst { get; }
    }
}
