using Ravel.Values;

namespace Ravel.Binding
{
    public sealed class BoundConvertExpression : BoundExpression
    {
        public BoundConvertExpression(BoundExpression expression, RavelImplictConverter converter)
        {
            Expression = expression;
            Converter = converter;
        }

        public BoundExpression Expression { get; }
        public RavelImplictConverter Converter { get; }

        public override RavelType Type => Converter.To;

        public override bool IsConst => Expression.IsConst && Converter.Function.IsConst;

        public override BoundNodeKind Kind => BoundNodeKind.ConvertExpression;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Expression;
        }
    }
}
