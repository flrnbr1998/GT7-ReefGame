using System.Collections;
using UnityEngine;

public class SharkMovement : MonoBehaviour
{
    [SerializeField] private Transform target; // Das Zielobjekt, um das sich das Objekt bewegt
    [SerializeField] private float moveSpeed = 1.0f; // Geschwindigkeit der Vorwärtsbewegung
    [SerializeField] private float rotationSpeed = 15.0f; // Geschwindigkeit der Drehung
    [SerializeField] private float detectionDistance = 5.0f; // Entfernung für die Hinderniserkennung
    [SerializeField] private float movementRadius = 10.0f; // Radius, innerhalb dessen sich das Objekt bewegen soll
    [SerializeField] private Terrain terrain; // Referenz zum Terrain
    [SerializeField] private float heightAboveTerrain = 5.0f; // Höhe über dem Terrain

    private bool moving = true;
    private Vector3 randomDirection;

    void Start()
    {
        StartCoroutine(Moving());
        StartCoroutine(MakeSound(1));
    }

    private IEnumerator Moving()
    {
        while (true)
        {
            yield return null;
            DetectAndAvoidObstacles();
            if (moving)
            {
                MoveForward();
                ConstrainMovementWithinRadius();
                MaintainHeightAboveTerrain(); // Neue Methode aufrufen
            }
        }
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void DetectAndAvoidObstacles()
    {
        // Raycasts nach vorne, links und rechts abfeuern
        Ray forwardRay = new Ray(transform.position, transform.forward);
        Ray leftRay = new Ray(transform.position, -transform.right);
        Ray rightRay = new Ray(transform.position, transform.right);

        // Debug-Rays für die Visualisierung im Editor
        Debug.DrawRay(transform.position, transform.forward * detectionDistance, Color.red);
        Debug.DrawRay(transform.position, -transform.right * detectionDistance, Color.green);
        Debug.DrawRay(transform.position, transform.right * detectionDistance, Color.blue);

        RaycastHit hit;
        bool forwardHit = Physics.Raycast(forwardRay, out hit, detectionDistance);
        bool leftHit = Physics.Raycast(leftRay, out hit, (0.5f * detectionDistance));
        bool rightHit = Physics.Raycast(rightRay, out hit, (0.5f * detectionDistance));

        moving = true;
        if (forwardHit)
        {
            moving = false;
            // Hindernis erkannt, nach links oder rechts ausweichen
            if (rightHit && !leftHit)
            {
                // Wenn rechts ein Hindernis ist, aber links frei, nach links ausweichen
                Quaternion targetRotation = Quaternion.LookRotation(transform.forward - transform.right, transform.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else if (!rightHit && leftHit)
            {
                // Wenn links ein Hindernis ist, aber rechts frei, nach rechts ausweichen
                Quaternion targetRotation = Quaternion.LookRotation(transform.forward + transform.right, transform.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                // Wenn beide Seiten Hindernisse haben, nach links oder rechts ausweichen
                Quaternion targetRotation = Quaternion.LookRotation(transform.forward + transform.right, transform.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else if (leftHit)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward + transform.right, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else if (rightHit)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward - transform.right, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void ConstrainMovementWithinRadius()
    {
        if (target == null) return; // Wenn kein Zielobjekt gesetzt ist, nichts tun

        float distanceFromTarget = Vector3.Distance(transform.position, target.position);
        if (distanceFromTarget > movementRadius)
        {
            moving = false;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            directionToTarget.y = 0f;
            transform.rotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
            randomDirection = GetRandomDirection();
        }
        else
        {
            moving = true;
        }
    }

    private Vector3 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        return new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
    }

    private void MaintainHeightAboveTerrain()
    {
        if (terrain != null)
        {
            // Höhe des Terrains an der aktuellen Position des Objekts ermitteln
            float terrainHeight = terrain.SampleHeight(transform.position);
            // Neue Position mit der gewünschten Höhe über dem Terrain setzen
            Vector3 newPosition = transform.position;
            newPosition.y = terrainHeight + heightAboveTerrain;
            transform.position = newPosition;
        }
    }

    private IEnumerator MakeSound(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            AudioManager.instance.PlayOneShot(FMODEvents.SplashPath, this.transform.position);
        }
    }
}
