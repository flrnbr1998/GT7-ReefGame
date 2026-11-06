using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [SerializeField] public float speed = 1.0f;
    [SerializeField] float rotationSpeed = 2.0f;
    [SerializeField] float neighbourDistance = 5.0f;
    bool turning = false;

    //[SerializeField] private float detectionDistance = 5.0f; // Entfernung für die Hinderniserkennung

    void Start()
    {
        // speed = Random.Range(1.0f, 2.0f);
    }

    void Update()
    {
        // DetectAndAvoidObstacles(); // Hinderniserkennung und -vermeidung

        if (Vector3.Distance(transform.position, Vector3.zero) >= GlobalFlock.tankSize)
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            Vector3 direction = Vector3.zero - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(direction),
                                                  rotationSpeed * Time.deltaTime);
            speed = Random.Range(1.0f, 2.0f);
        }
        else
        {
            if (Random.Range(0.0f, 1.0f) < 0.1f) // Reduziert die Wahrscheinlichkeit, Regeln anzuwenden
                ApplyRules();
        }

        //transform.Translate(0, 0, Time.deltaTime * speed);
    }

    void ApplyRules()
    {
        GameObject[] gos;
        GameObject[] gosJ;
        gosJ = GlobalFlock.allJelly;
        gos = GlobalFlock.allFish;

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.1f;

        float dist;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if (dist < 1.0f)
                    {
                        vavoid += (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed += anotherFlock.speed;
                }
            }
        }

        foreach (GameObject go in gosJ)
        {
            if (go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if (dist < 1.0f)
                    {
                        vavoid += (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed += anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            vcentre = vcentre / groupSize;
            vcentre += (Vector3.zero - this.transform.position) * 0.5f; // Stärker zum Zentrum ausrichten
            speed = Mathf.Lerp(speed, gSpeed / groupSize, Time.deltaTime * 0.5f); // Geschwindigkeit sanfter anpassen

            Vector3 direction = (vcentre + vavoid) - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(direction),
                                                      rotationSpeed * Time.deltaTime * 0.5f); // Sanftere Drehung
            }
        }
    }
}
