namespace Ravel.Values
{
    public abstract class RavelImplictConverter
    {
        protected RavelImplictConverter(RavelFunction function)
        {
            Function = function;
            From = Function.Type.Parameter;
            To = Function.Type.ReturnType;
        }

        public RavelFunction Function { get; }
        public RavelType From { get; }
        public RavelType To { get; }
        public void Invoke(NeoEvaluator evaluator, RavelObject obj)
        {
            Function.Invoke(evaluator, obj);
        }
    }
}
