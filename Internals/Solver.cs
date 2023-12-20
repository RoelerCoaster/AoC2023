using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Solutions;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace RoelerCoaster.AdventOfCode.Year2023.Internals;

internal class Solver
{
    private readonly InputReader _inputReader;
    private readonly DayResolver _dayResolver;
    private readonly int _year;

    public Solver(int year)
    {
        _inputReader = new InputReader();
        _dayResolver = new DayResolver();
        _year = year;
    }

    public async Task Run(int? day)
    {

        AnsiConsole.MarkupLine($"[green] Advent of Code [bold]{_year}[/][/]");
        AnsiConsole.Write(new Rule { Style = Style.Parse("green") });

        if (day.HasValue)
        {
            await RunDay(_dayResolver.Get(day.Value));
        }
        else
        {
            AnsiConsole.WriteLine("Determining the latest active day...");
            AnsiConsole.WriteLine();

            await RunDay(_dayResolver.GetLatest());
        }
    }

    private async Task RunDay(DayBase day)
    {
        AnsiConsole.MarkupLine($"[cyan]Running solutions for day [white]{day.Day}[/].[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.WriteLine("Reading input.");
        var input = await _inputReader.GetInputForDay(day.Day, day.UseTestInput);
        AnsiConsole.WriteLine();

        var table = new Table()
            .AddColumns("Part", "Solution", "Time");

        var solutions = new List<PartSolution>();

        var parts = new[]
                {
                    PartToRun.Part1,
                    PartToRun.Part2
                };

        foreach (var part in parts)
        {


            var partNumber = part switch
            {
                PartToRun.Part1 => 1,
                PartToRun.Part2 => 2,
                _ => throw new InvalidOperationException("Invalid part")
            };

            AnsiConsole.Write(new Rule($"Part {partNumber}"));

            await AnsiConsole.Status()
                .StartAsync($"[orangered1]Running Part {partNumber}...[/]", async ctx =>
                {
                    var solution = await day.RunPart(part, input);

                    if (solution.Exception != null)
                    {
                        AnsiConsole.WriteException(solution.Exception);
                        AnsiConsole.WriteLine();
                    }

                    var table = new Table();

                    table.AddColumn("Solution");
                    table.AddColumn("Time");
                    table.AddRow(SolutionRenderable(solution), new Text(
                        solution.Elapsed?.ToString() ?? "-",
                        Style.Parse(solution.Elapsed.HasValue ? "blue" : "grey")
                    ));

                    AnsiConsole.Write(table);
                    AnsiConsole.WriteLine();
                });

        }
    }

    private IRenderable SolutionRenderable(PartSolution solution)
    {
        switch (solution.Type)
        {
            case SolutionType.Skipped:
                return new Markup("[dim grey]Skipped[/]");
            case SolutionType.Error:
                return new Markup("[red]ERROR[/]");
            case SolutionType.Valid:
                return new Text(solution.Answer!, Style.Parse("green"));
            default:
                throw new NotSupportedException();
        }
    }
}
