using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private Terrain terrain;

    private float _angle;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical   = Input.GetAxis("Vertical");

        _angle += horizontal * speed * Time.deltaTime;

        Vector3 direction = new Vector3(Mathf.Sin(_angle), 0f, Mathf.Cos(_angle));
        transform.rotation = Quaternion.LookRotation(direction);

        Vector3 newPos = transform.position + direction * vertical * speed * Time.deltaTime;
        newPos.y = SampleTerrainHeight(newPos);
        transform.position = newPos;
    }

    private float SampleTerrainHeight(Vector3 position)
    {
        if (terrain == null) return position.y;
        Vector3 local = position - terrain.transform.position;
        return terrain.SampleHeight(local) + terrain.transform.position.y;
    }
}
