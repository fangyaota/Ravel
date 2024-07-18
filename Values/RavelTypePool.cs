using Ravel.Syntax;

using System.Numerics;
using System.Text;

using SysType = System.Type;

namespace Ravel.Values
{
    public class RavelTypePool
    {
        public Dictionary<string, RavelType> TypeMap { get; } = new();

#nullable disable
        public RavelTypePool()
        {
            SystemScope = new()
            {
            };
            RegistObjects();
            RegistOperators();
            RegistBuiltins();
        }
#nullable enable

        private void RegistObjects()
        {
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

            Unit = RavelObject.GetVoid(this);
            True = RavelObject.GetBoolean(true, this);
            False = RavelObject.GetBoolean(false, this);
        }

        private void RegistBuiltins()
        {
            SystemScope.TryDeclare("int", RavelObject.GetType(IntType), true, true);
            SystemScope.TryDeclare("bool", RavelObject.GetType(BoolType), true, true);
            SystemScope.TryDeclare("string", RavelObject.GetType(StringType), true, true);
            SystemScope.TryDeclare("type", RavelObject.GetType(TypeType), true, true);
            SystemScope.TryDeclare("object", RavelObject.GetType(ObjectType), true, true);
            SystemScope.TryDeclare("void", RavelObject.GetType(VoidType), true, true);

            SystemScope.TryDeclare("callable", RavelObject.GetType(CallableType), true, true);
            SystemScope.TryDeclare("function", RavelObject.GetFunction(FunctionConstructor.Function), true, true);

            SystemScope.TryDeclare("enumerable", RavelObject.GetType(EnumerableType), true, true);
            SystemScope.TryDeclare("list", RavelObject.GetFunction(ListConstructor.Function), true, true);

            SystemScope.TryDeclare("true", True, true, true);
            SystemScope.TryDeclare("false", False, true, true);
            SystemScope.TryDeclare("unit", Unit, true, true);
        }

        private void RegistOperators()
        {
            RavelType OTB = GetFuncType(BoolType, ObjectType, TypeType);

            RavelType OSS = GetFuncType(StringType, ObjectType, StringType);

            RavelType OOB = GetFuncType(BoolType, ObjectType, ObjectType);

            RavelType OTO = GetFuncType(ObjectType, ObjectType, TypeType);

            RavelRealFunction objectIs = new(ObjectIs, OTB, true);

            RavelRealFunction objectEqual = new(ObjEqual, OOB, false);

            RavelRealFunction stringAdd = new(StringAdd, OSS, true);

            RavelRealFunction objectAs = new(ObjAs, OTO, true);//?


            ObjectType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new (SyntaxKind.Is, RavelBinaryOperatorKind.TypeIs, objectIs),
                new(SyntaxKind.Plus, RavelBinaryOperatorKind.Addition, stringAdd),
                new(SyntaxKind.EqualEqual, RavelBinaryOperatorKind.EqualComparision, objectEqual),
                new(SyntaxKind.As, RavelBinaryOperatorKind.As, objectAs),

            });
            RavelType OS = GetFuncType(StringType, ObjectType);
            ObjectType.SonVariables.TryDeclare("ToString", RavelObject.GetFunction(new RavelRealFunction(ObjGetString, OS, true)), true, true, true);



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
            EnumerableType.SonVariables.TryDeclare("ToString", RavelObject.GetFunction(new RavelRealFunction(ListToString, LS, true)), true, true, true);
        }
        

        
        public RavelType VoidType { get; private set; }
        public RavelType IntType { get; private set; }
        public RavelType BoolType { get; private set; }
        public RavelType StringType { get; private set; }
        public RavelType TypeType { get; private set; }
        public RavelRealConstructor FunctionConstructor { get; private set; }
        public RavelRealConstructor ListConstructor { get; private set; }
        public RavelType EnumerableType { get; private set; }
        public RavelType CallableType { get; private set; }
        public RavelType ObjectType { get; private set; }
        public RavelObject Unit { get; private set; }
        public RavelObject True { get; private set; }
        public RavelObject False { get; private set; }
        public RavelScope SystemScope { get; private set; }

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
                    name = FunctionConstructor.GetSpecificName(types[index - 1], currentType);
                }
            }
            return currentType;
        }
        private RavelType TypePoolGetFunctionType(RavelType f, RavelType s)
        {
            return new RavelType(FunctionConstructor, CallableType, new RavelType[] { f, s }, new());
        }

        private RavelObject GetListType(NeoEvaluator evaluator, RavelObject first)
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
        internal RavelObject ObjEqual(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            object l = left.GetValue<object>();
            object r = right.GetValue<object>();
            return l == r ? True : False;
        }
        internal RavelObject ObjGetString(NeoEvaluator evaluator, RavelObject arg)
        {
            return RavelObject.GetString(arg.ToString(), this);
        }
        internal RavelObject IntAdd(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return RavelObject.GetInteger(l + r, this);
        }
        internal RavelObject IntSub(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return RavelObject.GetInteger(l - r, this);
        }
        internal RavelObject IntMul(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return RavelObject.GetInteger(l * r, this);
        }
        internal RavelObject IntDiv(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return RavelObject.GetInteger(l / r, this);
        }
        internal RavelObject IntMod(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return RavelObject.GetInteger(l % r, this);
        }
        internal RavelObject IntPow(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return RavelObject.GetInteger(BigInteger.Pow(l, (int)r), this);
        }
        internal RavelObject IntLarge(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return l > r ? True : False;
        }
        internal RavelObject IntLargeEqual(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return l >= r ? True : False;
        }
        internal RavelObject IntSmall(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return l < r ? True : False;
        }
        internal RavelObject IntSmallEqual(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return l <= r ? True : False;
        }
        internal RavelObject IntEqual(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return l == r ? True : False;
        }
        internal RavelObject IntNotEqual(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return l != r ? True : False;
        }
        internal RavelObject BoolAnd(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            bool l = left.GetValue<bool>();
            bool r = right.GetValue<bool>();
            return l & r ? True : False;
        }
        internal RavelObject BoolOr(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            bool l = left.GetValue<bool>();
            bool r = right.GetValue<bool>();
            return l | r ? True : False;
        }
        internal RavelObject IntIdentity(NeoEvaluator evaluator, RavelObject obj)
        {
            return obj;
        }
        internal RavelObject IntNegation(NeoEvaluator evaluator, RavelObject obj)
        {
            return RavelObject.GetInteger(-obj.GetValue<BigInteger>(), this);
        }
        internal RavelObject BoolNot(NeoEvaluator evaluator, RavelObject obj)
        {
            return obj.GetValue<bool>() ? False : True;
        }
        internal RavelObject ObjectIs(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            return left.Type.IsSonOrEqual(right.GetValue<RavelType>()) ? True : False;
        }
        internal RavelObject ObjectTypeOf(NeoEvaluator evaluator, RavelObject operand)
        {
            return RavelObject.GetType(operand.Type);
        }
        internal RavelObject StringAdd(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            string l = left.ToString();
            string r = right.ToString();
            return RavelObject.GetString(l + r, this);
        }
        internal RavelObject ObjAs(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            return left;
        }
        internal RavelObject TypePoint(NeoEvaluator evaluator, RavelObject first, RavelObject second)
        {
            var f = first.GetValue<RavelType>();
            var s = second.GetValue<RavelType>();
            RavelType func = GetFuncType(s, f);
            return RavelObject.GetType(func);
        }
        internal RavelObject ListToString(NeoEvaluator evaluator, RavelObject list)
        {
            StringBuilder builder = new();
            builder.Append('[');
            builder.AppendJoin(' ', list.GetValue<List<RavelObject>>());
            builder.Append(']');
            return RavelObject.GetString(builder.ToString(), this);
        }
    }
}
