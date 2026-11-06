using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class TokenSpawner : MonoBehaviour
{

    private List<GameObject> gs;

    [SerializeField]
    private GameObject token;

    private GameObject currToken;
    

    public GameObject getCurr()
    {
        return currToken;
    } 

    // Start is called before the first frame update
    void Start()
    {
        gs = new List<GameObject>();

        Transform[] ts = this.GetComponentsInChildren<Transform>();
        if (ts == null)
        {
            return;
        }

       

        Vector3 fieldPos;
        foreach (Transform t in ts) {


            if (t != null && t.gameObject != null)
            {
                gs.Add(t.gameObject);
                if (t.gameObject.GetComponent<Renderer>() != null) {
                    fieldPos = t.gameObject.GetComponent<Renderer>().transform.position;
                    Vector3 pos = new Vector3(fieldPos.x,
                        fieldPos.y + 0.5f,
                        fieldPos.z);
                    if (t.gameObject.GetComponent<Renderer>().tag == "StartField") {

                        Material tempM = t.gameObject.GetComponent<Renderer>().material;
                        if (tempM != null)
                        {
                            print(tempM);
                            Transform parts = token.GetComponentInChildren<Transform>();
                            token.GetComponent<Renderer>().material = tempM;
                            
                            foreach(Transform p in parts){
                                p.gameObject.GetComponent<Renderer>().material = tempM;
                            } 

                        }
                        GameObject instance = (GameObject)Instantiate(token, pos, Quaternion.identity);
                        instance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        instance.transform.parent = gameObject.transform;
                    }
                }
            }
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
