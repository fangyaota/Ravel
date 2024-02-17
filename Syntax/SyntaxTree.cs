using Ravel.Values;

namespace Ravel.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<Diagnostic> diagnostics, CompilationUnitSyntax root)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
        }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }
    }
}
