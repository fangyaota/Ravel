using Ravel;
using Ravel.Values;

using Spectre.Console;
using Spectre.Console.Cli;
internal class Program
{
    private static int Main(string[] args)
    {
        var app = new CommandApp();
        app.Configure(config =>
        {
            config.PropagateExceptions();
            config.ValidateExamples();
            config.AddCommand<EditCommand>("edit")
                .WithDescription("编辑新文件");
        });
        app.SetDefaultCommand<EditCommand>();
        return app.Run(args);
    }
    private class EditCommand : Command
    {
        public override int Execute(CommandContext context)
        {
            RavelGlobal global = new();
            NeoInteractor interactor = new(global);
            interactor.Run();
            return 0;
        }
    }
}