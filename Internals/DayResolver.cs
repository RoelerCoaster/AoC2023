using RoelerCoaster.AdventOfCode.Year2023.Solutions;

namespace RoelerCoaster.AdventOfCode.Year2023.Internals;
internal class DayResolver
{
    public DayBase GetLatest()
    {
        var d = AllDays()
                .OrderByDescending(d => d.Day)
                .FirstOrDefault(d => d.IsActive);

        return d ?? throw new InvalidOperationException($"No active days found.");
    }

    public DayBase Get(int day)
    {
        var d = AllDays().FirstOrDefault(d => d.Day == day);

        return d ?? throw new InvalidOperationException($"Implementation for day {day} not found.");
    }

    private List<DayBase> AllDays()
    {
        return GetType()
                .Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(DayBase)) && !t.IsAbstract)
                .Select(Activator.CreateInstance)
                .Where(o => o != null)
                .Cast<DayBase>()
                .ToList();
    }
}
