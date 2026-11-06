using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawnerScript : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // Eine Liste der Objekte, die du fallen lassen mÃ¶chtest
    public float spawnInterval = 20f; // Das Intervall, in dem Objekte spawnen
    public float spawnRadius = 5f; // Der Radius innerhalb dessen Objekte spawnen

    private List<GameObject> spawnedObjects = new List<GameObject>();

    private float timer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject SpawnRandomObject()
    {
        // Definiere die HÃ¶he Ã¼ber dem Spielfeld, von der aus die Objekte fallen sollen
        float spawnHeight = 20f; // BeispielhÃ¶he, passe dies entsprechend deiner Szene an

        // Erzeuge eine zufÃ¤llige X- und Z-Position innerhalb des Spawn-Radius
        Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), spawnHeight, Random.Range(-spawnRadius, spawnRadius));

        // WÃ¤hle ein zufÃ¤lliges Objekt aus der Liste aus
        int randomIndex = Random.Range(0, objectsToSpawn.Length);

        // Erzeuge das ausgewÃ¤hlte Objekt an der berechneten Spawn-Position ohne Drehung
        GameObject spawnedObject = Instantiate(objectsToSpawn[randomIndex], spawnPosition, Quaternion.identity);
        spawnedObjects.Add(spawnedObject);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.pickUpTrash, spawnedObject.transform.position);
        return spawnedObject;
    }

    public float getTrashScore()
    {

        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedObjects[i] == null)
            {
                spawnedObjects.RemoveAt(i);
            }
        }
        return spawnedObjects.Count;
    }
    /*
    IEnumerator playAmbientShot() {
        for(;;) {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.choralFact,transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius)));
            yield return new WaitForSeconds(40f);
        }
    }
    */
}

