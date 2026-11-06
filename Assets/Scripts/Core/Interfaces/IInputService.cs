using UnityEngine;

public interface IInputService
{
    bool IsPickupTriggered();
    Ray GetPointerRay();
}
