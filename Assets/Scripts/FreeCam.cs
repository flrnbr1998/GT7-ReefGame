using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCam : MonoBehaviour
{
    public float movementSpeed = 10f; // Speed of the camera movement
    public float mouseSensitivity = 100f; // Sensitivity of the mouse for looking around

    private float pitch = 0f; // Rotation around the X axis
    private float yaw = 0f; // Rotation around the Y axis

    void Start()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Camera movement
        float moveForwardBackward = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float moveLeftRight = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        float moveUpDown = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            moveUpDown = -movementSpeed * Time.deltaTime; // Move down
        }
        else if (Input.GetKey(KeyCode.E))
        {
            moveUpDown = movementSpeed * Time.deltaTime; // Move up
        }

        Vector3 move = transform.right * moveLeftRight + transform.up * moveUpDown + transform.forward * moveForwardBackward;
        transform.position += move;

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Limit the pitch to prevent flipping

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }
}
