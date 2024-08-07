﻿using Ravel.Values;

namespace Ravel.Binding
{
    internal class BoundIfExpression : BoundExpression
    {
        public BoundIfExpression(BoundExpression condition, BoundExpression expTrue, BoundExpression expFalse, RavelType returnType)
        {
            Condition = condition;
            ExpTrue = expTrue;
            ExpFalse = expFalse;
            Type = returnType;
        }

        public BoundExpression Condition { get; }
        public BoundExpression ExpTrue { get; }
        public BoundExpression ExpFalse { get; }

        public override RavelType Type { get; }

        public override bool IsConst => Condition.IsConst && ExpTrue.IsConst && ExpFalse.IsConst;

        public override BoundNodeKind Kind => BoundNodeKind.IfExpression;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Condition;
            yield return ExpTrue;
            yield return ExpFalse;
        }
    }
}