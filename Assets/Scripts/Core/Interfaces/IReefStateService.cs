using System;

public interface IReefStateService
{
    ReefState CurrentState { get; }
    event Action<ReefState> OnReefStateChanged;
}
