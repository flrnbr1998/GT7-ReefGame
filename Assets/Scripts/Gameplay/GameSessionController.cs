using System.Collections;
using UnityEngine;
using VContainer;

public class GameSessionController : MonoBehaviour
{
    [SerializeField] private float winSustainDuration = 30f;

    [Inject] private IWellnessService _wellness;
    [Inject] private IAnimalSpawnService _animals;
    [Inject] private IReefStateService _reefState;
    [Inject] private IHUDService _hud;

    private float _sessionTime;
    private int _animalsReturned;
    private bool _sessionEnded;
    private Coroutine _loseCoroutine;
    private Coroutine _winCoroutine;

    private void Start()
    {
        _wellness.OnScoreChanged += OnScoreChanged;
        _animals.OnAnimalSpawned  += _ => _animalsReturned++;
        _animals.OnAnimalDespawned += _ => { if (_animalsReturned > 0) _animalsReturned--; };
        _reefState.OnReefStateChanged += OnReefStateChanged;
    }

    private void OnDestroy()
    {
        _wellness.OnScoreChanged -= OnScoreChanged;
        _reefState.OnReefStateChanged -= OnReefStateChanged;
    }

    private void Update()
    {
        if (!_sessionEnded) _sessionTime += Time.deltaTime;
    }

    private void OnScoreChanged(float score)
    {
        if (_sessionEnded) return;

        if (score <= 0f && _loseCoroutine == null)
            _loseCoroutine = StartCoroutine(LoseAfterDelay(60f));
        else if (score > 0f && _loseCoroutine != null)
        {
            StopCoroutine(_loseCoroutine);
            _loseCoroutine = null;
        }
    }

    private void OnReefStateChanged(ReefState state)
    {
        if (_sessionEnded) return;

        if (state == ReefState.Restored && _winCoroutine == null)
            _winCoroutine = StartCoroutine(WinAfterSustain());
        else if (state != ReefState.Restored && _winCoroutine != null)
        {
            StopCoroutine(_winCoroutine);
            _winCoroutine = null;
        }
    }

    private IEnumerator LoseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_wellness.Score <= 0f)
            EndSession(won: false);
    }

    private IEnumerator WinAfterSustain()
    {
        yield return new WaitForSeconds(winSustainDuration);
        if (_reefState.CurrentState == ReefState.Restored)
            EndSession(won: true);
    }

    private void EndSession(bool won)
    {
        if (_sessionEnded) return;
        _sessionEnded = true;
        Time.timeScale = 0f;
        _hud?.ShowEndScreen(won, _sessionTime, _animalsReturned);
    }
}
