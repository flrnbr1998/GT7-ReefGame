using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiffController : MonoBehaviour
{
    // Start is called before the first frame update
    public TerrainSpawner terrainSpawner;
    public TrashSpawnerScript trashSpawner;
    public float wellnesScore = 0;
    public float spawnTrashInterval;
    public float spawnCoralInterval;
    public float factInterval = 60f;
    public float trashValue = 6f;
    public float coralValue = 1f;
    private float trashTimer = 0f;
    private float coralTimer = 0f;

    public float spawnCoralIntervalCalculated = 8.0f;
    public float spawnTrashIntervalCalculated = 16.0f;

    public float trashScore;
    public float coralScore;

    public bool recoloring = false;
    public bool bleaching = false;

    void Start()
    {

        // Initialisiere Nebel
        RenderSettings.fog = true;
        UpdateFogSettings();
        wellnesScore = 1f; 
        StartCoroutine(PlayChoralFact(factInterval));

    }

    private void calculateWellnessScore()
    {
        coralScore = terrainSpawner.GetCoralScore();
        trashScore = trashSpawner.getTrashScore();
        if (coralScore > 0 && trashScore > 0)
        {
            wellnesScore = 1f + (coralScore * (0.01f * coralValue) - trashScore * (0.01f * trashValue));
            

            if(wellnesScore < 0)
            {
                wellnesScore = 0;
            }

            if (wellnesScore > 2)
            {
                wellnesScore = 2;
            }

            spawnCoralIntervalCalculated = spawnCoralInterval + wellnesScore;
            spawnTrashIntervalCalculated = spawnTrashInterval - wellnesScore;

            if (wellnesScore < 0.8 && !bleaching)
            {
                bleaching = true;
                CoralColorManager.SetisBleachingOrRecoloring(bleaching);
                Debug.Log("Bleaching Triggered in Controller");
                StartCoroutine(CoralColorManager.Instance.TriggerColorChange(true));
            }
            if (wellnesScore > 0.8 && bleaching)
            {
                bleaching = false;
                CoralColorManager.SetisBleachingOrRecoloring(bleaching);
            }
            if (wellnesScore > 1.1 && !recoloring)
            {
                recoloring = true;
                CoralColorManager.SetisBleachingOrRecoloring(recoloring);
                Debug.Log("Recoloring Triggered in Controller");
                StartCoroutine(CoralColorManager.Instance.TriggerColorChange(false));
            }
            if (wellnesScore < 1.1 && recoloring)
            {
                recoloring = false;
                CoralColorManager.SetisBleachingOrRecoloring(recoloring);
            }



            if (wellnesScore == 0)
            {
                DestroyObjectsAfterDelay(60);
            }
            
            // Update Nebeleinstellungen basierend auf dem Wellness Score
            UpdateFogSettings();

            // Musikparameter setzen
            AudioManager.instance.SetAmbienceParameter("Toxic", wellnesScore / 20);
            switch (wellnesScore)
            {
                case > 1.9f:
                    AudioManager.instance.SetMusicParameter("MusicToxic", "Value A");
                    AudioManager.instance.setFactNum(0);
                    break;
                case >= 1.8f and < 1.9f:
                    AudioManager.instance.SetMusicParameter("MusicToxic", "Value B");
                    AudioManager.instance.setFactNum(0);
                    break;
                case >= 1.7f and < 1.8f:
                    AudioManager.instance.SetMusicParameter("MusicToxic", "Value C");
                    AudioManager.instance.setFactNum(1);
                    break;
                case >= 1.6f and < 1.7f:
                    AudioManager.instance.SetMusicParameter("MusicToxic", "Value D");
                    AudioManager.instance.setFactNum(2);
                    break;
                case >= 1.5f and < 1.6f:
                    AudioManager.instance.SetMusicParameter("MusicToxic", "Value E");
                    AudioManager.instance.setFactNum(3);
                    break;
                case >= 1.4f and < 1.5f:
                    AudioManager.instance.SetMusicParameter("MusicToxic", "Value F");
                    AudioManager.instance.setFactNum(3);
                    break;
                case >= 1.3f and < 1.4f:
                    AudioManager.instance.SetMusicParameter("MusicToxic", "Value G");
                    AudioManager.instance.setFactNum(4);
                    break;
                case >= 1.2f and < 1.3f:
                    AudioManager.instance.SetMusicParameter("MusicToxic", "Value H");
                    AudioManager.instance.setFactNum(5);
                    break;
                case >= 1.1f and < 1.2f:
                    AudioManager.instance.SetMusicParameter("MusicToxic", "Value I");
                    AudioManager.instance.setFactNum(5);
                    break;
                case >= 1.0f and < 1.1f:
                    AudioManager.instance.SetMusicParameter("MusicToxic", "Value J");
                    AudioManager.instance.setFactNum(5);
                    break;
                case < 1.0f:
                    AudioManager.instance.SetMusicParameter("MusicToxic", "Value J");
                    AudioManager.instance.setFactNum(5);

                    break;
            }

        }
        // Debug.Log("Trashscore: " + trashScore);
        AudioManager.instance.SetAmbienceParameter("Toxic", trashScore / 20);
    }

    // Update is called once per frame
    void Update()
    {
        coralTimer += Time.deltaTime;
        trashTimer += Time.deltaTime;
        if (coralTimer >= spawnCoralIntervalCalculated)
        {
            coralTimer = 0f;
            terrainSpawner.SpawnCoral();
            calculateWellnessScore();
        }
        if (trashTimer >= spawnTrashInterval)
        {
            trashTimer = 0f;
            trashSpawner.SpawnRandomObject();
            calculateWellnessScore();
        }
    }




    private void UpdateFogSettings()
    {
        // Beispielhafte Anpassung des Nebels basierend auf dem Wellness Score
        float lerpValue = Mathf.Clamp01(1f - wellnesScore); // Umkehrung des Wellness Scores (0 wird 1, 1 wird 0)


        RenderSettings.fogDensity = Mathf.Lerp(0.03f, 0.06f, lerpValue); // Anpassung der Nebeldichte


    }


    private IEnumerator DestroyObjectsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (wellnesScore == 0)
        {
            DestroyAllDestroyObjects();
            Debug.Log("All objects with DestroyObject script destroyed after delay");
            wellnesScore = 1f;
        }
    }

    private void DestroyAllDestroyObjects()
    {
        DestroyObject[] destroyObjects = FindObjectsOfType<DestroyObject>();
        foreach (DestroyObject obj in destroyObjects)
        {
            Destroy(obj.gameObject);
        }
    }
    private IEnumerator PlayChoralFact(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            AudioManager.instance.PlayFact(FMODEvents.instance.choralFact);
        };
    }

    // Kann raus wenn wir nicht mehr bleaching / recoloring debuggen müssen
    private IEnumerator SetWellnessScoreAfterDelay(bool stopBleaching)
    {
        Debug.Log("Set Wellness score in 300 seconds");
        yield return new WaitForSeconds(300);
        if (stopBleaching)
        {
            this.wellnesScore = 10f;
            Debug.Log("Wellness score set to 10 after delay");
        }
        else if (!stopBleaching)
        {
            this.wellnesScore = 0.9f;
            Debug.Log("Wellness score set to 0.9 after delay");
        }
    }
}
