using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    public Terrain terrain; // Ziehe das Terrain-Objekt hier hinein
    public int textureIndex; // Der Index der Textur, die du überprüfen möchtest

    public GameObject[] corals;
    public GameObject player;
    public float maxDistance = 10; 

    private List<Vector3> positions;
    private List<GameObject> createdCorals = new List<GameObject>(); 
    void Start()
    {
        positions = GetAllTextureCoordinates();
    }



    public List<Vector3> GetAllTextureCoordinates()
    {
        List<Vector3> result = new List<Vector3>();
        // Hole die Terrain-Daten
        TerrainData terrainData = terrain.terrainData;
        int alphamapWidth = terrainData.alphamapWidth;
        int alphamapHeight = terrainData.alphamapHeight;
        int textureCount = terrainData.alphamapLayers;

        // Überprüfe, ob der textureIndex gültig ist
        if (textureIndex < 0 || textureIndex >= textureCount)
        {
            Debug.LogError("Ungültiger Texturindex");
            return null;
        }

        // Hole die Alphamap (Texturen)
        float[,,] alphaMaps = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);

        // Hole die Texturen des Terrains
        SplatPrototype[] splatPrototypes = terrainData.splatPrototypes;

        // Hole den Namen der Textur
        string textureName = splatPrototypes[textureIndex].texture.name;

        // Iteriere über alle Punkte der Alphamap
        for (int x = 0; x < alphamapWidth; x++)
        {
            for (int y = 0; y < alphamapHeight; y++)
            {
                // Überprüfe, ob die Textur an diesem Punkt dominiert
                if (alphaMaps[x, y, textureIndex] >= 0.0f) // hier wird angenommen, dass die Textur dominiert, wenn sie mehr als 50% ausmacht
                {
                    // Umrechnung in Weltkoordinaten
                    float worldX = x * terrainData.size.x / alphamapWidth;
                    float worldZ = y * terrainData.size.z / alphamapHeight;
                    float worldY = terrainData.GetHeight(Mathf.RoundToInt(x * terrainData.heightmapResolution / alphamapWidth), Mathf.RoundToInt(y * terrainData.heightmapResolution / alphamapHeight)); // Höhe an dieser Position

                    Vector3 worldPosition = new Vector3(worldX, worldY, worldZ);
                    
                    // Hier kannst du die Weltkoordinaten und den Texturnamen verwenden
                    //Debug.Log($"Weltkoordinate mit Textur '{textureName}': {worldPosition}");
                    result.Add(worldPosition);
                }
            }
        }
        return result;
    }

    public GameObject SpawnCoral()
    { 
        Vector3 randomPosition = GetRandomPosition(); // Get random position on the plane
        GameObject spawnedObject = Instantiate(corals[UnityEngine.Random.Range(0, corals.Length)], randomPosition, Quaternion.identity);        
        spawnedObject.transform.SetParent(transform);
        if (spawnedObject == null)
        {
            Debug.LogError("Failed to instantiate objectToSpawn.");
        }
        else
        {
            
            //Debug.Log("Object instantiated successfully.");
            // Debug.Log($"Object position: {spawnedObject.transform.position}");
            ISpawnable spawnable = spawnedObject.GetComponent<ISpawnable>();
            if (spawnable != null)
            {
                spawnable.SetStartPosition(randomPosition);
                spawnable.SetParent(spawnedObject);
                createdCorals.Add(spawnedObject);

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
                return spawnedObject;
            }
            else Debug.Log("No implementation of ISpawnable");
        }
        return null;  
    }


    private Vector3 GetRandomPositionOnPlane()
    {
        int randomIndex = UnityEngine.Random.Range(0, positions.Count);
        return positions[randomIndex];
    }

    private Vector3 GetRandomPosition(){
        
        Vector3 playerPosition = player.transform.position;
        float maxDistanceSquared = maxDistance * maxDistance; // Quadratische Entfernung zur Vermeidung von Wurzelberechnungen

        // Filtere die Punkte nach Entfernung zur Kamera und berechne quadratische Entfernung
        List<Vector3> nearbyPoints = positions
            .Where(point => (point - playerPosition).sqrMagnitude <= maxDistanceSquared)
            .ToList();

        if (nearbyPoints.Count > 0)
        {
            // Wähle zufällig einen Punkt aus den gefilterten Punkten
            
            Vector3 randomPoint = nearbyPoints[Random.Range(0, nearbyPoints.Count)];
            //Debug.Log("Zufälliger Punkt in der Nähe: " + randomPoint);
            return randomPoint;
        }
        else
        {   
            Debug.Log("Kein zufälliger Punkt in der Nähe: " + playerPosition);
            return GetRandomPositionOnPlane();
        }
    }

    public float GetCoralScore(){
        for (int i = createdCorals.Count - 1; i >= 0; i--)
        {
            if (createdCorals[i] == null)
            {
                createdCorals.RemoveAt(i);
            }
        }
        return createdCorals.Count;
    }
}
