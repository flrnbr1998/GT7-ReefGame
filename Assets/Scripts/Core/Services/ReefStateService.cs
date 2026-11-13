using System;

public class ReefStateService : IReefStateService
{
    public ReefState CurrentState { get; private set; } = ReefState.Degraded;
    public event Action<ReefState> OnReefStateChanged;

    public ReefStateService(IWellnessService wellness)
    {
        wellness.OnScoreChanged += OnScoreChanged;
    }

    private void OnScoreChanged(float score)
    {
        ReefState next = score >= 1.6f ? ReefState.Restored
                       : score >= 1.2f ? ReefState.Recovering
                       : ReefState.Degraded;

        if (next == CurrentState) return;
        CurrentState = next;
        OnReefStateChanged?.Invoke(next);
    }
}
