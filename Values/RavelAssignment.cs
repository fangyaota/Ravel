namespace Ravel.Values
{
    public sealed class RavelDefining : lDeclare
    {
        public RavelType Type { get; }

        public string Name { get; }

        public bool IsReadOnly { get; }

        public bool IsConst { get; }


        public RavelDefining(RavelType type, string name, bool isReadOnly, bool isConst)
        {
            Type = type;
            Name = name;
            IsReadOnly = isReadOnly;
            IsConst = isConst;
        }
    }
}
