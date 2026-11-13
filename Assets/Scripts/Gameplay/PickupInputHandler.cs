using UnityEngine;

public class PickupInputHandler : MonoBehaviour
{
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Camera cam = PS_TiltFive.Core.CameraHelper.Main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var handler = hit.collider.GetComponentInParent<TrashPickupHandler>();
            handler?.StartPickup();
        }
    }
}
