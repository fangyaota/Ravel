using System.Numerics;
namespace Ravel.Values
{
    public sealed class RavelGlobal
    {
        public RavelGlobal()
        {
            SyntaxFacts = new();

            TypePool = new();

            RavelType OT = TypePool.GetFuncType(TypePool.TypeType, TypePool.ObjectType);
            RavelRealFunction typeOf = new(TypePool.ObjectTypeOf, OT, true);

            RavelType III = TypePool.GetFuncType(TypePool.IntType, TypePool.IntType, TypePool.IntType);
            RavelRealFunction add = new(TypePool.IntAdd, III, true);

            RavelType US = TypePool.GetFuncType(TypePool.StringType, TypePool.VoidType);
            RavelRealFunction input = new(VoidInput, US, false);

            RavelType SU = TypePool.GetFuncType(TypePool.VoidType, TypePool.StringType);
            RavelRealFunction print = new(VoidPrint, SU, false);

            RavelRealFunction randint = new(Randint, III, false);

            RavelType TT = TypePool.GetFuncType(TypePool.ObjectType, TypePool.TypeType);

            RavelType OU = TypePool.GetFuncType(TypePool.VoidType, TypePool.ObjectType);

            RavelType OU_C = TypePool.GetFuncType(TypePool.ObjectType, OU);

            RavelType OU_C_C = TypePool.GetFuncType(TypePool.ObjectType, OU_C);

            RavelRealFunction callcc = new(Callcc, OU_C_C, false);

            Variables = new(TypePool.SystemScope)
            {
                new(typeOf.GetRavelObject(), "typeof", true, true),
                new(add.GetRavelObject(), "add", false, false),
                new(input.GetRavelObject(), "input", false, true, true),
                new(print.GetRavelObject(), "print", false, true),
                new(randint.GetRavelObject(), "randint", false, true),
                new(TypePool.ListConstructor.Function.GetRavelObject(), "list", true, true),
                new(callcc.GetRavelObject(), "callcc", false, true),
            };
        }
        public RavelSyntaxFacts SyntaxFacts { get; }
        public RavelTypePool TypePool { get; }
        public RavelScope Variables { get; }

        private RavelObject VoidInput(NeoEvaluator evaluator, RavelObject obj)
        {
            return TypePool.StringType.GetRavelObject(Console.ReadLine()!);
        }

        private RavelObject VoidPrint(NeoEvaluator evaluator, RavelObject obj)
        {
            Console.WriteLine(obj);
            return TypePool.Unit;
        }

        private Random? random;

        private RavelObject Randint(NeoEvaluator evaluator, RavelObject min, RavelObject max)
        {
            random ??= new();
            return TypePool.IntType.GetRavelObject(random.NextInt64((long)min.GetValue<BigInteger>(), (long)max.GetValue<BigInteger>()));
        }

        private RavelObject Callcc(NeoEvaluator evaluator, RavelObject function)
        {
            var current = evaluator.CurrentCallStack;
            RavelObject Return(NeoEvaluator evaluator, RavelObject result)
            {
                evaluator.CurrentCallStack = current;
                evaluator.AddResult(result);
                return TypePool.Unit;
            }
            RavelType OU = TypePool.GetFuncType(TypePool.VoidType, TypePool.ObjectType);
            RavelRealFunction ret = new(Return, OU, false);
            function.Call(evaluator, ret.GetRavelObject());
            return evaluator.EvaluateInstantResult(current);
        }
    }
}
