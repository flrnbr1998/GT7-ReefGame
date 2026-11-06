using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject myTarget;
    [SerializeField]
    private GameObject mySecondTarget;

    private GameObject actualTarget;

    [SerializeField]
    private float speed = 0.2f;
    [SerializeField]
    private float height = 0.05f;

    private float pos = 0.0f;


    [SerializeField]
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        this.actualTarget = myTarget;
    }

    // Update is called once per frame
    void Update()
    {

        

        //Vector3 rotatedOffset = actualTarget.transform.rotation * (offset);

        //transform.position = actualTarget.transform.position + rotatedOffset;

        //transform.rotation = actualTarget.transform.rotation;

        //transform.Rotate();
    }

    public void setSecondTarget(GameObject newTarget)
    {
        this.actualTarget = mySecondTarget;
    }

    public void setFirstTarget(GameObject newTarget)
    {
        this.actualTarget = myTarget;
    }

    public void setPerspektive()
    {

    }
}
