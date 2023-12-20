namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day20;
internal abstract class ModuleBase
{
    public required string Name { get; init; }
    public required IReadOnlyList<string> Targets { get; init; }

    public IEnumerable<Pulse> ReceivePulse(Pulse pulse)
    {
        if (pulse.Target != Name)
        {
            throw new InvalidOperationException("Wrong Target");
        }

        return ReceivePulseImpl(pulse);
    }

    protected abstract IEnumerable<Pulse> ReceivePulseImpl(Pulse pulse);
}
