using Spectre.Console;

namespace RoelerCoaster.AdventOfCode.Year2023.Internals;

internal class InputReader
{
    private const string INPUT_DIR = "./Inputs";

    private const string TEST_DIR_NAME = "Test";
    private const string ACTUAL_DIR_NAME = "Actual";


    public async Task<string> GetInputForDay(int day, bool useTestInput)
    {
        string inputDirName;
        if (useTestInput)
        {
            AnsiConsole.MarkupLine("[yellow]Warning: Using test input. The reported solutions will not be correct for the real input![/]");
            inputDirName = TEST_DIR_NAME;
        }
        else
        {
            inputDirName = ACTUAL_DIR_NAME;
        }

        var path = Path.Combine(INPUT_DIR, inputDirName, $"{day:D2}.txt");

        if (!Path.Exists(path))
        {
            AnsiConsole.MarkupLine("[red]Input file not found. Please create the file at:[/]");
            AnsiConsole.Write(new TextPath(Path.GetFullPath(path)));
            AnsiConsole.WriteLine();

            throw new FileNotFoundException();
        }

        return (await File.ReadAllTextAsync(path)).Trim().ReplaceLineEndings();
    }
}
