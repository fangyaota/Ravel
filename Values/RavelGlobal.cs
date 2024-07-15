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
            Variables = new()
            {
                ["int"] = new(RavelObject.GetType(TypePool.IntType), "int", true, true),
                ["bool"] = new(RavelObject.GetType(TypePool.BoolType), "bool", true, true),
                ["string"] = new(RavelObject.GetType(TypePool.StringType), "string", true, true),
                ["type"] = new(RavelObject.GetType(TypePool.TypeType), "type", true, true),
                ["object"] = new(RavelObject.GetType(TypePool.ObjectType), "object", true, true),
                ["void"] = new(RavelObject.GetType(TypePool.VoidType), "void", true, true),
            };
            RavelType OT = TypePool.GetFuncType(TypePool.TypeType, TypePool.ObjectType);
            RavelRealFunction typeOf = new(TypePool.ObjectTypeOf, OT, TypePool, true);
            Variables["typeof"] = new(RavelObject.GetFunction(typeOf), "typeof", true, true);

            RavelType III = TypePool.GetFuncType(TypePool.IntType, TypePool.IntType, TypePool.IntType);
            RavelRealFunction add = new(TypePool.IntAdd, III, TypePool, true);
            Variables["add"] = new(RavelObject.GetFunction(add), "add", false, false);

            var US = TypePool.GetFuncType(TypePool.StringType, TypePool.VoidType);
            RavelRealFunction input = new(VoidInput, US, TypePool, false);
            Variables["input"] = new(RavelObject.GetFunction(input), "input", false, true, true);

            var OU = TypePool.GetFuncType(TypePool.VoidType, TypePool.StringType);
            RavelRealFunction print = new(VoidPrint, OU, TypePool, false);
            Variables["print"] = new(RavelObject.GetFunction(print), "print", false, true);

            RavelRealFunction randint = new(Randint, III, TypePool, false);
            Variables["randint"] = new(RavelObject.GetFunction(randint), "randint", false, true);

            var TT = TypePool.GetFuncType(TypePool.ObjectType, TypePool.TypeType);
            RavelRealFunction list = new(GetList, TT, TypePool, true, true);
            Variables["list"] = new(RavelObject.GetFunction(list), "list", true, true);
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

        RavelObject GetList(RavelObject type)
        {
            return RavelObject.GetType(TypePool.ListConstructor.GetRavelType(type.GetValue<RavelType>()));
        }
    }
}
