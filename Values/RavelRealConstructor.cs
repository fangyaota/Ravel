using System.Text;

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

    public class RavelRealConstructor : RavelConstructor
    {
        public RavelRealConstructor(RavelRealFunction function, RavelScope scope, string name)
            : base(function, scope, name)
        {
            RealFunction = function;
        }

        private RavelRealFunction RealFunction { get; }

        public override RavelType GetRavelType(NeoEvaluator evaluator, params RavelType[] genericTypes)
        {
            var name = GetSpecificName(genericTypes);
            if (!TypePool.TypeMap.TryGetValue(name, out var type))
            {
                type = RealFunction.Invoke(genericTypes.Select(RavelObject.GetType).ToArray()).GetValue<RavelType>();
            }
            return type;
        }
        public override string GetSpecificName(params RavelType[] genericTypes)
        {
            StringBuilder sb = new();
            sb.Append('(');
            sb.Append(Name);
            sb.Append(' ');
            sb.AppendJoin(' ', (IEnumerable<RavelType>)genericTypes);
            sb.Append(')');
            return sb.ToString();
        }
    }
}
