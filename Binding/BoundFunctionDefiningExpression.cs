using Ravel.Values;

namespace Ravel.Binding
{
    internal class BoundFunctionDefiningExpression : BoundExpression
    {
        public BoundFunctionDefiningExpression(List<RavelDefining> parameters, BoundExpression sentence, RavelType type)
        {
            Parameters = parameters;
            Sentence = sentence;
            Type = type;
        }

        public List<RavelDefining> Parameters { get; }
        public BoundExpression Sentence { get; }

        public override RavelType Type { get; }

        public override bool IsConst => false;//?

        public override BoundNodeKind Kind => BoundNodeKind.FunctionDefiningExpression;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Sentence;
        }
    }
}