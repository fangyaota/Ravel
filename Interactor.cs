using Ravel;
using Ravel.Binding;
using Ravel.Syntax;
using Ravel.Text;
using Ravel.Values;

internal class Interactor
{
    private static void PrettyPrint(SyntaxNode node, string prefix = "", string son = "", string self = "")
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(prefix + self + node.Kind);
        if (node is SyntaxToken t && t.Kind is not (SyntaxKind.BadToken or SyntaxKind.EndOfLine or SyntaxKind.EndOfFile))
        {
            Console.Write(": ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(t.Value);

        }
        Console.WriteLine();
        SyntaxNode[] l = node.GetChildren().ToArray();
        for (int i = 0; i < l.Length; i++)
        {
            if (i != l.Length - 1)
            {
                PrettyPrint(l[i], prefix + son, "|   ", "|---");
            }
            else
            {
                PrettyPrint(l[i], prefix + son, "    ", "\\---");
            }
        }
        Console.ResetColor();
    }
    private static void PrettyPrint(BoundNode node, string prefix = "", string son = "", string self = "")
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(prefix + self + node.Kind);
        if (node is BoundLiteralExpression t)
        {
            Console.Write(": ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(t.Value);
        }
        Console.WriteLine();
        BoundNode[] l = node.GetChildren().ToArray();
        for (int i = 0; i < l.Length; i++)
        {
            if (i != l.Length - 1)
            {
                PrettyPrint(l[i], prefix + son, "|   ", "|---");
            }
            else
            {
                PrettyPrint(l[i], prefix + son, "    ", "\\---");
            }
        }
        Console.ResetColor();
    }
}
