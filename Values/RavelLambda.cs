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

        protected override void InvokeMust(NeoEvaluator evaluator, RavelObject[] obj)
        {
            evaluator.CurrentCallStack = new(evaluator.CurrentCallStack, Expression.Sentence);
            RavelScope sco = evaluator.CurrentCallStack.Scope = new(Scope);
            evaluator.CurrentCallStack.LastFunctionCall = evaluator.CurrentCallStack;
            for (int i = 0; i < Expression.Parameters.Count; i++)
            {
                sco.TryDeclare(Expression.Parameters[i].Name, obj[i], false, false);
            }
            sco.TryDeclare("self", GetRavelObject(), true, false);
        }
    }
}
