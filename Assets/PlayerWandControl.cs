using System.Collections;
using System.Collections.Generic;
using TiltFive;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWandControl : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector3 controllerPos;
    public Quaternion controllerRot;
    public float speed;
    public int timesObjectHitWithRay = 0;
    public float rotationSpeed = 10.0f;
    public Terrain terrain;
    public float angle = 0f;
    public GameObject Staubsauger;
    public GameObject Pointer;


    private void Start()
    {
        Staubsauger.SetActive(false);
        Pointer.SetActive(true);
    }

    private void Update()
    {
        if (TiltFive.Wand.TryGetWandDevice(TiltFive.PlayerIndex.One, TiltFive.ControllerIndex.Right, out TiltFive.WandDevice wandDevice))
        {
            controllerPos = TiltFive.Wand.GetPosition();
            controllerRot = TiltFive.Wand.GetRotation();
        }


        if (TiltFive.Input.TryGetTrigger(out var TriggerValue))
        {
            // Raycast durchf�hren
            if (TriggerValue > 0.5)
            {
                if (Pointer.activeSelf)
                {
                    Ray ray = new Ray(controllerPos, controllerRot * Vector3.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        GameObject hitObject = hit.collider.gameObject;
                        Debug.Log("Controller zeigt auf: " + hitObject.name);
                        timesObjectHitWithRay++;
                        DestroyObject objectToDestroy = hitObject.GetComponent<DestroyObject>();
                        if (objectToDestroy != null)
                        {
                            Debug.Log("Das Objekt hat ein DestroyObject-Skript.");
                            StartCoroutine(objectToDestroy.DestroyObj());
                        }
                    }
                }
                if (Staubsauger.activeSelf)
                {
                    DestroyAllObjects();
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.saug, this.transform.position);
                    Debug.Log("Staubsauger: Saug");
                }
            }
        }

        if (TiltFive.Input.TryGetButtonDown(TiltFive.Input.WandButton.One, out bool buttonOnePressed))
        {
            // Raycast durchf�hren
            if (buttonOnePressed)
            {

                Pointer.SetActive(true);
                Staubsauger.SetActive(false);
            }
        }

        if (TiltFive.Input.TryGetButtonDown(TiltFive.Input.WandButton.Two, out bool buttonTwoPressed))
        {
            // Raycast durchf�hren
            if (buttonTwoPressed)
            {
                Staubsauger.SetActive(true);
                Pointer.SetActive(false);
            }
        }

    }



    public void MovePlayer(InputAction.CallbackContext context)
    {


        float moveHorizontal = context.ReadValue<Vector2>().x;
        angle += moveHorizontal * Time.deltaTime * rotationSpeed;
        Vector3 targetDirection = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        this.transform.rotation = Quaternion.LookRotation(targetDirection);

        this.transform.Translate(context.ReadValue<Vector2>().x * Time.deltaTime * speed, 0, context.ReadValue<Vector2>().y * Time.deltaTime * speed);
        Vector3 newPosition = this.transform.position;
        // Passe die y-Koordinate der neuen Position an die H�he des Terrains an
        newPosition.x = Mathf.Clamp(newPosition.x, 177f, 717f);
        newPosition.z = Mathf.Clamp(newPosition.z, 285f, 707f);

        newPosition.y = GetTerrainHeight(newPosition);

        transform.position = newPosition;
    }

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

    float GetTerrainHeight(Vector3 position)
    {
        if (terrain != null)
        {
            // Konvertiere die Weltposition in Terrain-Koordinaten
            Vector3 terrainPosition = position - terrain.transform.position;
            float height = terrain.SampleHeight(terrainPosition) + terrain.transform.position.y;
            return height;
        }
        else
        {
            Debug.LogError("Terrain reference is missing.");
            return position.y;
        }
    }

    // Neue Methode zum Zerst�ren aller Objekte mit dem DestroyObject-Skript
    public void DestroyAllObjects()
    {
        DestroyObject[] objectsToDestroy = FindObjectsOfType<DestroyObject>();
        foreach (DestroyObject obj in objectsToDestroy)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null && renderer.isVisible)
            {
                StartCoroutine(obj.DestroyObj());
            }

        }
    }
}