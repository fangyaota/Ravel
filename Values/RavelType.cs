using Ravel.Syntax;

using System.Xml.Linq;

namespace Ravel.Values
{
    public delegate RavelObject RawConstructor(object? raw);
    public class RavelType
    {
        public RavelTypePool TypePool { get; }
        public string Name { get; }
        public RavelType Parent { get; }
        private int ParentDepth { get; }
        public List<RavelBinaryOperator> BinaryOperators { get; } = new();
        public List<RavelUnaryOperator> UnaryOperators { get; } = new();
        public List<RavelImplictConverter> ImplictConverters { get; } = new();
        public RavelScope SonVariables { get; }

        public RavelRealConstructor? TypeConstructor { get; }
        public RavelType[] GenericTypes { get; }


        public static RavelType GetRavelType(string name, RavelTypePool typePool)
        {
            if(typePool.TypeMap.TryGetValue(name, out var type))
            {
                return type;
            }
            RavelType t = new(name, typePool);
            typePool.TypeMap[name] = t;
            return t;
        }
        public static RavelType GetRavelType(string name, RavelType parent)
        {
            if (parent.TypePool.TypeMap.TryGetValue(name, out var type))
            {
                if(type.Parent != parent)
                {
                    throw new InvalidOperationException(name);
                }
                return type;
            }
            RavelType t = new(name, parent);
            parent.TypePool.TypeMap[name] = t;
            return t;
        }
        public static RavelType GetRavelType(RavelRealConstructor typeConstructor, RavelType parent, RavelType[] genericTypes)
        {
            string name = typeConstructor.GetSpecificName(genericTypes);
            if (parent.TypePool.TypeMap.TryGetValue(name, out var type))
            {
                if (type.Parent != parent || type.TypeConstructor != typeConstructor)
                {
                    throw new InvalidOperationException(name);
                }
                return type;
            }
            RavelType t = new(typeConstructor, parent, genericTypes, new(parent.SonVariables));
            parent.TypePool.TypeMap[t.Name] = t;
            return t;
        }
        private RavelType(string name, RavelTypePool typePool)
        {
            Name = name;
            TypePool = typePool;
            Parent = this;
            ParentDepth = 0;
            GenericTypes = Array.Empty<RavelType>();
            SonVariables = new();
        }

        private RavelType(string name, RavelType parent)
        {
            Name = name;
            TypePool = parent.TypePool;
            Parent = parent;
            ParentDepth = parent.ParentDepth + 1;
            GenericTypes = Array.Empty<RavelType>();
            SonVariables = new(parent.SonVariables);
        }
        private RavelType(RavelRealConstructor typeConstructor, RavelType parent, RavelType[] genericTypes, RavelScope temp_scope)
        {
            string name = typeConstructor.GetSpecificName(genericTypes);
            Name = name;
            TypePool = parent.TypePool;
            Parent = parent;
            ParentDepth = parent.ParentDepth + 1;
            GenericTypes = genericTypes;
            SonVariables = temp_scope;
            TypeConstructor = typeConstructor;
        }
        public bool IsSonOrEqual(RavelType other)
        {
            if (this == other)
            {
                return true;
            }
            if (this == Parent)
            {
                return false;
            }
            return Parent.IsSonOrEqual(other);
        }
        public override string ToString()
        {
            return Name;
        }

        public RavelBinaryOperator? GetOperator(SyntaxKind kind, RavelType right)
        {
            foreach (RavelBinaryOperator i in BinaryOperators)
            {
                if (right.IsSonOrEqual(i.RightType) && i.SyntaxKind == kind)
                {
                    return i;
                }
            }
            if (this == Parent)
            {
                return null;
            }
            return Parent.GetOperator(kind, right);
        }
        public RavelUnaryOperator? GetOperator(SyntaxKind kind)
        {
            foreach (RavelUnaryOperator i in UnaryOperators)
            {
                if (i.SyntaxKind == kind)
                {
                    return i;
                }
            }
            if (this == Parent)
            {
                return null;
            }
            return Parent.GetOperator(kind);
        }
        public RavelImplictConverter? GetImplictConverter(RavelType to)
        {
            foreach(var i in ImplictConverters)
            {
                if(i.To == to)
                {
                    return i;
                }
            }
            if(this == Parent)
            {
                return null;
            }
            return Parent.GetImplictConverter(to);
        }
        public bool TryGetSonVariable(string name, out RavelVariable? variable)
        {
            return SonVariables.TryGetVariable(name, out variable);
        }
        public RavelObject GetRavelObject(object from)
        {
            return new RavelObject(from, this, TypePool);
        }
        public bool IsGenericFrom(RavelRealConstructor constructor)
        {
            return TypeConstructor == constructor;
        }
        public bool IsFunction
        {
            get
            {
                return TypeConstructor == TypePool.FunctionConstructor || (Parent != this && Parent.IsFunction);
            }
        }

        public RavelType GetFuncParametersIndex(int index)
        {
            if (!IsFunction)
            {
                throw new InvalidOperationException();
            }
            RavelType type = this;
            while (index > 0)
            {
                type = type.ReturnType;
                index--;
            }
            return type.Parameter;
        }
        public IEnumerable<RavelType> GetFuncParameters(int count)
        {
            if (!IsFunction)
            {
                throw new InvalidOperationException();
            }
            RavelType type = this;
            while (count > 0)
            {
                yield return type.Parameter;
                type = type.ReturnType;
                count--;
            }
        }
        public int GetMaxParameters()
        {
            if (!IsFunction)
            {
                throw new InvalidOperationException();
            }
            RavelType ret = this;
            int count = 0;
            while (ret.IsFunction)
            {
                ret = ret.ReturnType;
                count++;
            }
            return count;
        }

        public RavelType GetTypeWhenCall(int count)
        {
            if (!IsFunction)
            {
                throw new InvalidOperationException();
            }
            RavelType type = this;
            while (count > 0)
            {
                type = type.ReturnType;
                count--;
            }
            return type;
        }
        public static RavelType GetCommonParent(RavelType left, RavelType right)
        {
            while (right.ParentDepth > left.ParentDepth)
            {
                right = right.Parent;
            }
            while (left.ParentDepth > right.ParentDepth)
            {
                left = left.Parent;
            }
            while(left != right)
            {
                left = left.Parent;
                right = right.Parent;
            }
            return left;
        }
        public RavelType ReturnType => GenericTypes[1];

        public RavelType Parameter => GenericTypes[0];
    }
}
