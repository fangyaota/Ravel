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
            ObjectType = new("object", this);

            VoidType = new("void", this, ObjectType, GetRawObject);

            IntType = new("int", this, ObjectType, GetRawObject);

            BoolType = new("bool", this, ObjectType, GetRawObject);

            StringType = new("string", this, ObjectType, GetRawObject);

            TypeType = new("type", this, ObjectType, GetRawObject);

            BaseFuncType = new("func", this, ObjectType, 2, GetRawObject);//?

            BaseMaybeType = new("maybe", this, ObjectType, 1, GetRawObject);//?

            Unit = RavelObject.GetVoid(this);
            True = RavelObject.GetBoolean(true, this);
            False = RavelObject.GetBoolean(false, this);
            None = RavelObject.GetNone(this);
            RegistFunctions();
        }

        private void RegistFunctions()
        {
            RavelType OTB = GetFuncType(BoolType, ObjectType, TypeType);

            RavelType OSS = GetFuncType(StringType, ObjectType, StringType);

            RavelRealFunction objectIs = new(ObjectIs, OTB, this, true);

            ObjectType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new (SyntaxKind.Is, RavelBinaryOperatorKind.TypeIs, objectIs),
                new(SyntaxKind.Plus, RavelBinaryOperatorKind.Addition, new RavelRealFunction(StringAdd, OSS, this, true))

            });
            RavelType OS = GetFuncType(StringType, ObjectType);
            ObjectType.SonVariables["ToString"] = new(RavelObject.GetFunction(new RavelRealFunction(ObjGetString, OS, this, true), this), "ToString", true, true, false, true);



            RavelType III = GetFuncType(IntType, IntType, IntType);
            RavelType IIB = GetFuncType(BoolType, IntType, IntType);
            IntType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new(SyntaxKind.Plus, RavelBinaryOperatorKind.Addition, new RavelRealFunction(IntAdd, III, this, true)),
                new(SyntaxKind.Minus, RavelBinaryOperatorKind.Subtraction, new RavelRealFunction(IntSub, III, this, true)),
                new(SyntaxKind.Star, RavelBinaryOperatorKind.Multiplication, new RavelRealFunction(IntMul, III, this, true)),
                new(SyntaxKind.Slash, RavelBinaryOperatorKind.Division, new RavelRealFunction(IntDiv, III, this, true)),
                new(SyntaxKind.Percent, RavelBinaryOperatorKind.Mod, new RavelRealFunction(IntMod, III, this, true)),
                new(SyntaxKind.StarStar, RavelBinaryOperatorKind.Power, new RavelRealFunction(IntPow, III, this, true)),
                new(SyntaxKind.Large, RavelBinaryOperatorKind.LargeComparision, new RavelRealFunction(IntLarge, IIB, this, true)),
                new(SyntaxKind.Small, RavelBinaryOperatorKind.SmallComparision, new RavelRealFunction(IntSmall, IIB, this, true)),
                new(SyntaxKind.LargeEqual, RavelBinaryOperatorKind.LargeEqualComparision, new RavelRealFunction(IntLargeEqual, IIB, this, true)),
                new(SyntaxKind.SmallEqual, RavelBinaryOperatorKind.SmallEqualComparision, new RavelRealFunction(IntSmallEqual, IIB, this, true)),
                new(SyntaxKind.EqualEqual, RavelBinaryOperatorKind.EqualComparision, new RavelRealFunction(IntEqual, IIB, this, true)),
                new(SyntaxKind.NotEqual, RavelBinaryOperatorKind.NotEqualComparision, new RavelRealFunction(IntNotEqual, IIB, this, true)),
            });

            RavelType II = GetFuncType(IntType, IntType);
            IntType.UnaryOperators.AddRange(new RavelUnaryOperator[]
            {
                new(SyntaxKind.Minus, RavelUnaryOperatorKind.Negation, new RavelRealFunction(IntNegation, II, this, true)),
                new(SyntaxKind.Plus, RavelUnaryOperatorKind.Indentity, new RavelRealFunction(IntIdentity, II, this, true)),
            });
            RavelType BBB = GetFuncType(BoolType, BoolType, BoolType);

            BoolType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new(SyntaxKind.And, RavelBinaryOperatorKind.And, new RavelRealFunction(BoolAnd, BBB, this, true)),
                new(SyntaxKind.Or, RavelBinaryOperatorKind.Or, new RavelRealFunction(BoolOr, BBB, this, true)),
                new(SyntaxKind.ShortCutAnd, RavelBinaryOperatorKind.ShortCutAnd, new RavelRealFunction(BoolAnd, BBB, this, true)),
                new(SyntaxKind.ShortCutOr, RavelBinaryOperatorKind.ShortCutOr, new RavelRealFunction(BoolOr, BBB, this, true)),
            });
            RavelType BB = GetFuncType(BoolType, BoolType);

            BoolType.UnaryOperators.AddRange(new RavelUnaryOperator[]
            {
                new(SyntaxKind.Not, RavelUnaryOperatorKind.Not, new RavelRealFunction(BoolNot, BB, this, true)),
            });

            RavelType SOS = GetFuncType(StringType, StringType, ObjectType);

            StringType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new(SyntaxKind.Plus, RavelBinaryOperatorKind.Addition, new RavelRealFunction(StringAdd, SOS, this, true))
            });

            RavelType TTT = GetFuncType(TypeType, TypeType, TypeType);
            TypeType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new(SyntaxKind.MinusLarge, RavelBinaryOperatorKind.Point, new RavelRealFunction(TypePoint, TTT, this, true))
            });
        }

        public RavelType VoidType { get; }
        public RavelType IntType { get; }
        public RavelType BoolType { get; }
        public RavelType StringType { get; }
        public RavelType TypeType { get; }
        public RavelType BaseFuncType { get; }
        public RavelType BaseMaybeType { get; }
        public RavelType ObjectType { get; }
        public RavelObject Unit { get; }
        public RavelObject True { get; }
        public RavelObject False { get; }
        public RavelObject None { get; }
        public RavelObject GetRawObject(RavelType type, object? raw)
        {
            return new(raw!, type, this);
        }
        public RavelType GetFuncType(RavelType returnType, params RavelType[] types)
        {
            types = types.Append(returnType).ToArray();
            RavelType type = types[^1];
            string name = types[^1].ToString();

            for(int index = types.Length - 1; index >= 0; index--)
            {
                if (TypeMap.TryGetValue(name, out var val))
                {
                    type = val;
                }
                else
                {
                    type = new(name, BaseFuncType, this, types[index], type);
                }
                if (index - 1 >= 0)
                {
                    name = $"{types[index - 1]}->{name}";
                }
            }
            return type;
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
            return RavelObject.GetType(operand.Type, this);
        }
        internal RavelObject StringAdd(RavelObject left, RavelObject right)
        {
            string l = left.ToString();
            string r = right.ToString();
            return RavelObject.GetString(l + r, this);
        }
        internal RavelObject TypePoint(RavelObject left, RavelObject right)
        {
            var l = left.GetValue<RavelType>();
            var r = right.GetValue<RavelType>();
            return RavelObject.GetType(GetFuncType(r, l), this);
        }

    }
}
