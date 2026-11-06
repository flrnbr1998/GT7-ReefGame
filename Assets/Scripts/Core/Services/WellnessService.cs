using System;
using UnityEngine;

public class WellnessService : IWellnessService
{
    public float Score { get; private set; } = 1f;
    public event Action<float> OnScoreChanged;

    private readonly WellnessConfig _config;

    public WellnessService(WellnessConfig config)
    {
        _config = config;
    }

    public void Recalculate(float coralCount, float trashCount)
    {
        if (coralCount <= 0 && trashCount <= 0) return;

        float newScore = 1f
            + coralCount * 0.01f * _config.coralValue
            - trashCount * 0.01f * _config.trashValue;

        Score = Mathf.Clamp(newScore, 0f, 2f);
        OnScoreChanged?.Invoke(Score);
    }
}
