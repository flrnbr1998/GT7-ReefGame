using UnityEngine;

public class PCInputService : IInputService
{
    public bool IsPickupTriggered() => Input.GetMouseButtonDown(0);

    public Ray GetPointerRay()
    {
        Camera cam = Camera.main;
        if (cam == null) return default;
        return cam.ScreenPointToRay(Input.mousePosition);
    }
}
