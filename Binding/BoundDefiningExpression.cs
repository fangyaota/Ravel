using Ravel.Values;

namespace Ravel.Binding
{
    public sealed class BoundDefiningExpression : BoundExpression
    {
        public BoundDefiningExpression(lDeclare declare, BoundExpression expression)
        {
            Declare = declare;
            Expression = expression;
        }

        public override RavelType Type => Declare.Type;

        public override BoundNodeKind Kind => BoundNodeKind.DefiningExpression;

        public lDeclare Declare { get; }
        public BoundExpression Expression { get; }

        public override bool IsConst => Expression.IsConst;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Expression;
        }
    }
}
