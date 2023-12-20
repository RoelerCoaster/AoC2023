namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day20;
internal class FlipFlopModule : ModuleBase
{
    public bool IsOn { get; private set; } = false;

    protected override IEnumerable<Pulse> ReceivePulseImpl(Pulse pulse)
    {
        if (pulse.Kind is PulseKind.High)
        {
            yield break;
        }

        IsOn = !IsOn;

        foreach (var target in Targets)
        {
            yield return new(Name, target, IsOn ? PulseKind.High : PulseKind.Low);
        }
    }
}
