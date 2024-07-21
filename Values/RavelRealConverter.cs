namespace Ravel.Values
{
    public sealed class RavelRealConverter : RavelImplictConverter
    {
        public RavelRealConverter(RavelRealFunction function) : base(function)
        {
            RealFunction = function;
        }

        public RavelRealFunction RealFunction { get; }

        public RavelObject InvokeReal(NeoEvaluator evaluator, RavelObject obj)
        {
            return RealFunction.InvokeReal(evaluator, obj);
        }
    }
}
