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
        public RavelObject GetRavelObject()
        {
            return new(this, Type, TypePool);
        }
        public void Invoke(NeoEvaluator evaluator, params RavelObject[] obj)
        {
            if (obj.Length < RealParameters.Length)
            {
                RavelType newType = TypePool.GetFuncType(ResultType, RealParameters[obj.Length..]);
                evaluator.AddResult(new RavelParcialFunction(this, obj, newType).GetRavelObject());
                return;
            }
            if (obj.Length == RealParameters.Length)
            {
                InvokeMust(evaluator, obj);
                return;
            }
            BoundLiteralExpression functionExp = new(GetRavelObject());
            List<BoundExpression> formerParamExp = obj[..RealParameters.Length].Select(x => (BoundExpression)new BoundLiteralExpression(x)).ToList();
            List<BoundExpression> latterParamExp = obj[RealParameters.Length..].Select(x => (BoundExpression)new BoundLiteralExpression(x)).ToList();
            BoundFunctionCallExpression expression = new(new BoundFunctionCallExpression(functionExp, formerParamExp), latterParamExp);

            evaluator.CurrentCallStack = new(evaluator.CurrentCallStack, expression);
        }
        protected abstract void InvokeMust(NeoEvaluator evaluator, RavelObject[] obj);
    }
}
