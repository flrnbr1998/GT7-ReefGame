using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class HUDController : MonoBehaviour, IHUDService
{
    [Header("Wellness")]
    [SerializeField] private Slider wellnessSlider;
    [SerializeField] private Text wellnessLabel;

    [Header("Animal Return")]
    [SerializeField] private Text animalReturnLabel;

    [Header("End Screen")]
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private Text endResultLabel;
    [SerializeField] private Text endTimeLabel;
    [SerializeField] private Text endAnimalsLabel;
    [SerializeField] private Button restartButton;

    [Inject] private IWellnessService _wellness;
    [Inject] private IAnimalSpawnService _animals;

    private void Start()
    {
        if (endScreenPanel != null) endScreenPanel.SetActive(false);
        if (animalReturnLabel != null) animalReturnLabel.gameObject.SetActive(false);

        _wellness.OnScoreChanged += OnScoreChanged;
        _animals.OnAnimalSpawned += OnAnimalSpawned;

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartScene);

        OnScoreChanged(_wellness.Score);
    }

    private void OnDestroy()
    {
        _wellness.OnScoreChanged -= OnScoreChanged;
        _animals.OnAnimalSpawned -= OnAnimalSpawned;
    }

    private void OnScoreChanged(float score)
    {
        if (wellnessSlider != null) wellnessSlider.value = score * 0.5f; // 0-2 → 0-1
        if (wellnessLabel != null) wellnessLabel.text = $"Wellness: {score:0.00}";
    }

    private void OnAnimalSpawned(AnimalType type)
    {
        string name = type switch
        {
            AnimalType.Crab     => "Krabbe",
            AnimalType.Turtle   => "Schildkröte",
            AnimalType.MantaRay => "Mantarochen",
            AnimalType.Shark    => "Hai",
            _                   => type.ToString()
        };
        StartCoroutine(ShowPopup($"{name} ist zurückgekehrt!"));
    }

    public void ShowEndScreen(bool won, float sessionTime, int animalsReturned)
    {
        if (endScreenPanel != null) endScreenPanel.SetActive(true);
        if (endResultLabel  != null) endResultLabel.text  = won ? "Riff gerettet!" : "Riff zerstört";
        if (endTimeLabel    != null) endTimeLabel.text    = $"Zeit: {Mathf.RoundToInt(sessionTime)}s";
        if (endAnimalsLabel != null) endAnimalsLabel.text = $"Tiere zurückgekehrt: {animalsReturned}";
    }

    public void HideEndScreen()
    {
        if (endScreenPanel != null) endScreenPanel.SetActive(false);
    }

    private IEnumerator ShowPopup(string message)
    {
        if (animalReturnLabel == null) yield break;
        animalReturnLabel.text = message;
        animalReturnLabel.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        animalReturnLabel.gameObject.SetActive(false);
    }

    private static void RestartScene()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
