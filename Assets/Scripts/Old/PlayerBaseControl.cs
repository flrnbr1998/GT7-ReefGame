using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseControl : MonoBehaviour
{

    [SerializeField]
    private GameObject myPreFab;

    [SerializeField]
    public Material color;

    // Start is called before the first frame update
    void Start()
    {
        GameObject spawnedBase;

        myPreFab.GetComponent<Renderer>().material = color;
        Vector3 pos = new Vector3(this.transform.position.x + 0.5f,
            this.transform.position.y + 0.0f,
            this.transform.position.z + 0.5f);
        spawnedBase = (GameObject) Instantiate(myPreFab, pos, Quaternion.identity);
        spawnedBase.transform.parent = gameObject.transform;

        Vector3 pos2 = new Vector3(this.transform.position.x - 0.5f,
            this.transform.position.y + 0.0f,
            this.transform.position.z - 0.5f);
        spawnedBase = (GameObject) Instantiate(myPreFab, pos2, Quaternion.identity);
        spawnedBase.transform.parent = gameObject.transform;

        Vector3 pos3 = new Vector3(this.transform.position.x - 0.5f,
            this.transform.position.y + 0.0f,
            this.transform.position.z + 0.5f);
        spawnedBase = (GameObject) Instantiate(myPreFab, pos3, Quaternion.identity);
        spawnedBase.transform.parent = gameObject.transform;

        Vector3 pos4 = new Vector3(this.transform.position.x + 0.5f,
            this.transform.position.y + 0.0f,
            this.transform.position.z - 0.5f);
        spawnedBase = (GameObject) Instantiate(myPreFab, pos4, Quaternion.identity);
        spawnedBase.transform.parent = gameObject.transform;



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
