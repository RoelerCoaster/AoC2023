using System.Diagnostics.CodeAnalysis;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day20;
internal class OutputModule : ModuleBase
{
    public bool IsOn { get; private set; } = false;

    [SetsRequiredMembers]
    public OutputModule(string name)
    {
        Name = name;
        Targets = new List<string>();
    }

    protected override IEnumerable<Pulse> ReceivePulseImpl(Pulse pulse)
    {
        if (pulse.Kind == PulseKind.Low)
        {
            IsOn = true;
        }

        yield break;
    }
}
