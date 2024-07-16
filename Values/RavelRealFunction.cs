namespace Ravel.Values
{
    public class RavelRealFunction : RavelFunction
    {
        public RavelRealFunction(Func<RavelObject, RavelObject> func, RavelType type, bool isConst)
            : this((Delegate)func, type, isConst)
        {
        }
        public RavelRealFunction(Func<RavelObject, RavelObject, RavelObject> func, RavelType type, bool isConst)
            : this((Delegate)func, type, isConst)
        {
        }
        public RavelRealFunction(Delegate func, RavelType type, bool isConst)
            : base(type, func.Method.GetParameters().Length)
        {
            Func = func;
            IsConst = isConst;
        }

        public override bool IsConst { get; }
        private Delegate Func { get; }

        protected override void InvokeMust(NeoEvaluator evaluator, RavelObject[] objs)
        {
            evaluator.CurrentCallStack.SonResults.Add(Invoke(objs));
        }
        public RavelObject Invoke(params RavelObject[] objs)
        {
            return (RavelObject)Func.DynamicInvoke(objs.Cast<object>().ToArray())!;
        }
    }
}
