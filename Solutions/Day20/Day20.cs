using csdot;
using csdot.Attributes.DataTypes;
using MoreLinq;
using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using Spectre.Console;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day20;

internal class Day20 : DayBase
{
    public override int Day => 20;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var modules = ParseModules(input);

        var lowCount = 0;
        var highCount = 0;

        for (var i = 0; i < 1000; i++)
        {
            var (l, h) = SimulateButtonPress(modules);
            lowCount += l;
            highCount += h;
        }

        return (lowCount * highCount).ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var modules = ParseModules(input);

        var rx = modules["rx"] as OutputModule ?? throw new InvalidOperationException();

        /*
         * Analysis of input: after the broadcaster the network splits in 4 subnetworks.
         * 
         * Each subnetwork is then combined into again a conjuction, which feeds the output.
         * 
         * So we have to detect cycles on the the inputs on the terminal conjunctions of the 4 subnetworks.
         *
         */

        var conjunctionBeforeOutput = modules.Values
            .Where(kp => kp is ConjunctionModule && kp.Targets.Contains("rx"))
            .Select(m => (ConjunctionModule)m)
            .Single();

        var pressesFirstHigh = conjunctionBeforeOutput.InputMemory.Keys.ToDictionary(x => x, _ => -1);

        var presses = 0;

        conjunctionBeforeOutput.InputHigh += (_, s) =>
        {
            if (pressesFirstHigh[s] == -1)
            {
                pressesFirstHigh[s] = presses;
            }
        };

        while (pressesFirstHigh.Values.Contains(-1))
        {
            presses++;
            SimulateButtonPress(modules);
        }

        var result = pressesFirstHigh.Values.Select(x => (long)x).Aggregate(MathUtil.LCM);

        return result.ToString();
    }


    private (int LowCount, int HighCount) SimulateButtonPress(Dictionary<string, ModuleBase> modules)
    {
        var queue = new Queue<Pulse>();
        queue.Enqueue(new("button", "broadcaster", PulseKind.Low));

        var lowCount = 0;
        var highCount = 0;
        while (queue.TryDequeue(out var pulse))
        {
            if (pulse.Kind is PulseKind.Low)
            {
                lowCount++;
            }
            else
            {
                highCount++;
            }

            var module = modules[pulse.Target];

            foreach (var newPulse in module.ReceivePulse(pulse))
            {
                queue.Enqueue(newPulse);
            }
        }

        return (lowCount, highCount);
    }

    private Dictionary<string, ModuleBase> ParseModules(string input)
    {
        var splitData = input
            .Lines()
            .Select(line => line.Split(" -> "))
            .Select(split => (
                modulePart: split[0],
                targets: split[1].Split(", ").ToList()
            ))
            .ToList();

        var result = new Dictionary<string, ModuleBase>();

        foreach (var (modulePart, targets) in splitData)
        {
            if (modulePart == "broadcaster")
            {
                result.Add("broadcaster", new BroadcasterModule
                {
                    Name = "broadcaster",
                    Targets = targets
                });
            }
            else if (modulePart.StartsWith('%'))
            {
                var name = modulePart.Substring(1);
                result.Add(name, new FlipFlopModule
                {
                    Name = modulePart.Substring(1),
                    Targets = targets
                });

            }
            else if (modulePart.StartsWith('&'))
            {
                var name = modulePart.Substring(1);
                var sources = splitData
                    .Where(d => d.targets.Contains(name))
                    .Select(d => d.modulePart.TrimStart('%', '&'))
                    .ToList();


                result.Add(name, new ConjunctionModule(sources)
                {
                    Name = name,
                    Targets = targets
                });
            }
        }

        splitData
            .SelectMany(d => d.targets)
            .Distinct()
            .Where(t => !result.ContainsKey(t))
            .ForEach(t => result[t] = new OutputModule(t));

        return result;
    }

    private void PrintDot(Dictionary<string, ModuleBase> modules)
    {
        var graph = new Graph
        {
            type = "digraph"
        };

        var nodes = modules.Values.Select(m =>
        {
            var node = new Node(m.Name);
            node.Attribute.label.Value = $"{m.Name}-{m.GetType().Name}";

            return node;
        });

        var edges = modules.Values.SelectMany(m =>
        {
            var edges = m.Targets.Select(t => new Edge([
                new Transition(m.Name, EdgeOp.directed),
                new Transition(t, EdgeOp.unspecified)
            ])); ;

            return edges;
        });

        graph.AddElements(nodes.ToArray());
        graph.AddElements(edges.ToArray());

        AnsiConsole.WriteLine(graph.ElementToString());
    }
}
