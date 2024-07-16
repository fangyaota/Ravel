using Ravel.Syntax;

using System.Numerics;
using System.Text;

using SysType = System.Type;

namespace Ravel.Values
{
    public class RavelTypePool
    {
        public Dictionary<string, RavelType> TypeMap { get; } = new();
        public RavelTypePool()
        {
            SystemScope = new()
            {

            };

            ObjectType = new("object", this);

            TypeType = new("type", ObjectType);

            VoidType = new("void", ObjectType);

            IntType = new("int", ObjectType);

            BoolType = new("bool", ObjectType);

            StringType = new("string", ObjectType);


            EnumerableType = new("enumerable", ObjectType);

            CallableType = new("callable", ObjectType);

            FunctionConstructor = new(null!, SystemScope, "function");

            var TTT = GetFuncType(TypeType, TypeType, TypeType);
            var TT = GetFuncType(TypeType, TypeType);

            FunctionConstructor.Function = new RavelRealFunction(TypePoint, TTT, true);

            ListConstructor = new(new RavelRealFunction(GetListType, TT, true), SystemScope, "list");
            True = RavelObject.GetBoolean(true, this);
            False = RavelObject.GetBoolean(false, this);
            RegistFunctions();

            SystemScope["int"] = new(RavelObject.GetType(IntType), "int", true, true);
            SystemScope["bool"] = new(RavelObject.GetType(BoolType), "bool", true, true);
            SystemScope["string"] = new(RavelObject.GetType(StringType), "string", true, true);
            SystemScope["type"] = new(RavelObject.GetType(TypeType), "type", true, true);
            SystemScope["object"] = new(RavelObject.GetType(ObjectType), "object", true, true);
            SystemScope["void"] = new(RavelObject.GetType(VoidType), "void", true, true);

        }

        private void RegistFunctions()
        {
            RavelType OTB = GetFuncType(BoolType, ObjectType, TypeType);

            RavelType OSS = GetFuncType(StringType, ObjectType, StringType);

            RavelType OOB = GetFuncType(BoolType, ObjectType, ObjectType);

            RavelRealFunction objectIs = new(ObjectIs, OTB, true);

            RavelRealFunction objectEqual = new(ObjEqual, OOB, false);

            RavelRealFunction stringAdd = new(StringAdd, OSS, true);

            ObjectType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new (SyntaxKind.Is, RavelBinaryOperatorKind.TypeIs, objectIs),
                new(SyntaxKind.Plus, RavelBinaryOperatorKind.Addition, stringAdd),
                new(SyntaxKind.EqualEqual, RavelBinaryOperatorKind.EqualComparision, objectEqual),

            });
            RavelType OS = GetFuncType(StringType, ObjectType);
            ObjectType.SonVariables["ToString"] = new(RavelObject.GetFunction(new RavelRealFunction(ObjGetString, OS, true)), "ToString", true, true, true);



            RavelType III = GetFuncType(IntType, IntType, IntType);
            RavelType IIB = GetFuncType(BoolType, IntType, IntType);
            IntType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new(SyntaxKind.Plus, RavelBinaryOperatorKind.Addition, new RavelRealFunction(IntAdd, III, true)),
                new(SyntaxKind.Minus, RavelBinaryOperatorKind.Subtraction, new RavelRealFunction(IntSub, III, true)),
                new(SyntaxKind.Star, RavelBinaryOperatorKind.Multiplication, new RavelRealFunction(IntMul, III, true)),
                new(SyntaxKind.Slash, RavelBinaryOperatorKind.Division, new RavelRealFunction(IntDiv, III, true)),
                new(SyntaxKind.Percent, RavelBinaryOperatorKind.Mod, new RavelRealFunction(IntMod, III, true)),
                new(SyntaxKind.StarStar, RavelBinaryOperatorKind.Power, new RavelRealFunction(IntPow, III, true)),
                new(SyntaxKind.Large, RavelBinaryOperatorKind.LargeComparision, new RavelRealFunction(IntLarge, IIB, true)),
                new(SyntaxKind.Small, RavelBinaryOperatorKind.SmallComparision, new RavelRealFunction(IntSmall, IIB, true)),
                new(SyntaxKind.LargeEqual, RavelBinaryOperatorKind.LargeEqualComparision, new RavelRealFunction(IntLargeEqual, IIB, true)),
                new(SyntaxKind.SmallEqual, RavelBinaryOperatorKind.SmallEqualComparision, new RavelRealFunction(IntSmallEqual, IIB, true)),
                new(SyntaxKind.EqualEqual, RavelBinaryOperatorKind.EqualComparision, new RavelRealFunction(IntEqual, IIB, true)),
                new(SyntaxKind.NotEqual, RavelBinaryOperatorKind.NotEqualComparision, new RavelRealFunction(IntNotEqual, IIB, true)),
            });

            RavelType II = GetFuncType(IntType, IntType);
            IntType.UnaryOperators.AddRange(new RavelUnaryOperator[]
            {
                new(SyntaxKind.Minus, RavelUnaryOperatorKind.Negation, new RavelRealFunction(IntNegation, II, true)),
                new(SyntaxKind.Plus, RavelUnaryOperatorKind.Indentity, new RavelRealFunction(IntIdentity, II, true)),
            });
            RavelType BBB = GetFuncType(BoolType, BoolType, BoolType);

            BoolType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new(SyntaxKind.And, RavelBinaryOperatorKind.And, new RavelRealFunction(BoolAnd, BBB, true)),
                new(SyntaxKind.Or, RavelBinaryOperatorKind.Or, new RavelRealFunction(BoolOr, BBB, true)),
                new(SyntaxKind.ShortCutAnd, RavelBinaryOperatorKind.ShortCutAnd, new RavelRealFunction(BoolAnd, BBB, true)),
                new(SyntaxKind.ShortCutOr, RavelBinaryOperatorKind.ShortCutOr, new RavelRealFunction(BoolOr, BBB, true)),
            });
            RavelType BB = GetFuncType(BoolType, BoolType);

            BoolType.UnaryOperators.AddRange(new RavelUnaryOperator[]
            {
                new(SyntaxKind.Not, RavelUnaryOperatorKind.Not, new RavelRealFunction(BoolNot, BB, true)),
            });

            RavelType SOS = GetFuncType(StringType, StringType, ObjectType);

            StringType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new(SyntaxKind.Plus, RavelBinaryOperatorKind.Addition, new RavelRealFunction(StringAdd, SOS, true))
            });

            RavelType TTT = GetFuncType(TypeType, TypeType, TypeType);
            TypeType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new(SyntaxKind.MinusLarge, RavelBinaryOperatorKind.Point, new RavelRealFunction(TypePoint, TTT, true))
            });
            RavelType LS = GetFuncType(StringType, EnumerableType);//?
            EnumerableType.SonVariables["ToString"] = new(RavelObject.GetFunction(new RavelRealFunction(ListToString, LS, true)), "ToString", true, true, true);
        }
        

        
        public RavelType VoidType { get; }
        public RavelType IntType { get; }
        public RavelType BoolType { get; }
        public RavelType StringType { get; }
        public RavelType TypeType { get; }
        public RavelConstructor FunctionConstructor { get; }
        public RavelConstructor ListConstructor { get; }
        public RavelType EnumerableType { get; }
        public RavelType CallableType { get; }
        public RavelType ObjectType { get; }
        public RavelObject Unit { get; }
        public RavelObject True { get; }
        public RavelObject False { get; }
        public RavelScope SystemScope { get; }

        public RavelObject GetRawObject(RavelType type, object? raw)
        {
            return new(raw!, type, this);
        }
        public RavelType GetFuncType(RavelType returnType, params RavelType[] types)
        {
            types = types.Append(returnType).ToArray();
            RavelType currentType = types[^1];
            string name = types[^1].ToString();

            for(int index = types.Length - 1; index >= 0; index--)
            {
                if (TypeMap.TryGetValue(name, out var val))
                {
                    currentType = val;
                }
                else
                {
                    currentType = TypePoolGetFunctionType(types[index], currentType);
                }
                if (index - 1 >= 0)
                {
                    name = FunctionConstructor.GetSpecificName(currentType, types[index - 1]);
                }
            }
            return currentType;
        }
        private RavelType TypePoolGetFunctionType(RavelType f, RavelType s)
        {
            return new RavelType(FunctionConstructor, CallableType, new RavelType[] { f, s }, new());
        }

        private RavelObject GetListType(RavelObject first)
        {
            var f = first.GetValue<RavelType>();
            var func = new RavelType(ListConstructor, EnumerableType, new RavelType[] { f }, new());
            return RavelObject.GetType(func);
        }
        public RavelType GetMappedType(SysType type)
        {
            if (type == typeof(BigInteger))
            {
                return IntType;
            }
            else if (type == typeof(bool))
            {
                return BoolType;
            }
            else if (type == typeof(string))
            {
                return StringType;
            }
            else if (type == typeof(RavelType))
            {
                return TypeType;
            }
            throw new NotImplementedException();
        }
        internal RavelObject ObjEqual(RavelObject left, RavelObject right)
        {
            object l = left.GetValue<object>();
            object r = right.GetValue<object>();
            return l == r ? True : False;
        }
        internal RavelObject ObjGetString(RavelObject arg)
        {
            return RavelObject.GetString(arg.ToString(), this);
        }
        internal RavelObject IntAdd(RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return RavelObject.GetInteger(l + r, this);
        }
        internal RavelObject IntSub(RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return RavelObject.GetInteger(l - r, this);
        }
        internal RavelObject IntMul(RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return RavelObject.GetInteger(l * r, this);
        }
        internal RavelObject IntDiv(RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return RavelObject.GetInteger(l / r, this);
        }
        internal RavelObject IntMod(RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return RavelObject.GetInteger(l % r, this);
        }
        internal RavelObject IntPow(RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return RavelObject.GetInteger(BigInteger.Pow(l, (int)r), this);
        }
        internal RavelObject IntLarge(RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return l > r ? True : False;
        }
        internal RavelObject IntLargeEqual(RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return l >= r ? True : False;
        }
        internal RavelObject IntSmall(RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return l < r ? True : False;
        }
        internal RavelObject IntSmallEqual(RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return l <= r ? True : False;
        }
        internal RavelObject IntEqual(RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return l == r ? True : False;
        }
        internal RavelObject IntNotEqual(RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return l != r ? True : False;
        }
        internal RavelObject BoolAnd(RavelObject left, RavelObject right)
        {
            bool l = left.GetValue<bool>();
            bool r = right.GetValue<bool>();
            return l & r ? True : False;
        }
        internal RavelObject BoolOr(RavelObject left, RavelObject right)
        {
            bool l = left.GetValue<bool>();
            bool r = right.GetValue<bool>();
            return l | r ? True : False;
        }
        internal RavelObject IntIdentity(RavelObject obj)
        {
            return obj;
        }
        internal RavelObject IntNegation(RavelObject obj)
        {
            return RavelObject.GetInteger(-obj.GetValue<BigInteger>(), this);
        }
        internal RavelObject BoolNot(RavelObject obj)
        {
            return obj.GetValue<bool>() ? False : True;
        }
        internal RavelObject ObjectIs(RavelObject left, RavelObject right)
        {
            return left.Type.IsSonOrEqual(right.GetValue<RavelType>()) ? True : False;
        }
        internal RavelObject ObjectTypeOf(RavelObject operand)
        {
            return RavelObject.GetType(operand.Type);
        }
        internal RavelObject StringAdd(RavelObject left, RavelObject right)
        {
            string l = left.ToString();
            string r = right.ToString();
            return RavelObject.GetString(l + r, this);
        }
        internal RavelObject TypePoint(RavelObject first, RavelObject second)
        {
            var f = first.GetValue<RavelType>();
            var s = second.GetValue<RavelType>();
            RavelType func = GetFuncType(f, s);
            return RavelObject.GetType(func);
        }
        internal RavelObject ListToString(RavelObject list)
        {
            StringBuilder builder = new();
            builder.Append('[');
            builder.AppendJoin(' ', list.GetValue<List<RavelObject>>());
            builder.Append(']');
            return RavelObject.GetString(builder.ToString(), this);
        }
    }
}
