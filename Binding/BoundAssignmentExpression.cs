using Ravel.Values;

namespace Ravel.Binding
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(string name, BoundExpression expression)
        {
            Name = name;
            Expression = expression;
        }

        public override RavelType Type => Expression.Type;

        public override bool IsConst => false;

        public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;

        public string Name { get; }
        public BoundExpression Expression { get; }

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}
