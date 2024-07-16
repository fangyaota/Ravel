using Ravel.Binding;

namespace Ravel.Values
{
    public abstract class RavelFunction
    {
        public RavelType[] RealParameters { get; }
        public RavelType ResultType { get; }
        public RavelType Type { get; internal set; }

        public RavelTypePool TypePool => Type.TypePool;
        public abstract bool IsConst { get; }

        public RavelFunction(RavelType type, int realParameterCount)
        {
            Type = type;
            RealParameters = type.GetFuncParameters(realParameterCount).ToArray();
            ResultType = type.GetTypeWhenCall(realParameterCount);
        }

        public void Invoke(NeoEvaluator evaluator, params RavelObject[] obj)
        {
            if (obj.Length < RealParameters.Length)
            {
                RavelType newType = TypePool.GetFuncType(ResultType, RealParameters[obj.Length..]);
                evaluator.AddResultAndReturn(RavelObject.GetFunction(new RavelParcialFunction(this, obj, newType)));
                return;
            }
            if (obj.Length == RealParameters.Length)
            {
                InvokeMust(evaluator, obj);
                return;
            }
            var functionExp = new BoundLiteralExpression(RavelObject.GetFunction(this));
            var formerParamExp = obj[..RealParameters.Length].Select(x => (BoundExpression)new BoundLiteralExpression(x)).ToList();
            var latterParamExp = obj[RealParameters.Length..].Select(x => (BoundExpression)new BoundLiteralExpression(x)).ToList();
            var expression = new BoundFunctionCallExpression(new BoundFunctionCallExpression(functionExp, formerParamExp), latterParamExp);

            evaluator.CurrentCallStack = new(evaluator.CurrentCallStack, expression);
        }
        protected abstract void InvokeMust(NeoEvaluator evaluator, RavelObject[] obj);
    }
}
