using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day08;

internal record Node(string Label, string Left, string Right);

internal class Day08 : DayBase
{
    public override int Day => 8;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var sections = input.Sections();

        var instructions = sections[0];

        var nodeMap = sections[1].Lines().Select(ParseNode).ToDictionary(n => n.Label);

        var currentNodeLabel = "AAA";
        var numberOfSteps = 0;

        do
        {
            var node = nodeMap[currentNodeLabel];

            currentNodeLabel = instructions[numberOfSteps % instructions.Length] switch
            {
                'L' => node.Left,
                'R' => node.Right,
                _ => throw new InvalidOperationException("Invalid instruction")
            };

            numberOfSteps++;
        } while (currentNodeLabel != "ZZZ");

        return numberOfSteps.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var sections = input.Sections();

        var instructions = sections[0];

        var nodeMap = sections[1].Lines().Select(ParseNode).ToDictionary(n => n.Label);

        var currentNodeLabels = nodeMap.Keys.Where(l => l.EndsWith('A')).ToList();

        var stepsUntilFirstZ = currentNodeLabels.Select(c =>
            {
                var numberOfSteps = 0;

                var currentNodeLabel = c;
                do
                {
                    var node = nodeMap[currentNodeLabel];

                    currentNodeLabel = instructions[numberOfSteps % instructions.Length] switch
                    {
                        'L' => node.Left,
                        'R' => node.Right,
                        _ => throw new InvalidOperationException("Invalid instruction")
                    };

                    numberOfSteps++;
                } while (!currentNodeLabel.EndsWith('Z'));

                return (long)numberOfSteps;
            })
            .ToList();


        return LCM(stepsUntilFirstZ).ToString();
    }

    private Node ParseNode(string line)
    {
        var matches = Regex.Matches(line, "\\w{3}");

        return new Node(matches[0].Value, matches[1].Value, matches[2].Value);
    }

    private long LCM(List<long> numbers)
    {
        return numbers.Aggregate(MathUtil.LCM);
    }
}
