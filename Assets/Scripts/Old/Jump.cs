using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{

    [SerializeField]
    private float speed = 10.0f;
    [SerializeField]
    private float height = 1.0f;

    private float pos = 0.0f;
    private bool jumping = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void jump()
    {
        if (!jumping)
        {
            jumping = true;
        }
        else
        {
            jumping = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            jump();
        }

        if (jumping)
        {
            transform.position += new Vector3(0.0f,  (height/5.0f) * Mathf.Sin(pos), 0.0f);
            pos += speed / 10.0f;
            
            if (pos - 6.28f >= 0.001f)
            {
                jump();
                pos = 0.0f;
            }


        }
    }

    public void test(int m)
    {
        print(m);
    }
}
