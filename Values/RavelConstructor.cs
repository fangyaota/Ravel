namespace Ravel.Values
{
    public abstract class RavelConstructor
    {
        protected RavelConstructor(RavelFunction function, RavelScope scope, string name)
        {
            Function = function;
            Name = name;
            Scope = scope;
        }

        public RavelFunction Function { get; internal set; }
        public string Name { get; }
        public RavelScope Scope { get; }

        public RavelTypePool TypePool => Function.TypePool;

        public abstract RavelType GetRavelType(NeoEvaluator evaluator, params RavelType[] genericTypes);
        public abstract string GetSpecificName(params RavelType[] genericTypes);
    }
}
