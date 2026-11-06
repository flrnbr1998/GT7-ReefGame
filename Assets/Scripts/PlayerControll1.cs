using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll1 : MonoBehaviour
{
    [SerializeField]
    private float speed = 1;

    private float angle = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        angle += moveHorizontal * Time.deltaTime * speed;

        Vector3 targetDirection = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        transform.rotation = Quaternion.LookRotation(targetDirection);

        transform.position += targetDirection * moveVertical * speed * Time.deltaTime;
    }
}
