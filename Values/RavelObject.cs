using System.Numerics;

namespace Ravel.Values
{
    public readonly struct RavelObject
    {
        private object Value { get; }
        private RavelTypePool TypePool { get; }
        private Dictionary<string, RavelObject> SonValues { get; } = new();
        public RavelType Type { get; }

        internal RavelObject(object value, RavelType type, RavelTypePool typePool)
        {
            Value = value;
            Type = type;
            TypePool = typePool;
        }
        internal static RavelObject GetVoid(RavelTypePool typePool)
        {
            return new(null!, typePool.VoidType, typePool);
        }
        public static RavelObject GetInteger(BigInteger integer, RavelTypePool typePool)
        {
            return new RavelObject(integer, typePool.IntType, typePool);
        }
        internal static RavelObject GetBoolean(bool boolean, RavelTypePool typePool)
        {
            return new RavelObject(boolean, typePool.BoolType, typePool);
        }
        public static RavelObject GetString(string str, RavelTypePool typePool)
        {
            return new RavelObject(str, typePool.StringType, typePool);
        }
        public static RavelObject GetFunction(RavelFunction value)
        {
            return new(value, value.Type, value.TypePool);//?
        }
        public static RavelObject GetType(RavelType type)
        {
            return new(type, type.TypePool.TypeType, type.TypePool);
        }
        public static RavelObject GetRawObject(object obj, RavelTypePool typePool)
        {
            return new(obj, typePool.GetMappedType(obj.GetType()), typePool);
        }
        public T GetValue<T>()
        {
            if (Value is T)
            {
                return (T)Value;
            }
            throw new InvalidCastException($"cast from {Type} to {typeof(T).Name}");
        }
        public bool TryGetSonValue(string name, out RavelObject obj)
        {
            
            if (SonValues.TryGetValue(name, out RavelObject value))
            {
                obj = value;
                return true;
            }
            if (!Type.TryGetSonVariable(name, out RavelVariable? variable))
            {
                obj = TypePool.Unit;
                return false;
            }
            obj = variable!.Object;
            if (variable!.IsFunctionSelf)
            {
                obj = obj.Call(this);
            }
            return true;
        }
        public bool TrySetSonValue(string name, in RavelObject obj)
        {
            if (Type.TryGetSonVariable(name, out RavelVariable? variable))
            {
                SonValues![name] = obj;
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            if (Type == TypePool.VoidType)
            {
                return "{}";
            }
            if (Type == TypePool.IntType)
            {
                return Value.ToString()!;
            }
            if (Type == TypePool.BoolType)
            {
                return Value.ToString()!.ToLower();
            }
            if (Type == TypePool.StringType)
            {
                return Value.ToString()!;
            }
            if (Type == TypePool.TypeType)
            {
                return Value.ToString()!;
            }
            return $"Instance of {Type}";
        }
        public bool Equals(RavelObject obj)
        {
            return this == obj;
        }
        public static bool operator ==(RavelObject left, RavelObject right)
        {
            if (left.Type != right.Type)
            {
                return false;
            }
            return left.Value == right.Value;
        }
        public static bool operator !=(RavelObject left, RavelObject right)
        {
            return !(left == right);
        }
        public RavelObject Call(params RavelObject[] obj)
        {
            return !Type.IsFunction ? throw new InvalidCastException() : GetValue<RavelFunction>().Invoke(obj);
        }
        public bool TryCallFunction(string funcName, out RavelObject result, params RavelObject[] obj)
        {
            if (TryGetSonValue(funcName, out RavelObject function))
            {
                result = function.Call(obj);
                return true;
            }
            result = TypePool.Unit;
            return false;
        }
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is RavelObject r)
            {
                return Equals(r);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
