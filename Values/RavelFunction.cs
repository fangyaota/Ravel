namespace Ravel.Values
{
    public abstract class RavelFunction
    {
        public RavelType[] RealParameters { get; }
        public RavelType ResultType { get; }
        public RavelType Type { get; }

        public RavelTypePool TypePool => Type.TypePool;
        public abstract bool IsConst { get; }

        public RavelFunction(RavelType type, int realParameterCount)
        {
            Type = type;
            RealParameters = type.GetFuncParameters(realParameterCount).ToArray();
            ResultType = type.GetTypeWhenCall(realParameterCount);
        }

        public RavelObject Invoke(params RavelObject[] obj)
        {
            if (obj.Length < RealParameters.Length)
            {
                RavelType newType = TypePool.GetFuncType(ResultType, RealParameters[obj.Length..]);
                return RavelObject.GetFunction(new RavelParcialFunction(this, obj, newType, TypePool));
            }
            if (obj.Length == RealParameters.Length)
            {
                return InvokeMust(obj);
            }
            RavelObject ret = InvokeMust(obj[..RealParameters.Length]);
            return ret.Call(obj[RealParameters.Length..]);
        }
        protected abstract RavelObject InvokeMust(RavelObject[] obj);
    }

    public class RavelConstructor
    {
        public RavelConstructor(RavelFunction function, RavelScope scope, string name)
        {
            Function = function;
            Scope = scope;
            Name = name;
        }
        public RavelFunction Function { get; }
        public RavelScope Scope { get; }
        public string Name { get; }

        public RavelTypePool TypePool => Function.TypePool;

        public RavelType GetRavelType(params RavelType[] genericTypes)
        {
            var name = RavelType.GetTypeName(this, genericTypes);
            if (!TypePool.TypeMap.TryGetValue(name, out var type))
            {
                type = Function.Invoke(genericTypes.Select(x => RavelObject.GetType(x)).ToArray()).GetValue<RavelType>();
            }
            return type;
        }
    }
}
