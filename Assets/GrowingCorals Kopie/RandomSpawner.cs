using System.Collections;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject[] objectToSpawn;  // Prefab for the object to spawn
    public int nObjectsToSpawn; //Number of Objects to Spawn

    private void Start()
    {
        //StartCoroutine(SpawnObject());
    }

    public GameObject SpawnCoral()
    { 
        Vector3 randomPosition = GetRandomPositionOnPlane(); // Get random position on the plane
        GameObject spawnedObject = Instantiate(objectToSpawn[UnityEngine.Random.Range(0, objectToSpawn.Length)], randomPosition, Quaternion.identity);
        
        if (spawnedObject == null)
        {
            Debug.LogError("Failed to instantiate objectToSpawn.");
        }
        else
        {
            Debug.Log("Object instantiated successfully.");
            Debug.Log($"Object position: {spawnedObject.transform.position}");

            // Ensure the object is visible
            MeshRenderer meshRenderer = spawnedObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
            }
            else
            {
                Debug.LogWarning("MeshRenderer component not found on spawned object.");
            }
        }
        return spawnedObject;
    }

    public IEnumerator SpawnObject()
    {
        int i = 0;
        while (i < nObjectsToSpawn)
        {
            yield return new WaitForSeconds(Random.Range(1f, 10f)); // Wait for a random time interval between 1 and 10 seconds
            Vector3 randomPosition = GetRandomPositionOnPlane(); // Get random position on the plane

            GameObject spawnedObject = Instantiate(objectToSpawn[UnityEngine.Random.Range(0, objectToSpawn.Length)], randomPosition, Quaternion.identity);
            if (spawnedObject == null)
            {
                Debug.LogError("Failed to instantiate objectToSpawn.");
            }
            else
            {
                Debug.Log("Object instantiated successfully.");
                Debug.Log($"Object position: {spawnedObject.transform.position}");

                // Ensure the object is visible
                MeshRenderer meshRenderer = spawnedObject.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = true;
                }
                else
                {
                    Debug.LogWarning("MeshRenderer component not found on spawned object.");
                }
            }
            i++;
        }
    }

    private Vector3 GetRandomPositionOnPlane()
    {
        // Replace this with your logic to get a random position on your plane
        float x = Random.Range(-10f, 10f);
        float z = Random.Range(-10f, 10f);
        return transform.position + new Vector3(x, 0, z);
    }
}
