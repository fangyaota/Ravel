using Ravel.Syntax;

namespace Ravel.Values
{

    public sealed class RavelBinaryOperator
    {
        public RavelBinaryOperator(SyntaxKind syntaxKind, RavelBinaryOperatorKind kind, RavelFunction function)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            LeftType = function.Type.GetFuncParametersIndex(0);
            RightType = function.Type.GetFuncParametersIndex(1);
            ResultType = function.Type.GetTypeWhenCall(2);
            Function = function;
        }

        public SyntaxKind SyntaxKind { get; }
        public RavelBinaryOperatorKind Kind { get; }
        public RavelType LeftType { get; }
        public RavelType ResultType { get; }
        public RavelType RightType { get; }

        public RavelFunction Function { get; }


    }
}
