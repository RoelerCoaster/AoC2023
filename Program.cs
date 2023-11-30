using RoelerCoaster.AdventOfCode.Year2023.Internals;
using Spectre.Console;

int? day = null;

if (args.Length > 0)
{
    day = int.Parse(args[0]);
}

try
{
    await new Solver(2023).Run(day);
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
}

AnsiConsole.WriteLine("Press any key to quit.");
Console.ReadLine();