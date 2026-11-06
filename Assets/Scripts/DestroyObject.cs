using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TiltFive;

public class DestroyObject : MonoBehaviour
{
    public float speed = 20f; // Geschwindigkeit des Objekts
    private bool isFlying = false; // Flag, ob das Objekt fliegt
    public Camera mainCamera; // Referenz zur Hauptkamera
    private Rigidbody rb;

    void Start()
    {
        SetMainCamera(); // Hauptkamera initialisieren
        rb = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        StartCoroutine(DestroyObj());
    }

    public IEnumerator DestroyObj()
    {
        isFlying = true; // Objekt soll fliegen, wenn es angeklickt wird
        if (rb != null)
        {
            rb.isKinematic = true; // Physik deaktivieren
        }

        while (isFlying)
        {
            if (mainCamera == null)
            {
                SetMainCamera(); // Versuche, die Kamera erneut zu setzen, falls sie null ist
                if (mainCamera == null) yield return null; // Beende, wenn keine Kamera gefunden wird
            }

            if (Wand.TryGetWandDevice(PlayerIndex.One, ControllerIndex.Right, out WandDevice wand))
            {
                // Position und Rotation des Controllers abrufen
                Vector3 controllerPosition = wand.devicePosition.ReadValue();
                Debug.Log("Controller Position: " + controllerPosition);
                Vector3 wandDirection = (controllerPosition - transform.position).normalized;
                transform.position += wandDirection * speed * Time.deltaTime;
            }
            else
            {
                // Bewege das Objekt in Richtung der Kamera
                Vector3 direction = (mainCamera.transform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
            }

            // Objekt schrumpfen lassen
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime);

            // Prüfen, ob das Objekt nicht mehr im Sichtfeld der Kamera ist
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            if (!onScreen || transform.localScale.magnitude < 0.01f) // Wenn das Objekt zu klein wird, zerstören
            {
                Destroy(gameObject); // Objekt zerstören
                isFlying = false;
                yield return null;
            }

            yield return null;
        }
    }

    void SetMainCamera()
    {
        mainCamera = Camera.allCameras[0];
    }
}
