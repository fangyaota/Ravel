using System.Numerics;
namespace Ravel.Values
{
    public sealed class RavelGlobal
    {
        public bool Dynamic { get; set; } = false;
        public RavelGlobal()
        {
            SyntaxFacts = new();

            TypePool = new();

            RavelType OT = TypePool.FunctionTypeOf(TypePool.TypeType, TypePool.ObjectType);

            RavelType III = TypePool.FunctionTypeOf(TypePool.IntType, TypePool.IntType, TypePool.IntType);

            RavelType US = TypePool.FunctionTypeOf(TypePool.StringType, TypePool.VoidType);

            RavelType SU = TypePool.FunctionTypeOf(TypePool.VoidType, TypePool.StringType);

            RavelType SO = TypePool.FunctionTypeOf(TypePool.ObjectType, TypePool.StringType);

            RavelType TT = TypePool.FunctionTypeOf(TypePool.ObjectType, TypePool.TypeType);

            RavelType OU = TypePool.FunctionTypeOf(TypePool.VoidType, TypePool.ObjectType);

            RavelType OU_C = TypePool.FunctionTypeOf(TypePool.ObjectType, OU);

            RavelType OU_C_C = TypePool.FunctionTypeOf(TypePool.ObjectType, OU_C);



            RavelRealFunction typeOf = new(TypePool.ObjectTypeOf, OT, true);

            RavelRealFunction add = new(TypePool.IntAdd, III, true);

            RavelRealFunction input = new(VoidInput, US, false);

            RavelRealFunction print = new(VoidPrint, SU, false);

            RavelRealFunction randint = new(Randint, III, false);
            RavelRealFunction callcc = new(Callcc, OU_C_C, false);

            RavelRealFunction eval = new(Eval, SO, false);
            Variables = new(TypePool.SystemScope)
            {
                new(typeOf.GetRavelObject(), "typeof", true),
                new(add.GetRavelObject(), "add", false),
                new(input.GetRavelObject(), "input", true, true),
                new(print.GetRavelObject(), "print", true),
                new(randint.GetRavelObject(), "randint", true),
                new(callcc.GetRavelObject(), "callcc", true),
                new(eval.GetRavelObject(), "eval", true),
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
            Console.WriteLine(obj.GetValue<string>());
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
            RavelType OU = TypePool.FunctionTypeOf(TypePool.VoidType, TypePool.ObjectType);
            RavelRealFunction ret = new(Return, OU, false);
            function.Call(evaluator, ret.GetRavelObject());
            return evaluator.EvaluateInstantResult(current);
        }
        private RavelObject Eval(NeoEvaluator evaluator, RavelObject obj)
        {
            string str = obj.GetValue<string>();
            Compiler compiler = new(str, this, evaluator.CurrentCallStack.Scope);
            if (compiler.Diagnostics.Any())
            {
                throw new RavelEvaluateException("编译错误");
            }
            var result = compiler.Evaluator.Evaluate();
            if (compiler.Evaluator.Diagnostics.Any())
            {
                throw new RavelEvaluateException("求值错误");
            }
            return result;
        }
    }
}
