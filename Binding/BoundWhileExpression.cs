using Ravel.Values;

namespace Ravel.Binding
{
    internal class BoundWhileExpression : BoundExpression
    {
        public BoundWhileExpression(BoundExpression condition, BoundExpression expr)
        {
            Condition = condition;
            Expression = expr;
        }

        public BoundExpression Condition { get; }
        public BoundExpression Expression { get; }

        public override RavelType Type => Expression.Type;

        public override bool IsConst => false;

        public override BoundNodeKind Kind => BoundNodeKind.WhileExpression;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Condition;
            yield return Expression;
        }
    }
}