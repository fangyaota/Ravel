using Ravel.Values;

namespace Ravel.Binding
{
    internal sealed class BoundFunctionCallExpression : BoundExpression
    {
        public BoundFunctionCallExpression(BoundExpression function, List<BoundExpression> parameters)
        {
            Function = function;
            Parameters = parameters;
            Type = function.Type.GetTypeWhenCall(parameters.Count);
        }

        public BoundExpression Function { get; }
        public List<BoundExpression> Parameters { get; }

        public override RavelType Type { get; }

        public override bool IsConst => Function.IsConst && Parameters.All(x => x.IsConst);//?

        public override BoundNodeKind Kind => BoundNodeKind.FunctionCallExpression;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Function;
            foreach (BoundNode node in Parameters)
            {
                yield return node;
            }
        }
    }
}
