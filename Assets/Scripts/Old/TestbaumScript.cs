using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class TestbaumScript : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vec = new(1, 0, 0);
        int i = 0;
        bool dir = true;
        while (this != null)
        {
            if (dir)
            {
                this.transform.Translate(vec);
                i++;
                if(i == 5)
                {
                    dir = false;
                }
            }
            else
            {
                this.transform.Translate(-vec);
                i--;
                if(i == -5)
                {
                    dir = true;
                }
            }
        }
        
    }
}
