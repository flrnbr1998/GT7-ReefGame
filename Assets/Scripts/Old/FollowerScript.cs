using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject myTarget;

    [SerializeField]
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotatedOffset = myTarget.transform.rotation * offset;

        transform.position = myTarget.transform.position + rotatedOffset;

        transform.rotation = myTarget.transform.rotation;
    }
}
