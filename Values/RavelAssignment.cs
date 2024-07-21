namespace Ravel.Values
{
    public sealed class RavelDefining : lDeclare
    {
        public RavelType Type { get; }

        public string Name { get; }

        public bool IsReadOnly { get; }

        public RavelDefining(RavelType type, string name, bool isReadOnly)
        {
            Type = type;
            Name = name;
            IsReadOnly = isReadOnly;
        }
    }
}
