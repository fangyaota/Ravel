namespace Ravel.Values
{
    public class RavelRealFunction : RavelFunction
    {
        public RavelRealFunction(Func<NeoEvaluator, RavelObject, RavelObject> func, RavelType type, bool isConst)
            : this((Delegate)func, type, isConst)
        {
        }
        public RavelRealFunction(Func<NeoEvaluator, RavelObject, RavelObject, RavelObject> func, RavelType type, bool isConst)
            : this((Delegate)func, type, isConst)
        {
        }
        private RavelRealFunction(Delegate func, RavelType type, bool isConst)
            : base(type, func.Method.GetParameters().Length - 1)
        {
            Func = func;
            IsConst = isConst;
        }

        public override bool IsConst { get; }
        private Delegate Func { get; }

        protected override void InvokeMust(NeoEvaluator evaluator, RavelObject[] objs)
        {
            evaluator.CurrentCallStack.SonResults.Add(InvokeReal(evaluator, objs));
        }
        public RavelObject InvokeReal(NeoEvaluator evaluator, params RavelObject[] objs)
        {
            return (RavelObject)Func.DynamicInvoke(objs.Cast<object>().Prepend(evaluator).ToArray())!;
        }
    }
}
