namespace Ravel.Syntax
{
    internal static class Utils
    {
        internal static bool IsVariableKeyword(this SyntaxKind Kind)
        {
            return Kind is
                SyntaxKind.Dynamic or
                SyntaxKind.Const or
                SyntaxKind.Readonly;
        }
        internal static bool IsEnd(this SyntaxKind kind)
        {
            return kind is
                SyntaxKind.EndOfFile or
                SyntaxKind.CloseHesis or
                SyntaxKind.CloseBracket or
                SyntaxKind.CloseBrace or
                SyntaxKind.SemiColon;
        }
        internal static bool IsEndOrLine(this SyntaxKind kind)
        {
            return kind is
                SyntaxKind.EndOfLine or
                SyntaxKind.EndOfFile or
                SyntaxKind.CloseHesis or
                SyntaxKind.CloseBracket or
                SyntaxKind.CloseBrace or
                SyntaxKind.SemiColon;
        }
        internal static bool IsStart(this SyntaxKind kind)
        {
            return kind is
                SyntaxKind.OpenHesis or
                SyntaxKind.OpenBracket or
                SyntaxKind.OpenBrace;
        }
    }
}
