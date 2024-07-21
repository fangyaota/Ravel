using System.Text;

namespace Ravel.Values
{

    public sealed class RavelRealConstructor : RavelConstructor
    {
        public RavelRealConstructor(RavelRealFunction function, RavelScope scope, string name)
            : base(function, scope, name)
        {
            RealFunction = function;
        }

        private RavelRealFunction RealFunction { get; }

        public override RavelType GetRavelType(NeoEvaluator evaluator, params RavelType[] genericTypes)
        {
            string name = GetSpecificName(genericTypes);
            if (!TypePool.TypeMap.TryGetValue(name, out RavelType? type))
            {
                type = RealFunction.InvokeReal(evaluator, genericTypes.Select(TypePool.TypeType.GetRavelObject).ToArray()).GetValue<RavelType>();
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
