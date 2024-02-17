namespace Ravel.Values
{
    public class RavelRealFunction : RavelFunction
    {
        public RavelRealFunction(Func<RavelObject, RavelObject> func, RavelType type, RavelTypePool typePool, bool isConst)
            : this((Delegate)func, type, typePool, isConst)
        {
        }
        public RavelRealFunction(Func<RavelObject, RavelObject, RavelObject> func, RavelType type, RavelTypePool typePool, bool isConst)
            : this((Delegate)func, type, typePool, isConst)
        {
        }
        public RavelRealFunction(Delegate func, RavelType type, RavelTypePool typePool, bool isConst)
            : base(type, typePool, func.Method.GetParameters().Length)
        {
            Func = func;
            IsConst= isConst;
        }

        public override bool IsConst { get; }

        private Delegate Func { get; }

        protected override RavelObject InvokeMust(RavelObject[] obj)
        {
            return (RavelObject)Func.DynamicInvoke(obj.Cast<object>().ToArray())!;
        }
    }
}
