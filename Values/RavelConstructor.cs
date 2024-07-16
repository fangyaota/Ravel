using System.Text;

namespace Ravel.Values
{
    public class RavelConstructor
    {
        public RavelConstructor(RavelFunction function, RavelScope scope, string name)
        {
            Function = function;
            Scope = scope;
            Name = name;
        }
        public RavelFunction Function { get; internal set; }
        public RavelScope Scope { get; }
        public string Name { get; }

        public RavelTypePool TypePool => Function.TypePool;

        public RavelType GetRavelType(params RavelType[] genericTypes)
        {
            var name = GetSpecificName(genericTypes);
            if (!TypePool.TypeMap.TryGetValue(name, out var type))
            {
                type = Function.Invoke(genericTypes.Select(x => RavelObject.GetType(x)).ToArray()).GetValue<RavelType>();
            }
            return type;
        }
        public string GetSpecificName(params RavelType[] genericTypes)
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
