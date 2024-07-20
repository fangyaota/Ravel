using Ravel;
using Ravel.Binding;
using Ravel.Syntax;
using Ravel.Values;

internal class Interactor
{
    private bool _outPut = true;
    private bool _showTree = false;
    private bool _showProgram;
    public RavelGlobal Global { get; }
    private Compiler? compiler;

    public Interactor(RavelGlobal global)
    {
        Global = global;
    }
    //[MetaCommand("load", "加载代码")]
    private void EvaluateLoad(string path)
    {
        path = Path.GetFullPath(path);

        if (!File.Exists(path))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"错误：文件不存在 '{path}'");
            Console.ResetColor();
            return;
        }

        string[] text = File.ReadAllLines(path);
        foreach (string line in text)
        {
            //AddHistory(line);
            EvaluateSubmission(line);
        }
    }
    //[MetaCommand("out", "是否输出")]
    

    //[MetaCommand("exit", "退出交互")]
    private void EvaluateExit()
    {
        Environment.Exit(0);
    }

    //[MetaCommand("cls", "清屏")]
    private void EvaluateCls()
    {
        Console.Clear();
    }

    //[MetaCommand("reset", "清除历史记录")]
    private void EvaluateReset()
    {
        compiler = null;
    }

    //[MetaCommand("parse", "显示解析树")]
    

    //[MetaCommand("control", "操作教程")]
    private void EvaluateControl()
    {
        Console.ResetColor();
        Console.WriteLine("""
        
        """
        );
    }
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

    protected bool IsCompleteSubmission(IReadOnlyList<string> text)
    {
        if (text.All(string.IsNullOrEmpty))
            return true;

        bool lastTwoLinesAreBlank = text
                                       .Reverse()
                                       .TakeWhile(string.IsNullOrEmpty)
                                       .Any();

        return lastTwoLinesAreBlank;
    }

    protected void EvaluateSubmission(string text)
    {
        compiler = new Compiler(text, Global, compiler?.Evaluator.CurrentCallStack.Scope);
        if (compiler.Diagnostics.Any())
        {
            foreach (Diagnostic diagnostic in compiler.Diagnostics)
            {
                int start = compiler.Source.GetLineIndex(diagnostic.Span.Start);
                int end = compiler.Source.GetLineIndex(diagnostic.Span.End);
                Ravel.Text.TextLine startLine = compiler.Source.Lines[start];
                Ravel.Text.TextLine endLine = compiler.Source.Lines[end];

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(diagnostic);

                Console.ResetColor();
                Console.Write(startLine[0, diagnostic.Span.Start - startLine.Start]);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(compiler.Source[diagnostic.Span]);

                Console.ResetColor();
                Console.WriteLine(endLine[diagnostic.Span.End - endLine.Start, endLine.LengthIncludingLineBreaking].Replace("\r", "").Replace("\n", ""));
                if (diagnostic.Span.Start >= endLine.Start)
                {
                    Console.Write(new string(' ', diagnostic.Span.Start - endLine.Start));
                }
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.Write(new string('^', diagnostic.Span.Length));
                Console.ResetColor();

                Console.WriteLine();
            }
            return;
        }
        if (_showTree)
        {
            PrettyPrint(compiler.Binder.Tree.Root);
        }
        RavelObject result = compiler.Evaluator.Evaluate();
        if (_outPut)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"==> {result}");
            Console.ResetColor();
        }
        if (_showProgram)
        {
            PrettyPrint(compiler.Evaluator.Root);
        }
    }
    protected object? RenderLine(IReadOnlyList<string> lines, int lineIndex, object? state)
    {
        Lexer lexer = new(new(lines[lineIndex]), Global);
        bool co = false;
        foreach (SyntaxToken i in lexer.Lex())
        {
            if (!co)
            {
                switch (i.Kind)
                {
                    case SyntaxKind.String:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case SyntaxKind.Integer:
                    case SyntaxKind.Boolean:
                    case SyntaxKind.Void:
                    case SyntaxKind.None:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case SyntaxKind.If:
                    case SyntaxKind.For:
                    case SyntaxKind.While:
                    case SyntaxKind.Using:
                    case SyntaxKind.What:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case SyntaxKind.Variable:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case SyntaxKind.Sharp:
                        Console.ForegroundColor = ConsoleColor.Green;
                        co = true;
                        break;
                    default:
                        Console.ResetColor();
                        break;
                }
            }
            Console.Write(i.Text);
        }
        //Console.WriteLine(lines[lineIndex]);
        Console.ResetColor();
        return null;
    }
}
