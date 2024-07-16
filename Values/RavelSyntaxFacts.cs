using Ravel.Syntax;

namespace Ravel.Values
{
    public class RavelSyntaxFacts
    {
        public RavelSyntaxFacts()
        {
            UnaryOperatorPrecedence = new()
            {
                [SyntaxKind.Plus] = 11,
                [SyntaxKind.Minus] = 11,
                [SyntaxKind.Not] = 11,
            };
            BinaryOperatorPrecedence = new()
            {


                [SyntaxKind.ShortCutOr] = 5,
                [SyntaxKind.Or] = 5,


                [SyntaxKind.ShortCutAnd] = 6,
                [SyntaxKind.And] = 6,


                [SyntaxKind.Xor] = 6,

                [SyntaxKind.Large] = 7,
                [SyntaxKind.Small] = 7,
                [SyntaxKind.LargeEqual] = 7,
                [SyntaxKind.SmallEqual] = 7,
                [SyntaxKind.EqualEqual] = 7,
                [SyntaxKind.NotEqual] = 7,
                [SyntaxKind.Is] = 7,

                [SyntaxKind.Plus] = 8,
                [SyntaxKind.Minus] = 8,

                [SyntaxKind.Star] = 9,
                [SyntaxKind.Slash] = 9,
                [SyntaxKind.Percent] = 9,

                [SyntaxKind.StarStar] = 12,

                [SyntaxKind.MinusLarge] = 13,

            };
            BinaryOperatorDirection = new()
            {
                [SyntaxKind.MinusLarge] = OperatorDirection.Right,
            };
            KeywordKind = new()
            {

            };
            SyntaxDict = new()
            {
                ["+"] = SyntaxKind.Plus,
                ["-"] = SyntaxKind.Minus,
                ["*"] = SyntaxKind.Star,
                ["**"] = SyntaxKind.StarStar,
                ["/"] = SyntaxKind.Slash,
                ["%"] = SyntaxKind.Percent,

                ["("] = SyntaxKind.OpenHesis,
                [")"] = SyntaxKind.CloseHesis,
                ["["] = SyntaxKind.OpenBracket,
                ["]"] = SyntaxKind.CloseBracket,
                ["{"] = SyntaxKind.OpenBrace,
                ["}"] = SyntaxKind.CloseBrace,

                ["!"] = SyntaxKind.Not,
                ["&"] = SyntaxKind.And,
                ["|"] = SyntaxKind.Or,
                ["^"] = SyntaxKind.Xor,
                ["&&"] = SyntaxKind.ShortCutAnd,
                ["||"] = SyntaxKind.ShortCutOr,

                ["=="] = SyntaxKind.EqualEqual,
                ["!="] = SyntaxKind.NotEqual,
                [">"] = SyntaxKind.Large,
                ["<="] = SyntaxKind.SmallEqual,
                ["<"] = SyntaxKind.Small,
                [">="] = SyntaxKind.LargeEqual,

                [","] = SyntaxKind.What,
                ["."] = SyntaxKind.Dot,
                [":"] = SyntaxKind.Colon,
                [";"] = SyntaxKind.SemiColon,
                ["="] = SyntaxKind.Equal,

                ["->"] = SyntaxKind.MinusLarge,
                ["=>"] = SyntaxKind.EqualLarge,

                ["#"] = SyntaxKind.Sharp,

                ["++"] = SyntaxKind.PlusPlus,
                ["--"] = SyntaxKind.MinusMinus,
                [".."] = SyntaxKind.DotDot,
                ["$"] = SyntaxKind.Cash,
                ["if"] = SyntaxKind.If,
                ["is"] = SyntaxKind.Is,
                ["for"] = SyntaxKind.For,
                ["while"] = SyntaxKind.While,
                ["using"] = SyntaxKind.Using,
                ["dynamic"] = SyntaxKind.Dynamic,
                ["const"] = SyntaxKind.Const,
                ["readonly"] = SyntaxKind.Readonly,

                ["fuck"] = SyntaxKind.Mad,
                ["dick"] = SyntaxKind.Mad,
                ["ass"] = SyntaxKind.Mad,
                ["hole"] = SyntaxKind.Mad,
            };
            LongestLength = SyntaxDict.Keys.Max(l => l.Length);
        }
        public Dictionary<SyntaxKind, int> UnaryOperatorPrecedence { get; }
        public Dictionary<SyntaxKind, int> BinaryOperatorPrecedence { get; }
        public Dictionary<SyntaxKind, OperatorDirection> BinaryOperatorDirection { get; }
        public Dictionary<string, SyntaxKind> KeywordKind { get; }

        public Dictionary<string, SyntaxKind> SyntaxDict { get; }
        public int LongestLength { get; }
        public int GetUnaryOperatorPrecedence(SyntaxKind kind)
        {
            return UnaryOperatorPrecedence.TryGetValue(kind, out var result) ? result : 0;
        }
        public int GetBinaryOperatorPrecedence(SyntaxKind kind)
        {
            return BinaryOperatorPrecedence.TryGetValue(kind, out var result) ? result : 0;
        }
        public OperatorDirection GetBinaryOperatorDirection(SyntaxKind kind)
        {
            return BinaryOperatorDirection.TryGetValue(kind, out var result) ? result : OperatorDirection.Left;
        }
        public SyntaxKind GetKeywordKind(string text)
        {
            return KeywordKind.TryGetValue(text, out SyntaxKind result) ? result : SyntaxKind.Variable;
        }
    }
    public enum OperatorDirection
    {
        None,
        Left,
        Right,
    }
}
