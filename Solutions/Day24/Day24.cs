using Microsoft.Z3;
using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day24;

internal class Day24 : DayBase
{
    public override int Day => 24;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        const long lower = 200_000_000_000_000;
        const long higher = 400_000_000_000_000;

        var hailstones = input.Lines().Select(ParseHailstone).ToList();

        var intersections = hailstones.UnorderedPairs(true)
            .Select(p =>
            new
            {
                Stone1 = p.Item1,
                Stone2 = p.Item2,
                Intersection = Intersection(HailstoneToLine(p.Item1), HailstoneToLine(p.Item2))
            })
            .ToList();

        var futureIntersections = intersections
            .Where(i => i.Intersection is not null && PointIsInTheFuture(i.Stone1, i.Intersection) && PointIsInTheFuture(i.Stone2, i.Intersection))
            .Select(i => i.Intersection)
            .ToList();

        var insideArea = futureIntersections
            .Where(p => p is
            {
                X: >= lower and <= higher,
                Y: >= lower and <= higher
            })
            .ToList();

        return insideArea.Count().ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var hailstones = input.Lines().Select(ParseHailstone).ToList();
        var result = GetStoneParameters(hailstones);

        return (result.X + result.Y + result.Z).ToString();
    }

    private HailStone ParseHailstone(string line)
    {
        var parts = line.Split('@');

        var startParts = parts[0].NumbersBySeparator<decimal>(", ");
        var velocityParts = parts[1].NumbersBySeparator<decimal>(", ");


        return new(new(startParts[0], startParts[1], startParts[2]), new(velocityParts[0], velocityParts[1], velocityParts[2]));
    }

    private Line2DByPoints HailstoneToLine(HailStone hailstone)
    {
        return new(
            new(hailstone.Start.X, hailstone.Start.Y),
            new(hailstone.Start.X + hailstone.Velocity.X, hailstone.Start.Y + hailstone.Velocity.Y)
        );
    }

    private Point2D? Intersection(Line2DByPoints first, Line2DByPoints second)
    {
        var (x1, y1) = (first.A.X / 1_000_000, first.A.Y / 1_000_000);
        var (x2, y2) = (first.B.X / 1_000_000, first.B.Y / 1_000_000);
        var (x3, y3) = (second.A.X / 1_000_000, second.A.Y / 1_000_000);
        var (x4, y4) = (second.B.X / 1_000_000, second.B.Y / 1_000_000);


        var denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
        if (denominator == 0)
        {
            return null;
        }

        var x = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4))
            / (denominator);


        var y = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4))
            / (denominator);


        return new(x * 1_000_000, y * 1_000_000);
    }

    private bool PointIsInTheFuture(HailStone hailstone, Point2D point)
    {
        return (point.X - hailstone.Start.X) / hailstone.Velocity.X >= 0;
    }

    private Point3D GetStoneParameters(List<HailStone> hailstones)
    {
        // We build a system of equations where for each hailstone i we add the equations
        // x_i + v_x,i * t_i == x_stone + v_x,stone * t_i
        // y_i + v_y,i * t_i == y_stone + v_y,stone * t_i
        // y_i + v_y,i * t_i == y_stone + v_y,stone * t_i
        // t_i >= 0
        // 
        // Solving this using z3 should give us (x_stone, y_stone, z_stone)
        using var ctx = new Context(new Dictionary<string, string>() { { "model", "true" } });

        var expressions = new List<BoolExpr>();

        var xStone = (IntExpr)ctx.MkConst(ctx.MkSymbol("x_stone"), ctx.IntSort);
        var yStone = (IntExpr)ctx.MkConst(ctx.MkSymbol("y_stone"), ctx.IntSort);
        var zStone = (IntExpr)ctx.MkConst(ctx.MkSymbol("z_stone"), ctx.IntSort);
        var vxStone = (IntExpr)ctx.MkConst(ctx.MkSymbol("vx_stone"), ctx.IntSort);
        var vyStone = (IntExpr)ctx.MkConst(ctx.MkSymbol("vy_stone"), ctx.IntSort);
        var vzStone = (IntExpr)ctx.MkConst(ctx.MkSymbol("vz_stone"), ctx.IntSort);

        var counter = 1;
        foreach (var hailstone in hailstones)
        {
            var t = (IntExpr)ctx.MkConst(ctx.MkSymbol($"t_{counter}"), ctx.IntSort);

            // x_i + v_x,i* t_i == x_stone + v_x,stone* t_i
            var xExpr = ctx.MkEq(
                ctx.MkAdd(
                    ctx.MkInt((long)hailstone.Start.X),
                    ctx.MkMul(
                        ctx.MkInt((long)hailstone.Velocity.X),
                        t
                    )
                ),
                ctx.MkAdd(
                    xStone,
                    ctx.MkMul(
                        vxStone,
                        t
                    )
                )
            );

            // y_i + v_y,i* t_i == y_stone + v_y,stone* t_i
            var yExpr = ctx.MkEq(
                ctx.MkAdd(
                    ctx.MkInt((long)hailstone.Start.Y),
                    ctx.MkMul(
                        ctx.MkInt((long)hailstone.Velocity.Y),
                        t
                    )
                ),
                ctx.MkAdd(
                    yStone,
                    ctx.MkMul(
                        vyStone,
                        t
                    )
                )
            );

            // y_i + v_y,i * t_i == y_stone + v_y,stone * t_i
            var zExpr = ctx.MkEq(
                ctx.MkAdd(
                    ctx.MkInt((long)hailstone.Start.Z),
                    ctx.MkMul(
                        ctx.MkInt((long)hailstone.Velocity.Z),
                        t
                    )
                ),
                ctx.MkAdd(
                    zStone,
                    ctx.MkMul(
                        vzStone,
                        t
                    )
                )
            );

            // t_i >= 0
            var tExpr = ctx.MkGe(t, ctx.MkInt(0));

            expressions.AddRange(new[]
            {
                xExpr, yExpr, zExpr, tExpr
            });

            counter++;
        }


        var solver = ctx.MkSolver();

        solver.Assert(expressions.ToArray());

        if (solver.Check() == Status.SATISFIABLE)
        {
            var model = solver.Model;

            return new(
                ((IntNum)model.Evaluate(xStone)).Int64,
                ((IntNum)model.Evaluate(yStone)).Int64,
                ((IntNum)model.Evaluate(zStone)).Int64
            );
        }
        else
        {
            throw new Exception();
        }
    }
}



internal record Point2D(decimal X, decimal Y);

internal record Point3D(decimal X, decimal Y, decimal Z);

internal record Velocity3D(decimal X, decimal Y, decimal Z);

internal record Line2DByPoints(Point2D A, Point2D B);

internal record HailStone(Point3D Start, Velocity3D Velocity);