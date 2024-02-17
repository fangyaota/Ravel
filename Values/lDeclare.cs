namespace Ravel.Values
{
    public interface lDeclare
    {
        public RavelType Type { get; }
        public string Name { get; }
        public bool IsReadOnly { get; }
        public bool IsConst { get; }
        public bool IsDynamic { get; }
    }
}
