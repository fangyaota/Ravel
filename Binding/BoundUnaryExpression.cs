using Ravel.Values;

namespace Ravel.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(RavelUnaryOperator op, BoundExpression operand)
        {
            Op = op;
            Operand = operand;
        }

        public RavelUnaryOperator Op { get; }
        public BoundExpression Operand { get; }

        public override RavelType Type => Op.OperandType;

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;

        public override bool IsConst => Operand.IsConst && Op.Function.IsConst;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Operand;
        }
    }
}
