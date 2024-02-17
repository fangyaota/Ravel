﻿namespace Ravel.Syntax
{
    public enum SyntaxKind
    {
        BadToken,
        EndOfFile,
        EndOfLine,
        WhiteSpace,

        Integer,
        Boolean,
        String,
        Variable,

        Plus,
        Minus,
        Star,
        Slash,
        Percent,

        And,
        Or,
        ShortCutAnd,
        ShortCutOr,
        Not,

        LargeEqual,
        SmallEqual,
        EqualEqual,
        NotEqual,
        Large,
        Small,

        OpenHesis,
        CloseHesis,
        OpenBracket,
        CloseBracket,
        OpenBrace,
        CloseBrace,

        UnaryExpression,
        BinaryExpression,
        LiteralExpression,
        ParenthesizedExpression,
        DefiningExpression,
        NameExpression,
        FunctionCallExpression,
        StatementExpression,
        BlockExpression,
        CompilationUnitExpression,

        Xor,

        Colon,
        Equal,


        Void,

        Is,
        For,
        While,
        Using,
        AssignmentExpression,
        SemiColon,
        None,
        Dot,
        DotExpression,
        EqualLarge,
        DeclareExpression,
        FunctionDefineExpression,
        StarStar,
        MinusLarge,
        Sharp,
        PlusPlus,
        MinusMinus,
        What,
        DotDot,
        Cash,
        Mad,
        If,
        IfExpression,
        LambdaDefiningExpression,
        WhileExpression,
        Dynamic,
        Const,
        Readonly,
    }
}
