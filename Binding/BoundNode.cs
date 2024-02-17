namespace Ravel.Binding
{
    public abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }
        public abstract IEnumerable<BoundNode> GetChildren();
        public void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }
        private static void PrettyPrint(TextWriter writer, BoundNode node, string prefix = "", string son = "", string self = "")
        {
            writer.Write(prefix + self + node.Kind);
            if (node is BoundLiteralExpression t)
            {
                writer.Write(": ");
                writer.Write(t.Value);
            }
            writer.WriteLine();
            BoundNode[] l = node.GetChildren().ToArray();
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
