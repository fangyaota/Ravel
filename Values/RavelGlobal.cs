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
                ["int"] = new(RavelObject.GetType(TypePool.IntType, TypePool), "int", true, true),
                ["bool"] = new(RavelObject.GetType(TypePool.BoolType, TypePool), "bool", true, true),
                ["string"] = new(RavelObject.GetType(TypePool.StringType, TypePool), "string", true, true),
                ["type"] = new(RavelObject.GetType(TypePool.TypeType, TypePool), "type", true, true),
                ["object"] = new(RavelObject.GetType(TypePool.ObjectType, TypePool), "object", true, true),
                ["void"] = new(RavelObject.GetType(TypePool.VoidType, TypePool), "void", true, true),
            };
            RavelType OT = TypePool.GetFuncType(TypePool.TypeType, TypePool.ObjectType);
            RavelRealFunction typeOf = new(TypePool.ObjectTypeOf, OT, TypePool, true);
            Variables["typeof"] = new(RavelObject.GetFunction(typeOf, TypePool), "typeof", true, true);

            RavelType III = TypePool.GetFuncType(TypePool.IntType, TypePool.IntType, TypePool.IntType);
            RavelRealFunction add = new(TypePool.IntAdd, III, TypePool, true);
            Variables["add"] = new(RavelObject.GetFunction(add, TypePool), "add", false, false);

            var US = TypePool.GetFuncType(TypePool.StringType, TypePool.VoidType);
            RavelRealFunction input = new(VoidInput, US, TypePool, false);
            Variables["input"] = new(RavelObject.GetFunction(input, TypePool), "input", false, true, false, true);

            var OU = TypePool.GetFuncType(TypePool.VoidType, TypePool.StringType);
            RavelRealFunction print = new(VoidPrint, OU, TypePool, false);
            Variables["print"] = new(RavelObject.GetFunction(print, TypePool), "print", false, true);
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
    }
}
