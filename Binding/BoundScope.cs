using Ravel.Values;

namespace Ravel.Binding
{
    public sealed class BoundScope : IScope
    {
        private IScope? parent;
        private Dictionary<string, lDeclare> Variables { get; } = new();
        public BoundScope(IScope? parent = null)
        {
            this.parent = parent;
        }
        public lDeclare this[string name]
        {
            get
            {
                if (!TryGetVariable(name, out lDeclare? v))
                {
                    throw new InvalidOperationException();
                }
                return v;
            }
            set
            {
                Variables[name] = value;
            }
        }
        public bool TryDeclare(string name, RavelType type, bool isReadOnly, bool isConst)
        {
            if (Variables.ContainsKey(name))
            {
                return false;
            }
            Variables[name] = new RavelDefining(type, name, isReadOnly, isConst);
            return true;
        }
        public bool TryGetVariable(string name, out lDeclare variable)
        {
            if (Variables.TryGetValue(name, out variable!))
            {
                return true;
            }
            if (parent == null)
            {
                return false;
            }
            return parent.TryGetVariable(name, out variable);
        }

        bool IScope.Contain(string name)
        {
            return Variables.ContainsKey(name);
        }

        bool IScope.TryGetVariable(string name, out lDeclare value)
        {
            return TryGetVariable(name, out value);
        }
    }
}
