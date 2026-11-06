using UnityEngine;

public class PickupInputHandler : MonoBehaviour
{
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Camera cam = Camera.main;
        if (cam == null && Camera.allCameras.Length > 0)
            cam = Camera.allCameras[0];
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var handler = hit.collider.GetComponentInParent<TrashPickupHandler>();
            handler?.StartPickup();
        }
    }
}
