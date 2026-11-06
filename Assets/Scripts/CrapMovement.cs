using UnityEngine;

public class CrapMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f; // Geschwindigkeit der Vorwärtsbewegung
    [SerializeField] private float rotationSpeed = 15.0f; // Geschwindigkeit der Drehung
    [SerializeField] private float detectionDistance = 5.0f; // Entfernung für die Hinderniserkennung

    private bool moving = true;

    void Update()
    {
        DetectAndAvoidObstacles(); 
        if(moving){
            MoveForward();
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
        Debug.DrawRay(transform.position, (transform.right * detectionDistance), Color.blue);

        RaycastHit hit;
        bool forwardHit = Physics.Raycast(forwardRay, out hit, detectionDistance);
        bool leftHit = Physics.Raycast(leftRay, out hit, detectionDistance);
        bool rightHit = Physics.Raycast(rightRay, out hit, detectionDistance);

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
        } else if(leftHit){
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward + transform.right, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        } else if (rightHit){
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward - transform.right, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

