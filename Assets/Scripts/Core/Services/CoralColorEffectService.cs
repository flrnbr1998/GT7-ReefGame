using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoralColorEffectService : MonoBehaviour, IColorEffectService
{
    [SerializeField] private int secondsPerCoral = 20;

    private readonly List<IColorable> _colorables = new();
    private bool _effectActive;

    public void Register(IColorable colorable)
    {
        if (!_colorables.Contains(colorable))
            _colorables.Add(colorable);
    }

    public void Deregister(IColorable colorable) => _colorables.Remove(colorable);

    public void TriggerBleach()
    {
        _effectActive = true;
        StartCoroutine(ApplyEffect(bad: true));
    }

    public void TriggerRecolor()
    {
        _effectActive = true;
        StartCoroutine(ApplyEffect(bad: false));
    }

    public void StopEffect() => _effectActive = false;

    private IEnumerator ApplyEffect(bool bad)
    {
        while (_effectActive)
        {
            yield return StartCoroutine(ApplyToAll(bad));
            yield return new WaitForSeconds(10f);
        }
    }

    private IEnumerator ApplyToAll(bool bad)
    {
        var snapshot = new List<IColorable>(_colorables);
        foreach (IColorable colorable in snapshot)
        {
            bool growing = colorable.getIsGrowing();
            if (bad && colorable.getCurrentColor() == colorable.getOriginalColor() && !growing)
                colorable.Bleach();
            else if (!bad && colorable.getCurrentColor() != colorable.getOriginalColor() && !growing)
                colorable.Recolor();

            yield return new WaitForSeconds(secondsPerCoral);
        }
    }
}
