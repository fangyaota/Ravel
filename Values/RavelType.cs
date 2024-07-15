using Ravel.Syntax;

using System.Security.AccessControl;
using System.Text;

namespace Ravel.Values
{
    public delegate RavelObject RawConstructor(object? raw);
    public class RavelType
    {
        public RavelTypePool TypePool { get; }
        public string Name { get; }
        public RavelType Parent { get; }
        public List<RavelBinaryOperator> BinaryOperators { get; } = new();
        public List<RavelUnaryOperator> UnaryOperators { get; } = new();
        public RavelScope SonVariables { get; }

        public RavelConstructor? TypeConstructor { get; }
        public RavelType[] GenericTypes { get; }

        public RawConstructor? RawConstructor { get; set; }
        public RavelType(string name, RavelTypePool typePool)
        {
            Name = name;
            TypePool = typePool;
            Parent = this;
            TypePool.TypeMap[Name] = this;
            GenericTypes = Array.Empty<RavelType>();
            SonVariables = new();
        }
        public RavelType(string name, RavelType parent)
        {
            Name = name;
            TypePool = parent.TypePool;
            Parent = parent.TypePool.ObjectType;
            TypePool.TypeMap[Name] = this;
            GenericTypes = Array.Empty<RavelType>();
            SonVariables = new();
        }
        public RavelType(RavelConstructor typeConstructor, RavelType parent, RavelType[] genericTypes, RavelScope temp_scope)
        {
            var name = GetTypeName(typeConstructor, genericTypes);
            Name = name;
            TypePool = parent.TypePool;
            Parent = parent;
            TypePool.TypeMap[Name] = this;
            GenericTypes = genericTypes;
            SonVariables = temp_scope;
            TypeConstructor = typeConstructor;
        }

        public static string GetTypeName(RavelConstructor typeConstructor, RavelType[] genericTypes)
        {
            //if (typeGenerator.IsFunction)
            //{
            //    return string.Join("->", genericTypes.Append(typeGenerator));
            //}
            return $"({typeConstructor.Name} {string.Join(" ", genericTypes.Select(x => x.Name))})";
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
            if (IsFunction)
            {
                return $"{Parameter}->{ReturnType}";
            }
            return $"{Name}";
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
        public bool TryGetSonVariable(string name, out RavelVariable? variable)
        {
            return SonVariables.TryGetVariable(name, out variable);
        }
        public RavelObject RawObject(object? from = null)
        {
            return RawConstructor == null ? throw new InvalidOperationException() : RawConstructor(from);
        }
        public bool IsGenericFrom(RavelConstructor constructor)
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

        public RavelType ReturnType => GenericTypes[1];

        public RavelType Parameter => GenericTypes[0];
    }
}
