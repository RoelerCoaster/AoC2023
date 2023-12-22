
namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day20;
internal class ConjunctionModule : ModuleBase
{
    private readonly Dictionary<string, PulseKind> _inputMemory;

    public IReadOnlyDictionary<string, PulseKind> InputMemory => _inputMemory;

    public event EventHandler<string> InputHigh;

    public ConjunctionModule(IEnumerable<string> inputs)
    {
        _inputMemory = inputs.ToDictionary(x => x, _ => PulseKind.Low);
    }

    protected override IEnumerable<Pulse> ReceivePulseImpl(Pulse pulse)
    {
        if (!_inputMemory.ContainsKey(pulse.Source))
        {
            throw new InvalidOperationException("Unknown input");
        }

        if (pulse.Kind is PulseKind.High)
        {
            InputHigh?.Invoke(this, pulse.Source);
        }

        _inputMemory[pulse.Source] = pulse.Kind;

        var outputKind = _inputMemory.Values.All(k => k == PulseKind.High) ? PulseKind.Low : PulseKind.High;

        foreach (var target in Targets)
        {
            yield return new(Name, target, outputKind);
        }
    }
}
