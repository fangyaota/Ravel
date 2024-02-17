using Ravel.Syntax;

namespace Ravel.Values
{
    public sealed class RavelUnaryOperator
    {
        public RavelUnaryOperator(SyntaxKind syntaxKind, RavelUnaryOperatorKind kind, RavelFunction function)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            OperandType = function.Type.Parameter;
            ResultType = function.Type.ReturnType;
            Function = function;
        }

        public SyntaxKind SyntaxKind { get; }
        public RavelUnaryOperatorKind Kind { get; }
        public RavelType OperandType { get; }
        public RavelType ResultType { get; }
        public RavelFunction Function { get; }

    }
}
