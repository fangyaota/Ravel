namespace Ravel.Values
{
    public interface IScope
    {
        public bool Contain(string name);
        public bool TryGetVariable(string name, out lDeclare value);
    }
}
