using Ravel.Values;

namespace Ravel.Binding
{
    public sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(lDeclare declare)
        {
            Declare = declare;
        }
        public override RavelType Type => Declare.Type;

        public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;

        public string Name => Declare.Name;

        public override bool IsConst => Declare.IsConst;//?

        public lDeclare Declare { get; }

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}
