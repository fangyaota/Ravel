using Ravel.Binding;
using Ravel.Values;

namespace Ravel.Rewrite
{
    internal class Flatter : Rewriter
    {
        public RavelGlobal Global { get; }

        public Flatter(BoundProgram expression, RavelGlobal global) : base(expression)
        {
            Global = global;
        }
        protected override BoundExpression RewriteBlock(BoundBlockExpression block)
        {
            List<BoundExpression> expressions = new();
            bool diff = false;

            if (block.IsConst)
            {
                return Rewrite(block.Expressions.Last());
            }

            foreach (BoundExpression sent in block.Expressions)
            {
                BoundExpression item = Rewrite(sent);
                if (item is BoundBlockExpression blo && blo.Expressions.All(x => x is not BoundDefiningExpression))
                {
                    expressions.AddRange(blo.Expressions);
                    diff = true;
                    continue;
                }
                if (item != sent)
                {
                    diff = true;
                }
                expressions.Add(item);
            }
            if (!diff)
            {
                return block;
            }

            return new BoundBlockExpression(expressions);
        }
        protected override BoundExpression RewriteBinary(BoundBinaryExpression binary)
        {
            if (TryConstantFold(binary, out BoundExpression? result))
            {
                return result;
            }
            return base.RewriteBinary(binary);
        }
        protected override BoundExpression RewriteUnary(BoundUnaryExpression unary)
        {
            if (TryConstantFold(unary, out BoundExpression? result))
            {
                return result;
            }
            return base.RewriteUnary(unary);
        }
        private bool TryConstantFold(BoundExpression expression, out BoundExpression result)
        {
            if (expression.IsConst)
            {
                RavelObject obj = new NeoEvaluator(expression, Global).Evaluate();
                result = new BoundLiteralExpression(obj);
                return true;
            }
            result = expression;
            return false;
        }
    }
}
