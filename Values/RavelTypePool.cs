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
            ObjectType = RavelType.GetRavelType("object", this);

            TypeType = RavelType.GetRavelType("type", ObjectType);

            VoidType = RavelType.GetRavelType("void", ObjectType);

            IntType = RavelType.GetRavelType("int", ObjectType);

            BoolType = RavelType.GetRavelType("bool", ObjectType);

            StringType = RavelType.GetRavelType("string", ObjectType);

            EnumerableType = RavelType.GetRavelType("enumerable", ObjectType);

            CallableType = RavelType.GetRavelType("callable", ObjectType);

            FunctionConstructor = new(null!, SystemScope, "function");

            RavelType TTT = GetFuncType(TypeType, TypeType, TypeType);
            RavelType TT = GetFuncType(TypeType, TypeType);

            FunctionConstructor.Function = new RavelRealFunction(TypePoint, TTT, true);

            ListConstructor = new(new RavelRealFunction(GetListType, TT, true), SystemScope, "list");

            CallStackType = RavelType.GetRavelType("callstack", ObjectType);

            Unit = RavelObject.GetVoid(this);
            True = BoolType.GetRavelObject(true);
            False = BoolType.GetRavelObject(false);
        }

        private void RegistBuiltins()
        {
            SystemScope.TryDeclare("int", TypeType.GetRavelObject(IntType), true, true);
            SystemScope.TryDeclare("bool", TypeType.GetRavelObject(BoolType), true, true);
            SystemScope.TryDeclare("string", TypeType.GetRavelObject(StringType), true, true);
            SystemScope.TryDeclare("type", TypeType.GetRavelObject(TypeType), true, true);
            SystemScope.TryDeclare("object", TypeType.GetRavelObject(ObjectType), true, true);
            SystemScope.TryDeclare("void", TypeType.GetRavelObject(VoidType), true, true);
            SystemScope.TryDeclare("callstack", TypeType.GetRavelObject(CallStackType), true, true);

            SystemScope.TryDeclare("callable", TypeType.GetRavelObject(CallableType), true, true);
            SystemScope.TryDeclare("function", FunctionConstructor.Function.GetRavelObject(), true, true);

            SystemScope.TryDeclare("enumerable", TypeType.GetRavelObject(EnumerableType), true, true);
            SystemScope.TryDeclare("list", ListConstructor.Function.GetRavelObject(), true, true);

            SystemScope.TryDeclare("true", True, true, true);
            SystemScope.TryDeclare("false", False, true, true);
            SystemScope.TryDeclare("unit", Unit, true, true);
        }

        private void RegistOperators()
        {
            RavelType OTB = GetFuncType(BoolType, ObjectType, TypeType);

            RavelType SSS = GetFuncType(StringType, StringType, StringType);

            RavelType OOB = GetFuncType(BoolType, ObjectType, ObjectType);

            RavelType OTO = GetFuncType(ObjectType, ObjectType, TypeType);

            RavelRealFunction objectIs = new(ObjectIs, OTB, true);

            RavelRealFunction objectEqual = new(ObjEqual, OOB, false);

            RavelRealFunction stringAdd = new(StringAdd, SSS, true);

            RavelRealFunction objectAs = new(ObjAs, OTO, true);//?


            ObjectType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new (SyntaxKind.Is, RavelBinaryOperatorKind.TypeIs, objectIs),
                new(SyntaxKind.EqualEqual, RavelBinaryOperatorKind.EqualComparision, objectEqual),
                new(SyntaxKind.As, RavelBinaryOperatorKind.As, objectAs),

            });
            RavelType OS = GetFuncType(StringType, ObjectType);
            RavelType IS = GetFuncType(StringType, IntType);
            RavelType US = GetFuncType(StringType, VoidType);
            RavelType SS = GetFuncType(StringType, StringType);
            RavelType BS = GetFuncType(StringType, BoolType);
            RavelType TS = GetFuncType(StringType, TypeType);

            ObjectType.SonVariables.TryDeclare("ToString", new RavelRealFunction(ObjGetString, OS, true).GetRavelObject(), true, true, true);
            IntType.SonVariables.TryDeclare("ToString", new RavelRealFunction(IntGetString, IS, true).GetRavelObject(), true, true, true);
            VoidType.SonVariables.TryDeclare("ToString", new RavelRealFunction(VoidGetString, US, true).GetRavelObject(), true, true, true);
            StringType.SonVariables.TryDeclare("ToString", new RavelRealFunction(StringGetString, SS, true).GetRavelObject(), true, true, true);
            BoolType.SonVariables.TryDeclare("ToString", new RavelRealFunction(BoolGetString, BS, true).GetRavelObject(), true, true, true);
            TypeType.SonVariables.TryDeclare("ToString", new RavelRealFunction(TypeGetString, TS, true).GetRavelObject(), true, true, true);


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


            StringType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new(SyntaxKind.Plus, RavelBinaryOperatorKind.Addition, stringAdd)
            });

            RavelType TTT = GetFuncType(TypeType, TypeType, TypeType);
            TypeType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new(SyntaxKind.MinusLarge, RavelBinaryOperatorKind.Point, new RavelRealFunction(TypePoint, TTT, true))
            });
            RavelType LS = GetFuncType(StringType, EnumerableType);//?
            EnumerableType.SonVariables.TryDeclare("ToString", new RavelRealFunction(ListToString, LS, true).GetRavelObject(), true, true, true);
        }



        public RavelType VoidType { get; private set; }
        public RavelType IntType { get; private set; }
        public RavelType BoolType { get; private set; }
        public RavelType StringType { get; private set; }
        public RavelType TypeType { get; private set; }

        public RavelType CallStackType { get; private set; }
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
        public RavelType GetFuncType(RavelType returnType, params RavelType[] parameters)
        {
            var total = parameters.Append(returnType).ToArray();
            RavelType currentType = total[^1];
            for (int index = total.Length - 2; index >= 0; index--)
            {
                currentType = TypePoolGetFunctionType(total[index], currentType);
            }
            return currentType;
        }
        internal RavelType TypePoolGetFunctionType(RavelType from, RavelType to)
        {
            return RavelType.GetRavelType(FunctionConstructor, CallableType, new[] { from, to });
        }
        internal RavelType TypePoolGetListType(RavelType element)
        {
            return RavelType.GetRavelType(ListConstructor, EnumerableType, new[] { element });
        }
        private RavelObject GetListType(NeoEvaluator evaluator, RavelObject first)
        {
            RavelType f = first.GetValue<RavelType>();
            RavelType func = TypePoolGetListType(f);
            return TypeType.GetRavelObject(func);
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
            return StringType.GetRavelObject($"Instance of {arg.Type}");
        }
        internal RavelObject IntGetString(NeoEvaluator evaluator, RavelObject arg)
        {
            return StringType.GetRavelObject(arg.GetValue<BigInteger>().ToString());
        }
        internal RavelObject StringGetString(NeoEvaluator evaluator, RavelObject arg)
        {
            return StringType.GetRavelObject(arg.GetValue<string>());
        }
        internal RavelObject BoolGetString(NeoEvaluator evaluator, RavelObject arg)
        {
            return StringType.GetRavelObject(arg.GetValue<bool>().ToString());
        }
        internal RavelObject TypeGetString(NeoEvaluator evaluator, RavelObject arg)
        {
            return StringType.GetRavelObject(arg.GetValue<RavelType>().ToString());
        }
        internal RavelObject VoidGetString(NeoEvaluator evaluator, RavelObject arg)
        {
            return StringType.GetRavelObject($"()");
        }
        internal RavelObject IntAdd(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return IntType.GetRavelObject(l + r);
        }
        internal RavelObject IntSub(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return IntType.GetRavelObject(l - r);
        }
        internal RavelObject IntMul(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return IntType.GetRavelObject(l * r);
        }
        internal RavelObject IntDiv(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return IntType.GetRavelObject(l / r);
        }
        internal RavelObject IntMod(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return IntType.GetRavelObject(l % r);
        }
        internal RavelObject IntPow(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            BigInteger l = left.GetValue<BigInteger>();
            BigInteger r = right.GetValue<BigInteger>();
            return IntType.GetRavelObject(BigInteger.Pow(l, (int)r));
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
            return IntType.GetRavelObject(-obj.GetValue<BigInteger>());
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
            return TypeType.GetRavelObject(operand.Type);
        }
        internal RavelObject StringAdd(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            string l = left.GetValue<string>();
            string r = right.GetValue<string>();
            return StringType.GetRavelObject(l + r);
        }
        internal RavelObject ObjAs(NeoEvaluator evaluator, RavelObject left, RavelObject right)
        {
            return left;
        }
        internal RavelObject TypePoint(NeoEvaluator evaluator, RavelObject first, RavelObject second)
        {
            RavelType f = first.GetValue<RavelType>();
            RavelType s = second.GetValue<RavelType>();
            RavelType func = GetFuncType(s, f);
            return TypeType.GetRavelObject(func);
        }
        internal RavelObject ListToString(NeoEvaluator evaluator, RavelObject list)
        {
            StringBuilder builder = new();
            builder.Append('[');
            var last = evaluator.CurrentCallStack;
            builder.AppendJoin(' ', list.GetValue<List<RavelObject>>().Select(i =>
            {
                i.TryReturnSonValue(evaluator, "ToString");
                var text = evaluator.EvaluateInstantResult(last);
                return text.GetValue<string>();
            }));
            builder.Append(']');
            return StringType.GetRavelObject(builder.ToString());
        }

    }
}
