using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject myPreFab;

    [SerializeField]
    private int nCollect = 10;

    private List<GameObject> list;

    private int i = 0;


    private float pos = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        list = new List<GameObject>();

        for (int i = 0; i < nCollect; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-10f, 10f),
               1.0f,
               Random.Range(-10f, 10f));
            GameObject coll = (GameObject) Instantiate(myPreFab, pos, Quaternion.identity);
            print(coll.activeSelf);
            list.Add(coll);
            
        }
    }

    void Update()
    {
       

    }

}
