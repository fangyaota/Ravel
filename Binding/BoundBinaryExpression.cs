using Ravel.Values;

namespace Ravel.Binding
{
    internal class BoundBinaryExpression : BoundExpression
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
    internal sealed class BoundAsExpression : BoundBinaryExpression
    {
        public BoundAsExpression(BoundExpression left, RavelBinaryOperator op, BoundExpression right, RavelType resultType)
            : base(left, op, right)
        {
            Type = resultType;
        }


        public override RavelType Type { get; }

        public override BoundNodeKind Kind => BoundNodeKind.AsExpression;

        public override bool IsConst => Left.IsConst && Right.IsConst && Op.Function.IsConst;
    }
}
