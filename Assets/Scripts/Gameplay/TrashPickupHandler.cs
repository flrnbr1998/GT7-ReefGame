using System.Collections;
using UnityEngine;

public class TrashPickupHandler : MonoBehaviour
{
    [SerializeField] private float speed = 20f;

    private bool _flying;
    private Rigidbody _rb;
    private Camera _cam;

    private void Start()
    {
        _rb  = GetComponent<Rigidbody>();
        _cam = FindCamera();
    }

    private void OnMouseDown() => StartPickup();

    public void StartPickup()
    {
        if (_flying) return;
        _flying = true;
        if (_rb != null) _rb.isKinematic = true;
        StartCoroutine(FlyToCamera());
    }

    private IEnumerator FlyToCamera()
    {
        while (_flying)
        {
            if (_cam == null) _cam = FindCamera();
            if (_cam == null) { yield return null; continue; }

            Vector3 direction = (_cam.transform.position - transform.position).normalized;
            transform.position  += direction * speed * Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime);

            Vector3 vp       = _cam.WorldToViewportPoint(transform.position);
            bool    offScreen = vp.z <= 0 || vp.x < 0 || vp.x > 1 || vp.y < 0 || vp.y > 1;

            if (offScreen || transform.localScale.magnitude < 0.01f)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return null;
        }
    }

    // Mirror original DestroyObject behaviour: use allCameras[0] as fallback
    private static Camera FindCamera()
    {
        if (Camera.main != null) return Camera.main;
        return Camera.allCameras.Length > 0 ? Camera.allCameras[0] : null;
    }
}
