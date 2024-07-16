using System;
using System.Numerics;
namespace Ravel.Values
{
    public sealed class RavelGlobal
    {
        public RavelGlobal()
        {
            SyntaxFacts = new();
            SyntaxFacts = new();


            TypePool = new();
            
            RavelType OT = TypePool.GetFuncType(TypePool.TypeType, TypePool.ObjectType);
            RavelRealFunction typeOf = new(TypePool.ObjectTypeOf, OT, true);

            RavelType III = TypePool.GetFuncType(TypePool.IntType, TypePool.IntType, TypePool.IntType);
            RavelRealFunction add = new(TypePool.IntAdd, III, true);

            var US = TypePool.GetFuncType(TypePool.StringType, TypePool.VoidType);
            RavelRealFunction input = new(VoidInput, US, false);

            var OU = TypePool.GetFuncType(TypePool.VoidType, TypePool.StringType);
            RavelRealFunction print = new(VoidPrint, OU, false);

            RavelRealFunction randint = new(Randint, III, false);

            var TT = TypePool.GetFuncType(TypePool.ObjectType, TypePool.TypeType);

            Variables = new(TypePool.SystemScope)
            {
                ["typeof"] = new(RavelObject.GetFunction(typeOf), "typeof", true, true),
                ["add"] = new(RavelObject.GetFunction(add), "add", false, false),
                ["input"] = new(RavelObject.GetFunction(input), "input", false, true, true),
                ["print"] = new(RavelObject.GetFunction(print), "print", false, true),
                ["randint"] = new(RavelObject.GetFunction(randint), "randint", false, true),
                ["list"] = new(RavelObject.GetFunction(TypePool.ListConstructor.Function), "list", true, true),
            };
        }
        public RavelSyntaxFacts SyntaxFacts { get; }
        public RavelTypePool TypePool { get; }
        public RavelScope Variables { get; }

        RavelObject VoidInput(RavelObject obj)
        {
            return RavelObject.GetString(Console.ReadLine()!, TypePool);
        }
        RavelObject VoidPrint(RavelObject obj)
        {
            Console.WriteLine(obj);
            return TypePool.Unit;
        }

        Random? random;
        RavelObject Randint(RavelObject min, RavelObject max)
        {
            random ??= new();
            return RavelObject.GetInteger(random.NextInt64((long)min.GetValue<BigInteger>(), (long)max.GetValue<BigInteger>()), TypePool);
        }

    }
}
