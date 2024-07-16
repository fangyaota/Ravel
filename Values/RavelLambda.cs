using Ravel.Binding;

namespace Ravel.Values
{
    internal class RavelLambda : RavelFunction
    {
        public RavelLambda(BoundFunctionDefiningExpression expression, RavelScope scope, RavelGlobal global)
            : base(expression.Type, expression.Parameters.Count)
        {
            Expression = expression;
            Scope = scope;
            Global = global;
        }

        public BoundFunctionDefiningExpression Expression { get; }
        public RavelScope Scope { get; }
        public RavelGlobal Global { get; }

        public override bool IsConst => false;//?

        protected override RavelObject InvokeMust(RavelObject[] obj)
        {
            RavelScope sco = new RavelScope(Scope);
            for (int i = 0; i < Expression.Parameters.Count; i++)
            {
                sco.TryDeclare(Expression.Parameters[i].Name, obj[i], false, false);
            }
            sco.TryDeclare("self", RavelObject.GetFunction(this), true, false);
            Evaluator evaluator = new Evaluator(Expression.Sentence, Global, sco);
            RavelObject result = evaluator.EvaluateExpression(Expression.Sentence);
            return result;
        }
    }
}
