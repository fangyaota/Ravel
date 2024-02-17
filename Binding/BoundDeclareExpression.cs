using Ravel.Values;

namespace Ravel.Binding
{
    internal class BoundDeclareExpression : BoundExpression
    {
        public BoundDeclareExpression(string text, RavelType type)
        {
            Text = text;
            Type = type;
        }

        public string Text { get; }

        public override RavelType Type { get; }

        public override bool IsConst => true;//?

        public override BoundNodeKind Kind => BoundNodeKind.DeclareExpression;

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}