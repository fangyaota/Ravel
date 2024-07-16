
using Ravel.Binding;
using Ravel.Rewrite;
using Ravel.Syntax;
using Ravel.Text;
using Ravel.Values;

namespace Ravel
{
    public class Compiler
    {
        private readonly DiagnosticList _diagnostics;
        public Compiler(string text, RavelGlobal global, RavelScope? scope = null)
        {
            Text = text;
            Global = global;

            Source = new(text);

            _diagnostics = new(Source);

            Lexer = new(Source, Global);
            var lex = Lexer.Lex().ToArray();

            _diagnostics.AddRange(Lexer.Diagnostics);

            Parser = new(Source, lex, Global);
            var parse = Parser.Parse();

            _diagnostics.AddRange(Parser.Diagnostics);

            Binder = new(Source, parse, Global, scope);
            var bound = Binder.Bind();

            _diagnostics.AddRange(Binder.Diagnostics);

            Flatter flatter = new(bound, Global);
            bound = flatter.Rewrite();

            Evaluator = new(bound, Global, scope);
        }
        public IEnumerable<Diagnostic> Diagnostics
        {
            get
            {
                return _diagnostics;
            }
        }
        public string Text { get; }
        public RavelGlobal Global { get; }
        public SourceText Source { get; }
        public Lexer Lexer { get; }
        public Parser Parser { get; }
        public Binder Binder { get; }
        public NeoEvaluator Evaluator { get; }
    }
}
