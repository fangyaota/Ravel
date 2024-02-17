namespace Ravel.Values
{
    public sealed class RavelDefining : lDeclare
    {
        public RavelType Type { get; }

        public string Name { get; }

        public bool IsReadOnly { get; }

        public bool IsConst { get; }

        public bool IsDynamic { get; }

        public RavelDefining(RavelType type, string name, bool isReadOnly, bool isConst, bool isDynamic = false)
        {
            Type = type;
            Name = name;
            IsReadOnly = isReadOnly;
            IsConst = isConst;
            IsDynamic = isDynamic;
        }
    }
}
