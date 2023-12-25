using csdot;
using MoreLinq;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.MaximumFlow;
using QuikGraph.Algorithms.Search;
using QuikGraph.Predicates;
using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day25;

internal class Day25 : DayBase
{
    public override int Day => 25;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Part1;

    protected override async Task<string> SolvePart1(string input)
    {
        var graph = GetGraph(input);
        var (a, b) = CutGraph(graph, 3);

        return (a * b).ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        throw new NotImplementedException();
    }

    private (int, int) CutGraph(BidirectionalGraph<string, Edge<string>> graph, int expectedFlow)
    {
        var vertices = graph.Vertices.ToList();

        var source = vertices[0];

        Func<Edge<string>, double> capacityFunc = _ => 1;

        EdgeFactory<string, Edge<string>> edgeFactory = (source, target) => new Edge<string>(source, target);

        var reversedEdgeAugmentorAlgorithm = new ReversedEdgeAugmentorAlgorithm<string, Edge<string>>(graph, edgeFactory);
        reversedEdgeAugmentorAlgorithm.AddReversedEdges();

        foreach (var sink in vertices[1..])
        {
            var flow = new EdmondsKarpMaximumFlowAlgorithm<string, Edge<string>>(
                graph,
                capacityFunc,
                edgeFactory,
                reversedEdgeAugmentorAlgorithm);
            flow.Compute(source, sink);

            if (flow.MaxFlow == expectedFlow)
            {
                // lifted from EdmondsKarpMaximumFlowAlgorithm's implementation, as we need
                // the residual graph to find the cut sets
                var residualGraph = new FilteredVertexListGraph<string, Edge<string>, IVertexListGraph<string, Edge<string>>>(
                    graph,
                    _ => true,
                    new ResidualEdgePredicate<string, Edge<string>>(flow.ResidualCapacities).Test
                );

                var bfs = new BreadthFirstSearchAlgorithm<string, Edge<string>>(residualGraph);
                bfs.Compute(source);

                return (bfs.VerticesColors.Count(kp => kp.Value == GraphColor.White), bfs.VerticesColors.Count(kp => kp.Value == GraphColor.Black));
            }
        }

        throw new InvalidOperationException("Min cut not found");
    }

    private BidirectionalGraph<string, Edge<string>> GetGraph(string input)
    {
        var graph = new UndirectedGraph<string, Edge<string>>();

        input.Lines()
            .SelectMany(l =>
            {
                var parts = l.Split(":", StringSplitOptions.TrimEntries);

                return parts[1].Split(" ")
                .Select(e => (parts[0], e));
            })
            .ForEach(edge =>
            {
                graph.AddVerticesAndEdge(new(edge.Item1, edge.Item2));
            });


        return graph.ToBidirectionalGraph();
    }
}
