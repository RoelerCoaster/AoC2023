using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Solutions;
using Spectre.Console;

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

        var exceptions = new List<Exception>();

        await AnsiConsole.Live(table)
            .StartAsync(async ctx =>
            {
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

                    table.AddRow(partNumber.ToString(), "[orangered1]Running...[/]", "[orangered1]Running...[/]");
                    ctx.Refresh();

                    var solution = await day.RunPart(part, input);

                    WriteSolution(table, solution, partNumber);
                    ctx.Refresh();

                    if (solution.Exception != null)
                    {
                        exceptions.Add(solution.Exception);
                    }
                }
            });

        if (exceptions.Any())
        {
            AnsiConsole.Write(new Rule("[red]Exceptions[/]"));
            exceptions.ForEach(ex =>
            {
                AnsiConsole.WriteException(ex);
                AnsiConsole.WriteLine();
            });
        }
    }

    private void WriteSolution(Table table, PartSolution solution, int partNumber)
    {
        var row = partNumber - 1;

        switch (solution.Type)
        {
            case SolutionType.Skipped:
                table.UpdateCell(row, 1, "[dim grey]Skipped[/]");
                break;
            case SolutionType.Error:
                table.UpdateCell(row, 1, "[red]ERROR[/]");
                break;
            case SolutionType.Valid:
                table.UpdateCell(row, 1, new Text(solution.Answer!, Style.Parse("green")));
                break;
        }
        table.UpdateCell(
            row,
            2,
            new Text(
                    solution.Elapsed?.ToString() ?? "-",
                    Style.Parse(solution.Elapsed.HasValue ? "blue" : "grey")
                )
            );

    }
}
