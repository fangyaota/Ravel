using Ravel.Binding;

using System.Collections;

namespace Ravel.Values
{
    public sealed class RavelScope : IScope, IEnumerable<RavelVariable>
    {
        private RavelScope? parent;
        private Dictionary<string, RavelVariable> Variables { get; } = new();
        public RavelScope(RavelScope? parent = null)
        {
            this.parent = parent;
        }
        public RavelVariable this[string name]
        {
            get
            {
                if (!TryGetVariable(name, out RavelVariable? v))
                {
                    throw new InvalidOperationException();
                }
                return v;
            }
        }
        public void Add(RavelVariable variable)
        {
            Variables.Add(variable.Name, variable);
        }
        public bool TryDeclare(string name, RavelObject obj, bool isReadOnly, bool isConst, bool functionSelf = false, VariableMode mode = VariableMode.Public)
        {
            if (Variables.ContainsKey(name))
            {
                return false;
            }
            Add(new RavelVariable(obj, name, isReadOnly, isConst, functionSelf, mode));
            return true;
        }
        public bool TryGetVariable(string name, out RavelVariable variable, VariableMode mode = VariableMode.Public)
        {
            if (Variables.TryGetValue(name, out variable!))
            {
                return variable.Mode <= mode;
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
            bool b = TryGetVariable(name, out RavelVariable? variable);
            value = variable;
            return b;
        }

        public BoundScope ToBound()
        {
            BoundScope bound = new BoundScope(parent);
            foreach (KeyValuePair<string, RavelVariable> variable in Variables)
            {
                bound[variable.Key] = variable.Value;
            }
            return bound;
        }

        public IEnumerator<RavelVariable> GetEnumerator()
        {
            if(parent == null)
            {
                return Variables.Values.GetEnumerator();
            }
            return Variables.Values.Concat(parent).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
