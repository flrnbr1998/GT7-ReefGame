using System.Collections;
using UnityEngine;
using VContainer;

public class RiffGameLoop : MonoBehaviour
{
    [SerializeField] private WellnessConfig config;

    [Inject] private IWellnessService _wellness;
    [Inject] private ICoralSpawnService _corals;
    [Inject] private ITrashSpawnService _trash;
    [Inject] private IAudioService _audio;
    [Inject] private IColorEffectService _colorEffect;

    private float _coralTimer;
    private float _trashTimer;
    private bool _bleaching;
    private bool _recoloring;

    private float CoralInterval => config.spawnCoralBase + _wellness.Score;
    private float TrashInterval => config.spawnTrashBase - _wellness.Score;

    private void Start()
    {
        if (config == null)
        {
            Debug.LogError("[RiffGameLoop] WellnessConfig not assigned — drag Assets/Settings/WellnessConfig.asset into the Config field.", this);
            return;
        }
        _wellness.OnScoreChanged += OnScoreChanged;
        RenderSettings.fog = true;
        StartCoroutine(FactLoop());
    }

    private void OnDestroy() => _wellness.OnScoreChanged -= OnScoreChanged;

    private void Update()
    {
        if (config == null) return;
        _coralTimer += Time.deltaTime;
        _trashTimer += Time.deltaTime;

        if (_coralTimer >= CoralInterval)
        {
            _coralTimer = 0f;
            _corals.SpawnCoral();
            _wellness.Recalculate(_corals.GetScore(), _trash.GetScore());
        }

        if (_trashTimer >= TrashInterval)
        {
            _trashTimer = 0f;
            _trash.SpawnTrash();
            _wellness.Recalculate(_corals.GetScore(), _trash.GetScore());
        }
    }

    private void OnScoreChanged(float score)
    {
        UpdateFog(score);
        UpdateMusic(score);

        if (score < config.bleachThreshold && !_bleaching)
        {
            _bleaching = true;
            _recoloring = false;
            _colorEffect.TriggerBleach();
        }
        else if (score >= config.bleachThreshold && _bleaching)
        {
            _bleaching = false;
            _colorEffect.StopEffect();
        }

        if (score > config.recolorThreshold && !_recoloring)
        {
            _recoloring = true;
            _bleaching = false;
            _colorEffect.TriggerRecolor();
        }
        else if (score <= config.recolorThreshold && _recoloring)
        {
            _recoloring = false;
            _colorEffect.StopEffect();
        }

        if (score == 0f)
            StartCoroutine(ResetAfterDelay(60f));
    }

    private void UpdateFog(float score)
    {
        float t = Mathf.Clamp01(1f - score);
        RenderSettings.fogDensity = Mathf.Lerp(0.03f, 0.06f, t);
    }

    private void UpdateMusic(float score)
    {
        _audio.SetAmbienceParameter("Toxic", score / 20f);

        string label = score switch
        {
            > 1.9f              => "Value A",
            >= 1.8f             => "Value B",
            >= 1.7f             => "Value C",
            >= 1.6f             => "Value D",
            >= 1.5f             => "Value E",
            >= 1.4f             => "Value F",
            >= 1.3f             => "Value G",
            >= 1.2f             => "Value H",
            >= 1.1f             => "Value I",
            _                   => "Value J",
        };
        _audio.SetMusicParameter("MusicToxic", label);
    }

    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_wellness.Score == 0f)
        {
            foreach (var obj in FindObjectsOfType<TrashPickupHandler>())
                Destroy(obj.gameObject);
        }
    }

    private IEnumerator FactLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            _audio.PlayFact(0);
        }
    }
}
