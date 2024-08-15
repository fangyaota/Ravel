using Ravel.Syntax;

using System.Collections;
using System.Collections.Generic;
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
            RegistSystemScope();
            RegistTypes();
            RegistConsts();
            RegistConstructors();
            RegistOperators();
            RegistBuiltins();
        }

#nullable enable


        private void RegistSystemScope()
        {
            SystemScope = new()
            {
            };
        }
        private void RegistTypes()
        {
            ObjectType = RavelType.GetRavelType("object", this, true);

            TypeType = RavelType.GetRavelType("type", ObjectType, true);

            VoidType = RavelType.GetRavelType("void", ObjectType, true);

            IntType = RavelType.GetRavelType("int", ObjectType, true);

            BoolType = RavelType.GetRavelType("bool", ObjectType, true);

            StringType = RavelType.GetRavelType("string", ObjectType, true);

            EnumerableType = RavelType.GetRavelType("enumerable", ObjectType, true);

            CallableType = RavelType.GetRavelType("callable", ObjectType, true);

            VariableType = RavelType.GetRavelType("variable", ObjectType, true);
        }
        private void RegistConsts()
        {
            Unit = RavelObject.GetVoid(this);
            True = BoolType.GetRavelObject(true);
            False = BoolType.GetRavelObject(false);
        }
        private void RegistConstructors()
        {
        }
        private void RegistOperators()
        {
            RavelType OTB = FunctionTypeOf(BoolType, ObjectType, TypeType);

            RavelType SSS = FunctionTypeOf(StringType, StringType, StringType);

            RavelType OOB = FunctionTypeOf(BoolType, ObjectType, ObjectType);

            RavelType OTO = FunctionTypeOf(ObjectType, ObjectType, TypeType);

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
            RavelType OS = FunctionTypeOf(StringType, ObjectType);
            RavelType IS = FunctionTypeOf(StringType, IntType);
            RavelType US = FunctionTypeOf(StringType, VoidType);
            RavelType SS = FunctionTypeOf(StringType, StringType);
            RavelType BS = FunctionTypeOf(StringType, BoolType);
            RavelType TS = FunctionTypeOf(StringType, TypeType);
            RavelType EIO = FunctionTypeOf(ObjectType, EnumerableType, IntType);
            RavelType EIOO = FunctionTypeOf(ObjectType, EnumerableType, IntType, ObjectType);
            RavelType TO = FunctionTypeOf(ObjectType, TypeType);

            ObjectType.SonVariables.TryDeclare("ToString", new RavelRealFunction(ObjGetString, OS, true).GetRavelObject(), true, true);
            IntType.SonVariables.TryDeclare("ToString", new RavelRealFunction(IntGetString, IS, true).GetRavelObject(), true, true);
            VoidType.SonVariables.TryDeclare("ToString", new RavelRealFunction(VoidGetString, US, true).GetRavelObject(), true, true);
            StringType.SonVariables.TryDeclare("ToString", new RavelRealFunction(StringGetString, SS, true).GetRavelObject(), true, true);
            BoolType.SonVariables.TryDeclare("ToString", new RavelRealFunction(BoolGetString, BS, true).GetRavelObject(), true, true);
            TypeType.SonVariables.TryDeclare("ToString", new RavelRealFunction(TypeGetString, TS, true).GetRavelObject(), true, true);

            EnumerableType.SonVariables.TryDeclare("Get", new RavelRealFunction(GetVar, EIO, true).GetRavelObject(), true, true);
            EnumerableType.SonVariables.TryDeclare("Set", new RavelRealFunction(SetVar, EIOO, true).GetRavelObject(), true, true);

            TypeType.SonVariables.TryDeclare("New", new RavelRealFunction(NewClass, TO, false).GetRavelObject(), true, true);

            ObjectType.ImplictConverters.Add(new RavelRealConverter(new RavelRealFunction(ObjToString, OS, true)));

            RavelType III = FunctionTypeOf(IntType, IntType, IntType);
            RavelType IIB = FunctionTypeOf(BoolType, IntType, IntType);
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

            RavelType II = FunctionTypeOf(IntType, IntType);
            IntType.UnaryOperators.AddRange(new RavelUnaryOperator[]
            {
                new(SyntaxKind.Minus, RavelUnaryOperatorKind.Negation, new RavelRealFunction(IntNegation, II, true)),
                new(SyntaxKind.Plus, RavelUnaryOperatorKind.Indentity, new RavelRealFunction(IntIdentity, II, true)),
            });
            RavelType BBB = FunctionTypeOf(BoolType, BoolType, BoolType);

            BoolType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new(SyntaxKind.And, RavelBinaryOperatorKind.And, new RavelRealFunction(BoolAnd, BBB, true)),
                new(SyntaxKind.Or, RavelBinaryOperatorKind.Or, new RavelRealFunction(BoolOr, BBB, true)),
                new(SyntaxKind.ShortCutAnd, RavelBinaryOperatorKind.ShortCutAnd, new RavelRealFunction(BoolAnd, BBB, true)),
                new(SyntaxKind.ShortCutOr, RavelBinaryOperatorKind.ShortCutOr, new RavelRealFunction(BoolOr, BBB, true)),
            });
            RavelType BB = FunctionTypeOf(BoolType, BoolType);

            BoolType.UnaryOperators.AddRange(new RavelUnaryOperator[]
            {
                new(SyntaxKind.Not, RavelUnaryOperatorKind.Not, new RavelRealFunction(BoolNot, BB, true)),
            });


            StringType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new(SyntaxKind.Plus, RavelBinaryOperatorKind.Addition, stringAdd)
            });

            RavelType TTT = FunctionTypeOf(TypeType, TypeType, TypeType);
            TypeType.BinaryOperators.AddRange(new RavelBinaryOperator[]
            {
                new(SyntaxKind.MinusLarge, RavelBinaryOperatorKind.Point, new RavelRealFunction(TypePoint, TTT, true))
            });
            RavelType LS = FunctionTypeOf(StringType, EnumerableType);//?
            EnumerableType.SonVariables.TryDeclare("ToString", new RavelRealFunction(ListToString, LS, true).GetRavelObject(), true, true);
        }
        private void RegistBuiltins()
        {
            var TT = FunctionTypeOf(TypeType, TypeType);
            var TTT = FunctionTypeOf(TypeType, TypeType, TypeType);
            var STOD = FunctionTypeOf(VariableType, StringType, TypeType, ObjectType);

            var ST_VL_T = FunctionTypeOf(TypeType, StringType, TypeType, TypePoolGetListType(VariableType));
            RavelType SI = FunctionTypeOf(IntType, StringType);

            RavelRealFunction function = new(TypePoint, TTT, true);
            RavelRealFunction list = new(GetListType, TT, true);
            RavelRealFunction def = new(DefineVariable, STOD, true);
            RavelRealFunction cl = new(GetClass, ST_VL_T, true);

            SystemScope.TryDeclare("int", TypeType.GetRavelObject(IntType), true);
            SystemScope.TryDeclare("bool", TypeType.GetRavelObject(BoolType), true);
            SystemScope.TryDeclare("string", TypeType.GetRavelObject(StringType), true);
            SystemScope.TryDeclare("type", TypeType.GetRavelObject(TypeType), true);
            SystemScope.TryDeclare("object", TypeType.GetRavelObject(ObjectType), true);
            SystemScope.TryDeclare("void", TypeType.GetRavelObject(VoidType), true);

            if(SystemScope.TryGetVariable("int", out var integer))
            {
                integer.Object.TrySetSonValue("Of", new RavelRealFunction(IntOfString, SI, false).GetRavelObject());
            }
            SystemScope.TryDeclare("callable", TypeType.GetRavelObject(CallableType), true);

            SystemScope.TryDeclare("function", function.GetRavelObject(), true);

            SystemScope.TryDeclare("enumerable", TypeType.GetRavelObject(EnumerableType), true);
            SystemScope.TryDeclare("list", list.GetRavelObject(), true);
            SystemScope.TryDeclare("variable", TypeType.GetRavelObject(VariableType), true);

            SystemScope.TryDeclare("true", True, true);
            SystemScope.TryDeclare("false", False, true);
            SystemScope.TryDeclare("unit", Unit, true);

            SystemScope.TryDeclare("def", def.GetRavelObject(), true);
            SystemScope.TryDeclare("class", cl.GetRavelObject(), true);
        }


        public RavelType VoidType { get; private set; }
        public RavelType IntType { get; private set; }
        public RavelType BoolType { get; private set; }
        public RavelType StringType { get; private set; }
        public RavelType TypeType { get; private set; }

        //public RavelRealConstructor FunctionConstructor { get; private set; }
        //public RavelRealConstructor ListConstructor { get; private set; }
        public RavelType EnumerableType { get; private set; }
        public RavelType CallableType { get; private set; }
        public RavelType ObjectType { get; private set; }
        public RavelType VariableType { get; private set; }
        public RavelObject Unit { get; private set; }
        public RavelObject True { get; private set; }
        public RavelObject False { get; private set; }
        public RavelScope SystemScope { get; private set; }

        public RavelObject GetRawObject(RavelType type, object? raw)
        {
            return new(raw!, type, this);
        }
        public RavelType FunctionTypeOf(RavelType returnType, params RavelType[] parameters)
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
            return RavelType.GetRavelType("function", CallableType, Array.Empty<RavelVariable>(), from, to);
        }
        internal RavelType TypePoolGetListType(RavelType element)
        {
            return RavelType.GetRavelType("list", EnumerableType, Array.Empty<RavelVariable>(), element);
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
        internal RavelObject ObjToString(NeoEvaluator evaluator, RavelObject arg)
        {
            var last = evaluator.CurrentCallStack;
            arg.TryReturnSonValue(evaluator, "ToString");
            return evaluator.EvaluateInstantResult(last);
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
            var type = right.GetValue<RavelType>();
            if (left.Type.IsSonOrEqual(type))
            {
                return left;
            }
            var convert = left.Type.GetImplictConverter(type);
            if(convert != null)
            {
                var last = evaluator.CurrentCallStack;
                convert.Function.Invoke(evaluator, left);
                return evaluator.EvaluateInstantResult(last);
            }
            var str = ObjToString(evaluator, left).GetValue<string>();
            var t = ObjToString(evaluator, right).GetValue<string>();
            throw new RavelEvaluateException($"cannot explict cast {str} from {left.Type} to {t}");
        }
        internal RavelObject TypePoint(NeoEvaluator evaluator, RavelObject first, RavelObject second)
        {
            RavelType f = first.GetValue<RavelType>();
            RavelType s = second.GetValue<RavelType>();
            RavelType func = FunctionTypeOf(s, f);
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
        internal RavelObject DefineVariable(NeoEvaluator evaluator, RavelObject name, RavelObject type, RavelObject defaultValue)//?
        {
            string n = name.GetValue<string>();
            RavelType t = type.GetValue<RavelType>();

            return VariableType.GetRavelObject(new RavelVariable(defaultValue, n, false));
        }
        internal RavelObject GetVar(NeoEvaluator evaluator, RavelObject first, RavelObject second)
        {
            var ie = first.GetValue<IList<RavelObject>>();
            int i = (int)second.GetValue<BigInteger>();
            if(ie.Count <= i)
            {
                throw new RavelEvaluateException($"索引越界了！");
            }
            return ie[i];//?
        }
        internal RavelObject SetVar(NeoEvaluator evaluator, RavelObject first, RavelObject second, RavelObject val)
        {
            var ie = first.GetValue<IList<RavelObject>>();
            int i = (int)second.GetValue<BigInteger>();
            if (ie.Count <= i)
            {
                throw new RavelEvaluateException($"索引越界了！");
            }
            ie[i] = val;
            return ie[i];//?
        }
        internal RavelObject GetClass(NeoEvaluator evaluator, RavelObject first, RavelObject second, RavelObject third)
        {
            var ie = third.GetValue<IList<RavelObject>>().Select(x=> x.GetValue<RavelVariable>()).ToArray();
            RavelType parentType = second.GetValue<RavelType>();
            string name = first.GetValue<string>();
            var type = RavelType.GetRavelType(name, parentType, ie);//todo
            type.IsBuiltin = false;
            return TypeType.GetRavelObject(type);
        }
        internal RavelObject NewClass(NeoEvaluator evaluator, RavelObject first)
        {
            var t = first.GetValue<RavelType>();
            if(t.IsBuiltin)
            {
                throw new RavelEvaluateException($"内置类型不可New！");
            }
            return t.GetRavelObject(default!);//todo
        }
        internal RavelObject IntOfString(NeoEvaluator evaluator, RavelObject first)
        {
            string s = first.GetValue<string>();
            return IntType.GetRavelObject(BigInteger.Parse(s));//todo
        }
    }
}
