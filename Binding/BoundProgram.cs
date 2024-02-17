namespace Ravel.Binding
{
    public sealed class BoundProgram
    {
        public BoundProgram(BoundExpression expression)
        {
            Expression = expression;
        }

        public BoundExpression Expression { get; }
    }
}
