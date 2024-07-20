using Ravel.Text;

namespace Ravel.Syntax
{
    internal class FunctionDefiningExpressionSyntax : ExpressionSyntax
    {
        public FunctionDefiningExpressionSyntax(SyntaxToken left, List<DeclareExpressionSyntax> paramList, SyntaxToken right, SyntaxToken? colon, SyntaxToken? typeIdentifier, SyntaxToken equalLarge, ExpressionSyntax sentence)
        {
            Left = left;
            ParamList = paramList;
            Right = right;
            Colon = colon;
            TypeIdentifier = typeIdentifier;
            EqualLarge = equalLarge;
            Sentence = sentence;
            Span = new(Left.Span.Start, Sentence.Span.End);
        }

        public SyntaxToken Left { get; }
        public List<DeclareExpressionSyntax> ParamList { get; }
        public SyntaxToken Right { get; }
        public SyntaxToken? Colon { get; }
        public SyntaxToken? TypeIdentifier { get; }
        public SyntaxToken EqualLarge { get; }
        public ExpressionSyntax Sentence { get; }

        public override SyntaxKind Kind => SyntaxKind.FunctionDefineExpression;

        public override TextSpan Span { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            foreach (DeclareExpressionSyntax param in ParamList)
            {
                yield return param;
            }
            yield return Right;
            if (Colon != null)
            {
                yield return Colon;
            }
            if (TypeIdentifier != null)
            {
                yield return TypeIdentifier;
            }
            yield return EqualLarge;
            yield return Sentence;
        }
    }
}