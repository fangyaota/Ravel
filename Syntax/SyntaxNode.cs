using Ravel.Text;

namespace Ravel.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
        public abstract TextSpan Span { get; }
        public abstract IEnumerable<SyntaxNode> GetChildren();
        public void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }
        private static void PrettyPrint(TextWriter writer, SyntaxNode node, string prefix = "", string son = "", string self = "")
        {
            writer.Write(prefix + self + node.Kind);
            bool isConsole = writer == Console.Out;
            if (node is SyntaxToken t && t.Kind is not (SyntaxKind.BadToken or SyntaxKind.EndOfLine or SyntaxKind.EndOfFile))
            {
                writer.Write(": ");
                writer.Write(t.Value);

            }
            writer.WriteLine();
            SyntaxNode[] l = node.GetChildren().ToArray();
            for (int i = 0; i < l.Length; i++)
            {
                if (i != l.Length - 1)
                {
                    PrettyPrint(writer, l[i], prefix + son, "|   ", "|---");
                }
                else
                {
                    PrettyPrint(writer, l[i], prefix + son, "    ", "\\---");
                }
            }
        }
        public override string ToString()
        {
            using StringWriter writer = new StringWriter();
            WriteTo(writer);
            return writer.ToString();
        }
    }
}
