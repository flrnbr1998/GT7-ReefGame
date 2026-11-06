using UnityEngine;

public class MoveOnTerrain : MonoBehaviour
{
    public Terrain terrain; // Referenz auf das Terrain
    public float speed = 5f; // Bewegungsgeschwindigkeit

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;


        Vector3 newPosition = transform.position + new Vector3(horizontal, 0, vertical);

        float terrainHeight = terrain.SampleHeight(newPosition) + terrain.GetPosition().y;

        newPosition.y = terrainHeight;

        transform.position = newPosition;
    }
}
