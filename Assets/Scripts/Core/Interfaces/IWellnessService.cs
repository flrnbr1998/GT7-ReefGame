using System;

public interface IWellnessService
{
    float Score { get; }
    event Action<float> OnScoreChanged;

    void Recalculate(float coralCount, float trashCount);
}
