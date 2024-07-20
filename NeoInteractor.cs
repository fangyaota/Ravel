using Ravel;
using Ravel.Values;

using Spectre.Console;

using System.Text;

public class NeoInteractor
{
    public List<List<string>> Paras { get; set; } = new();
    public List<string> Lines
    {
        get
        {
            return Paras[Cursor_z];
        }
        set
        {
            Paras[Cursor_z] = value;
        }
    }
    private string CurrentLine
    {
        get
        {
            return Lines[Cursor_y];
        }
        set
        {
            Lines[Cursor_y] = value;
        }
    }
    public int Cursor_x { get; set; }
    public int Cursor_y { get; set; }
    public int Cursor_z { get; set; }
    public bool AutoTab { get; set; } = true;
    public bool OutPut { get; set; } = true;
    private bool ShowSyntaxTree { get; set; } = false;
    private bool ShowProgramTree { get; set; } = false;
    public RavelGlobal Global { get; }
    private Compiler? compiler;

    public NeoInteractor(RavelGlobal global)
    {
        Global = global;
        Paras.Add(new());
    }

    public void Run()
    {
        EvaluateShowRavel();
        Interact();
    }
    public string GetText()
    {
        return string.Join('\n', Lines).EscapeMarkup();
    }
    public string GetRenderText(int middle)
    {
        StringBuilder sb = new();
        
        var left = Math.Clamp(middle - 8, 0, Lines.Count);
        var right = Math.Clamp(left + 16, 0, Lines.Count);
        left = Math.Clamp(right - 16, 0, Lines.Count);
        sb.AppendLine($"------[purple]第{Cursor_z}页[/]------");
        if (Lines.Count == 0)
        {
            sb.AppendLine("   | <空>");
        }
        else
        {
            for (int i = left; i < right; i++)
            {
                sb.Append($"[blue]{i.ToString().PadLeft(3)}|[/] ");
                if (i == Cursor_y)
                {
                    sb.Append("[yellow]");
                    if (Lines[i].Length != 0)
                    {
                        sb.Append(Lines[i][..Cursor_x].EscapeMarkup());
                        if (Lines[i].Length > Cursor_x)
                        {
                            sb.Append("[underline red]");
                            sb.Append(Lines[i][Cursor_x].ToString().EscapeMarkup());
                            sb.Append("[/]");
                            sb.Append(Lines[i][(Cursor_x + 1)..].EscapeMarkup());
                        }
                        else
                        {
                            sb.Append("[underline red] [/]");
                        }
                    }
                    else
                    {
                        sb.Append("[underline red] [/]");
                    }
                    sb.Append("[/]");
                }
                else
                {
                    sb.Append(Lines[i].EscapeMarkup());
                }
                sb.AppendLine();
            }
            if (right - left <= 8)
            {
                sb.AppendJoin('\n', Enumerable.Repeat("   |", 8 - (right - left)));
            }
        }
        return sb.ToString();
    }
    //[MetaCommand("credits", "显示作者")]
    private void EvaluateShowRavel()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("""
             ____                                   ___       
            /\  _`\                                /\_ \      
            \ \ \L\ \      __      __  __     __   \//\ \     
             \ \ ,  /    /'__`\   /\ \/\ \  /'__`\   \ \ \    
              \ \ \\ \  /\ \L\.\_ \ \ \_/ |/\  __/    \_\ \_  
               \ \_\ \_\\ \__/.\_\ \ \___/ \ \____\   /\____\ 
                \/_/\/ / \/__/\/_/  \/__/   \/____/   \/____/ 
                        _____           ___           ___
                       /\  ___\        /'__`\        /'__`\   
                       \ \ \__/       /\ \/\ \      /\ \/\ \  
                        \ \___``\     \ \ \ \ \     \ \ \ \ \ 
                         \/\ \L\ \ __  \ \ \_\ \ __  \ \ \_\ \
                          \ \____//\_\  \ \____//\_\  \ \____/
                           \/___/ \/_/   \/___/ \/_/   \/___/ 
            Ravel 5.0.0 
            XMX制作，欢迎捉虫！
            [yellow link=https://www.github.com/fangyaota/Ravel]点我进Ravel官网[/]
            """);
        Pause();
        AnsiConsole.Clear();
    }
    private void EvaluateOutPut()
    {
        var result = AnsiConsole.Confirm("[yellow]开关显示执行结果吗？[/]", !OutPut);
        OutPut = result;
        AnsiConsole.WriteLine(OutPut ? "已开启显示" : "已关闭显示");
        Console.ReadKey(true);
    }
    private void EvaluateShowSyntaxTree()
    {
        var result = AnsiConsole.Confirm("[yellow]开关显示解析树吗？[/]", !ShowSyntaxTree);
        ShowSyntaxTree = result;
        AnsiConsole.WriteLine(ShowSyntaxTree ? "已开启显示" : "已关闭显示");
        Console.ReadKey(true);
    }

    //[MetaCommand("bind", "显示绑定树")]
    private void EvaluateShowProgramTree()
    {
        var result = AnsiConsole.Confirm("[yellow]开关显示绑定树吗？[/]", !ShowProgramTree);
        ShowProgramTree = result;
        AnsiConsole.WriteLine(ShowProgramTree ? "已开启显示" : "已关闭显示");
    }
    private void EvaluateShowAutoTab()
    {
        var result = AnsiConsole.Confirm("[yellow]开关自动对齐文本吗？[/]", !AutoTab);
        AutoTab = result;
        AnsiConsole.WriteLine(AutoTab ? "已开启自动对齐文本" : "已关闭自动对齐文本");
    }
    private void Interact()
    {
        Dictionary<int, (string description, Action action)> map = new()
        {
            [0] = ("编辑", DynamicInput),
            [1] = ("编译", EvaluateTryCompile),
            [2] = ("退出", EvaluateExit),
            [3] = ("开关结果显示", EvaluateOutPut),
            [4] = ("开关解析树显示", EvaluateShowSyntaxTree),
            [5] = ("开关绑定树显示", EvaluateShowProgramTree),
            [6] = ("显示作者", EvaluateShowRavel),
            [7] = ("开关自动对齐", EvaluateShowAutoTab),
        };
        var prompt = new SelectionPrompt<int>()
            .Title("------主菜单------")
            .EnableSearch()
            .SearchPlaceholderText("[grey]（输入序号快速跳转：）[/]")
            .UseConverter( x => $"{x}: {map[x].description}")
            .AddChoices(map.Keys);
        while (true)
        {
            ClearAndRender();

            var result = AnsiConsole.Prompt(prompt);
            map[result].action();
        }

        
    }

    private static void EvaluateExit()
    {
        Environment.Exit(0);
    }

    private void EvaluateTryCompile()
    {
        compiler = new(
            GetText()
            , Global//, compiler?.Evaluator.CurrentCallStack.Scope
            );
        if (compiler.Diagnostics.Any())
        {
            foreach (Diagnostic diagnostic in compiler.Diagnostics)
            {
                int start = compiler.Source.GetLineIndex(diagnostic.Span.Start);
                int end = compiler.Source.GetLineIndex(diagnostic.Span.End);
                Ravel.Text.TextLine startLine = compiler.Source.Lines[start];
                Ravel.Text.TextLine endLine = compiler.Source.Lines[end];

                AnsiConsole.MarkupLine($"[yellow]{diagnostic}[/]");

                string first = startLine[0, diagnostic.Span.Start - startLine.Start];
                string second = compiler.Source[diagnostic.Span];
                string third = endLine[diagnostic.Span.End - endLine.Start, endLine.LengthIncludingLineBreaking].Replace("\r", "").Replace("\n", "");
                AnsiConsole.MarkupLine($"{first}[underline red]{second}[/]{third}");
            }
            Pause();
            return;
        }
        do
        {
            RavelObject result = compiler.Evaluator.Evaluate();
            if (OutPut)
            {
                AnsiConsole.MarkupLine($"[yellow]==>[/]{result}");
            }
        } while (AnsiConsole.Confirm("重试吗？", false));

    }

    private static void Pause()
    {
        AnsiConsole.MarkupLine("[green]按任意键继续[/]");
        Console.ReadKey(true);
    }

    private void RenderText()
    {
        AnsiConsole.Write(new Panel(GetRenderText(Cursor_y)));
    }
    private void ClearAndRender()
    {
        var h = Console.CursorTop;
        var w = Console.WindowWidth;
        var blank = new string(' ', w);
        AnsiConsole.Cursor.SetPosition(0, 0);
        for (int i = 0;i < h;i++)
        {
            AnsiConsole.Write(blank);
        }
        AnsiConsole.Cursor.SetPosition(0, 0);
        RenderText();
    }
    private void RenderEditTips()
    {
        AnsiConsole.MarkupLine("""
            ------快捷栏------

              [yellow]Ins [/] : 清除该页
              [yellow]PgUp[/] : 上一条记录
              [yellow]PgDn[/] : 下一条记录
              [yellow]Home[/] : 跳至开头
              [yellow]End [/] : 跳至结尾
              [yellow]Del [/] : 向后删除
              [yellow]Esc [/] : 退出编辑
            """);
    }
    private void DynamicInput()
    {
        RenderEditTips();
        Console.CursorVisible = false;
        while (true)
        {
            var input = Console.ReadKey(true);
            if (CursorEdit(input))
            {
                ClearAndRender();
                RenderEditTips();
                continue;
            }
            switch (input.Key)
            {
                case ConsoleKey.Escape:
                    return;
            }
            if (char.IsControl(input.KeyChar))
            {
                continue;
            }
            InsertNewLineIfEmpty();
            CurrentLine = CurrentLine.Insert(Cursor_x, input.KeyChar.ToString());
            Cursor_x++;
            CursorClamp();
            ClearAndRender();
            RenderEditTips();
        }
    }
    private bool InsertNewLineIfEmpty()
    {
        if (Lines.Count == 0)
        {
            Lines.Insert(Cursor_y, "");
            return true;
        }
        return false;
    }
    private bool CursorEdit(ConsoleKeyInfo input)
    {
        switch (input.Key)
        {
            case ConsoleKey.PageUp:
                Cursor_z--;
                CursorClamp();
                return true;
            case ConsoleKey.PageDown:
                if(Cursor_z == Paras.Count - 1 && Lines.Count != 0)
                {
                    Paras.Add(new());
                }
                Cursor_z++;
                CursorClamp();
                return true;
            case ConsoleKey.UpArrow:
                Cursor_y--;
                CursorClamp();
                return true;
            case ConsoleKey.DownArrow:
                Cursor_y++;
                CursorClamp();
                return true;
            case ConsoleKey.LeftArrow:
                Cursor_x--;
                CursorClamp();
                return true;
            case ConsoleKey.RightArrow:
                Cursor_x++;
                CursorClamp();
                return true;
            case ConsoleKey.Insert:
                Lines.Clear();
                CursorClamp();
                return true;
            case ConsoleKey.Backspace:
                if (Cursor_x == 0)
                {
                    if (Cursor_y >= 1)
                    {
                        Lines[Cursor_y - 1] += CurrentLine;
                        Lines.RemoveAt(Cursor_y);
                    }
                }
                else
                {
                    CurrentLine = CurrentLine.Remove(Cursor_x - 1, 1);
                    Cursor_x--;
                }
                CursorClamp();
                return true;
            case ConsoleKey.Delete:
                if (Cursor_x == CurrentLine.Length)
                {
                    if (Cursor_y < Lines.Count - 1)
                    {
                        CurrentLine += Lines[Cursor_y + 1];
                        Lines.RemoveAt(Cursor_y + 1);
                    }
                }
                else
                {
                    CurrentLine = CurrentLine.Remove(Cursor_x, 1);
                }
                CursorClamp();
                return true;
            case ConsoleKey.Enter:
                if (Lines.Count == 0)
                {
                    Lines.Add("");
                }
                else
                {
                    int i = 0;
                    if (AutoTab)
                    {
                        while (i < CurrentLine.Length)
                        {
                            if (CurrentLine[i] != ' ')
                            {
                                break;
                            }
                            i++;
                        }
                    }
                    Lines.Insert(Cursor_y + 1, new string(' ', i));
                    Lines[Cursor_y + 1] += CurrentLine[Cursor_x..];
                    CurrentLine = CurrentLine[..Cursor_x];
                    Cursor_x = i;
                }
                Cursor_y++;
                CursorClamp();
                return true;
            case ConsoleKey.Tab:
                InsertNewLineIfEmpty();
                CurrentLine = CurrentLine.Insert(Cursor_x, "    ");
                Cursor_x += 4;
                return true;
            case ConsoleKey.Home:
                Cursor_x = 0;
                Cursor_y = 0;
                return true;
            case ConsoleKey.End:
                Cursor_y = Lines.Count - 1;
                Cursor_x = CurrentLine.Length;
                return true;
        }
        return false;
    }


    private void CursorClamp()
    {

        Cursor_z = Math.Clamp(Cursor_z, 0, Paras.Count - 1);
        if (Lines.Count == 0)
        {
            Cursor_y = 0;
            Cursor_x = 0;
            return;
        }
        Cursor_y = Math.Clamp(Cursor_y, 0, Lines.Count - 1);
        Cursor_x = Math.Clamp(Cursor_x, 0, CurrentLine.Length);
    }
}