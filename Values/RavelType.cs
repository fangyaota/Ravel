using Ravel.Syntax;

using System.Security.AccessControl;
using System.Text;

namespace Ravel.Values
{
    public delegate RavelObject NewGive(RavelType type, object? obj);
    public class RavelType
    {
        public RavelTypePool TypePool { get; }
        public string Name { get; }
        public RavelType Parent { get; }
        public List<RavelBinaryOperator> BinaryOperators { get; } = new();
        public List<RavelUnaryOperator> UnaryOperators { get; } = new();
        public RavelScope SonVariables { get; }
        public RavelType? BaseType { get; }
        public RavelType[] GenericTypes { get; }
        public int GenericTypesCount { get; }
        public bool IsCompleted { get; }
        private NewGive NewGiver { get; }

        internal RavelType(string name, RavelTypePool typePool)
        {
            Name = name;
            TypePool = typePool;
            Parent = this;
            TypePool.TypeMap[Name] = this;
            GenericTypes = Array.Empty<RavelType>();
            GenericTypesCount = 0;
            IsCompleted = false;
            NewGiver = null!;
            SonVariables = new();
        }
        internal RavelType(string name, RavelType baseType, RavelTypePool typePool, params RavelType[] genericTypes)
        {
            Name = name;
            BaseType = baseType;
            TypePool = typePool;
            Parent = typePool.ObjectType;
            TypePool.TypeMap[Name] = this;
            GenericTypes = genericTypes;
            GenericTypesCount = genericTypes.Length;
            IsCompleted = true;
            NewGiver = baseType.NewGiver;
            SonVariables = new(baseType.SonVariables);//?
        }


        internal RavelType(string name, RavelTypePool typePool, RavelType parent, int genericTypesCount, NewGive newGiver)
        {
            Name = name;
            Parent = parent;
            TypePool = typePool;
            TypePool.TypeMap[Name] = this;
            GenericTypes = Array.Empty<RavelType>();
            GenericTypesCount = genericTypesCount;
            IsCompleted = false;
            NewGiver = newGiver;
            SonVariables = new(parent.SonVariables);
        }
        internal RavelType(string name, RavelTypePool typePool, RavelType parent, NewGive newGiver)
        {
            Name = name;
            Parent = parent;
            TypePool = typePool;
            GenericTypes = Array.Empty<RavelType>();
            TypePool.TypeMap[Name] = this;
            GenericTypesCount = 0;
            IsCompleted = true;
            NewGiver = newGiver;
            SonVariables = new(parent.SonVariables);
        }
        public static string GetTypeName(RavelType baseType, RavelType[] genericTypes)
        {
            if (baseType.IsFunction)
            {
                return string.Join("->", genericTypes.Append(baseType));
            }
            return $"({baseType.Name} {string.Join(" ", genericTypes.Select(x => x.Name))})";
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
        public RavelObject NewObject(object? from = null)
        {
            if (!IsCompleted)
            {
                throw new InvalidOperationException();
            }
            return NewGiver(this, from);
        }
        public bool IsGenericFrom(RavelType baseType)
        {
            return BaseType == baseType;
        }
        public bool IsFunction
        {
            get
            {
                if (BaseType == null)
                {
                    return false;
                }
                return BaseType.IsSonOrEqual(TypePool.BaseFuncType);
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
