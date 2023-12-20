namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day20;
internal class BroadcasterModule : ModuleBase
{
    protected override IEnumerable<Pulse> ReceivePulseImpl(Pulse pulse)
    {
        foreach (var target in Targets)
        {
            yield return new(Name, target, pulse.Kind);
        }
    }
}
