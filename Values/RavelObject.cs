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
        public T GetValue<T>()
        {
            return Value is T v ? v : throw new InvalidCastException($"cast from {Type} to {typeof(T).Name}");
        }
        public bool TryReturnSonValue(NeoEvaluator evaluator, string name)
        {

            if (SonValues.TryGetValue(name, out RavelObject value))
            {
                evaluator.AddResult(value);
                return true;
            }
            if (!Type.TryGetSonVariable(name, out RavelVariable? variable))
            {
                return false;
            }
            RavelObject obj = variable!.Object;
            if (variable!.IsFunctionSelf)
            {
                obj.Call(evaluator, this);
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
        public void Call(NeoEvaluator evaluator, params RavelObject[] obj)
        {
            if (!Type.IsFunction)
            {
                throw new InvalidCastException();
            }
            else
            {
                GetValue<RavelFunction>().Invoke(evaluator, obj);
            }
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
