using Ravel.Values;

namespace Ravel.Binding
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, RavelBinaryOperator op, BoundExpression right)
        {
            Left = left;
            Op = op;
            Right = right;
        }

        public BoundExpression Left { get; }
        public RavelBinaryOperator Op { get; }
        public BoundExpression Right { get; }

        public override RavelType Type => Op.ResultType;

        public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;

        public override bool IsConst => Left.IsConst && Right.IsConst && Op.Function.IsConst;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Left;
            yield return Right;
        }
    }
}
