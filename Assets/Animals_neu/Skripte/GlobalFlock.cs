using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public GameObject fishPrefab;
    public GameObject jellyfishPrefab;
    public static int tankSize = 100;

    static int numFish = 100;
    static int numJelly = 20;
    public static GameObject[] allJelly = new GameObject[numJelly];
    public static GameObject[] allFish = new GameObject[numFish];

    void Start()
    {
        for (int i = 0; i < numFish; i++)
        {
            // Zufällige Position innerhalb des Tanks
            Vector3 pos = transform.position +  new Vector3(Random.Range(-tankSize / 5, tankSize / 5),
                                      Random.Range(-tankSize / 5, tankSize / 5),
                                      Random.Range(-tankSize / 5, tankSize / 5));
            allFish[i] = (GameObject)Instantiate(fishPrefab, pos, Quaternion.identity);
            allFish[i].transform.SetParent(transform);
        }

        for (int i = 0; i < numJelly; i++)
        {
            // Zufällige Position innerhalb des Tanks
            Vector3 pos = transform.position +  new Vector3(Random.Range(-tankSize / 4, tankSize / 4),
                                      Random.Range(-tankSize / 4, tankSize / 4),
                                      Random.Range(-tankSize / 4, tankSize / 4));
            allJelly[i] = (GameObject)Instantiate(jellyfishPrefab, pos, Quaternion.identity);
            allJelly[i].transform.SetParent(transform);
        }
    }
}
