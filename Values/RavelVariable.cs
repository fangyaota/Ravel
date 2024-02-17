namespace Ravel.Values
{
    public sealed class RavelVariable : lDeclare
    {
        private RavelObject ravelObject;
        public RavelObject Object
        {
            get
            {
                return ravelObject;
            }
            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException();
                }
                ravelObject = value;
            }
        }
        public RavelType Type => IsFunctionSelf ? ravelObject.Type.ReturnType : ravelObject.Type;
        public string Name { get; }
        public bool IsReadOnly { get; }
        public bool IsConst { get; }
        public bool IsDynamic { get; }
        public VariableMode Mode { get; }
        public bool IsFunctionSelf { get; }

        public RavelVariable(RavelObject ravelObject, string name, bool isConst, bool isReadOnly, bool isDynamic = false, bool functionSelf = false, VariableMode variable = VariableMode.Public)
        {
            this.ravelObject = ravelObject;
            Name = name;
            IsConst = isConst;
            IsReadOnly = isReadOnly;
            IsDynamic = isDynamic;
            IsFunctionSelf = functionSelf;
            Mode = variable;
        }
    }
}
