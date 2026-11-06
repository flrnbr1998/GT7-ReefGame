using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [SerializeField]
    private float speed = 1;

    private float angle = 0;

    // Referenz zum Terrain
    [SerializeField]
    private Terrain terrain;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        angle += moveHorizontal * Time.deltaTime * speed;

        Vector3 targetDirection = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        transform.rotation = Quaternion.LookRotation(targetDirection);

        Vector3 newPosition = transform.position + targetDirection * moveVertical * speed * Time.deltaTime;

        // Passe die y-Koordinate der neuen Position an die Höhe des Terrains an
        newPosition.y = GetTerrainHeight(newPosition);

        transform.position = newPosition;
    }

    // Methode zum Abrufen der Höhe des Terrains an einer bestimmten Position
    float GetTerrainHeight(Vector3 position)
    {
        if (terrain != null)
        {
            // Konvertiere die Weltposition in Terrain-Koordinaten
            Vector3 terrainPosition = position - terrain.transform.position;
            float height = terrain.SampleHeight(terrainPosition) + terrain.transform.position.y;
            return height;
        }
        else
        {
            Debug.LogError("Terrain reference is missing.");
            return position.y;
        }
    }
}
