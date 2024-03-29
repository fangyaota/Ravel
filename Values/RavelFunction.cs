﻿namespace Ravel.Values
{
    public abstract class RavelFunction
    {
        public RavelType[] RealParameters { get; }
        public RavelType ResultType { get; }
        public RavelType Type { get; }

        public RavelTypePool TypePool { get; }
        public abstract bool IsConst { get; }

        public RavelFunction(RavelType type, RavelTypePool typePool, int realParameterCount)
        {
            Type = type;
            TypePool = typePool;
            RealParameters = type.GetFuncParameters(realParameterCount).ToArray();
            ResultType = type.GetTypeWhenCall(realParameterCount);
        }

        public RavelObject Invoke(params RavelObject[] obj)
        {
            if (obj.Length < RealParameters.Length)
            {
                RavelType newType = TypePool.GetFuncType(ResultType, RealParameters[obj.Length..]);
                return RavelObject.GetFunction(new RavelParcialFunction(this, obj, newType, TypePool), TypePool);
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
}
