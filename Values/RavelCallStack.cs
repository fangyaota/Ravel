using Ravel.Binding;

namespace Ravel.Values
{
    public class RavelCallStack
    {
        public RavelCallStack(RavelCallStack? parent, BoundExpression expression)
        {
            Parent = parent;
            LastFunctionCall = parent?.LastFunctionCall ?? this;
            Expression = expression;
            Scope = parent?.Scope ?? new();
        }
        public RavelCallStack(RavelScope scope, BoundExpression expression)
        {
            Parent = null;
            LastFunctionCall = this;
            Expression = expression;
            Scope = scope;
        }
        private RavelCallStack(RavelCallStack basis, RavelObject append)
        {
            Parent = basis.Parent;
            LastFunctionCall = basis.LastFunctionCall;
            Expression = basis.Expression;
            Scope = basis.Scope;
            SonResults = new(append, basis.SonResults);
        }
        private RavelCallStack(RavelCallStack basis)
        {
            Parent = basis.Parent;
            LastFunctionCall = basis.LastFunctionCall;
            Expression = basis.Expression;
            Scope = basis.Scope;
            SonResults = SingleLinkedList<RavelObject>.Empty;
        }
        public RavelCallStack? Parent { get; }
        public RavelCallStack LastFunctionCall { get; internal set; }
        public BoundExpression Expression { get; }
        public RavelScope Scope { get; internal set; }

        public SingleLinkedList<RavelObject> SonResults { get; } = SingleLinkedList<RavelObject>.Empty;

        public RavelCallStack AddSonResult(RavelObject result)
        {
            return new(this, result);
        }
        public RavelCallStack ClearSonResult()
        {
            return new(this);
        }
    }
}
