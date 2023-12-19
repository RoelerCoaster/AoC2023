using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day19;

internal class Day19 : DayBase
{
    public override int Day => 19;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var sections = input.Sections();

        var workFlows = sections[0]
            .Lines()
            .Select(ParseWorkflow)
            .ToDictionary(x => x.Name);

        var parts = sections[1]
            .Lines()
            .Select(ParseMachinePart)
            .ToList();

        var accepted = parts.Where(p => IsAccepted(p, workFlows));

        return accepted.Sum(p => p.TotalRating).ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var sections = input.Sections();

        var workFlows = sections[0]
            .Lines()
            .Select(ParseWorkflow)
            .ToDictionary(x => x.Name);

        var initialRanges = new Dictionary<char, Range>
        {
            {'x', new(1, 4000) },
            {'m', new(1, 4000) },
            {'a', new(1, 4000) },
            {'s', new(1, 4000) }
        };

        var combinations = GetCombinations(initialRanges, "in", workFlows);

        return combinations.ToString();
    }

    private bool IsAccepted(MachinePart part, Dictionary<string, Workflow> workFlows)
    {
        var currentFlow = "in";

        while (currentFlow is not "A" and not "R")
        {
            currentFlow = workFlows[currentFlow].GetNext(part);
        }

        return currentFlow is "A";
    }

    private Workflow ParseWorkflow(string line)
    {
        var split = line.Split('{');

        var name = split[0];

        var rules = line.TrimEnd('}')
            .Split(',')
            .Select(ParseRule)
            .ToList();

        return new(name, rules);
    }

    private Rule ParseRule(string ruleString)
    {
        var split = ruleString.Split(':');
        if (split.Length == 1)
        {
            return new(split[0], null);
        }

        return new(split[1], ParseCondition(split[0]));
    }

    private Condition ParseCondition(string conditionString)
    {
        var match = Regex.Match(conditionString, "(x|m|a|s)(<|>)(\\w+)");

        return new(match.Groups[1].Value[0], match.Groups[2].Value[0], match.Groups[3].Value.ToNumber<int>());
    }

    private MachinePart ParseMachinePart(string line)
    {
        var dict = line.Trim('{', '}')
            .Split(',')
            .Select(x => x.Split('='))
            .ToDictionary(x => x[0][0], x => x[1].ToNumber<int>());

        return new(dict);
    }

    private long GetCombinations(Dictionary<char, Range> xmasRanges, string name, Dictionary<string, Workflow> workFlows)
    {
        if (name == "R")
        {
            return 0;
        }

        if (name == "A")
        {
            return xmasRanges.Values.Product(r => (long)r.Length);
        }

        var workflow = workFlows[name];
        var combinations = 0L;
        foreach (var rule in workflow.Rules)
        {
            if (rule.Condition is null)
            {
                combinations += GetCombinations(new(xmasRanges), rule.Next, workFlows);
                continue;
            }

            var (satisfied, rest) = SplitRangeByCondition(xmasRanges[rule.Condition.Category], rule.Condition);

            if (satisfied.IsValid)
            {
                var copy = new Dictionary<char, Range>(xmasRanges);
                copy[rule.Condition.Category] = satisfied;

                combinations += GetCombinations(copy, rule.Next, workFlows);
            }

            if (!rest.IsValid)
            {
                // we never branch of in the other rules;
                break;
            }

            xmasRanges = new Dictionary<char, Range>(xmasRanges);
            xmasRanges[rule.Condition.Category] = rest;
        }

        return combinations;
    }

    private (Range satisfied, Range rest) SplitRangeByCondition(Range range, Condition condition)
    {
        if (condition.Operator is '<')
        {
            return (
                range with { End = Math.Min(range.End, condition.Value - 1) },
                range with { Start = Math.Max(range.Start, condition.Value) }
            );
        }

        return (
                range with { Start = Math.Max(range.Start, condition.Value + 1) },
                range with { End = Math.Min(range.End, condition.Value) }
            );
    }
}

internal record MachinePart(int X, int M, int A, int S)
{
    public MachinePart(Dictionary<char, int> dict) : this(dict['x'], dict['m'], dict['a'], dict['s'])
    {
    }

    public int this[char label] => label switch
    {
        'x' => X,
        'm' => M,
        'a' => A,
        's' => S,
        _ => throw new NotSupportedException()
    };

    public int TotalRating => X + M + A + S;
}

internal record Condition(char Category, char Operator, int Value)
{
    public bool Check(MachinePart part)
    {
        return Operator switch
        {
            '<' => part[Category] < Value,
            '>' => part[Category] > Value,
            _ => throw new NotSupportedException()
        };
    }
}

internal record Rule(string Next, Condition? Condition);

internal record Workflow(string Name, List<Rule> Rules)
{
    public string GetNext(MachinePart part)
    {
        return Rules.First(r => r.Condition == null || r.Condition.Check(part)).Next;
    }
}

internal record Range(int Start, int End)
{
    public int Length => End - Start + 1;

    public bool IsValid => End >= Start;
}