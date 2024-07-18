namespace Ravel.Values
{
    public enum RavelBinaryOperatorKind
    {
        Unknown,
        Addition,
        Subtraction,
        Multiplication,
        Division,

        And,
        Or,
        ShortCutAnd,
        ShortCutOr,
        LargeComparision,
        SmallComparision,
        LargeEqualComparision,
        SmallEqualComparision,
        EqualComparision,
        NotEqualComparision,
        TypeIs,
        Power,
        Mod,
        Point,
        As,
    }
}
