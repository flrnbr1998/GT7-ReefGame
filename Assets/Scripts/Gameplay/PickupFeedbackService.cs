using UnityEngine;
using VContainer;

public class PickupFeedbackService : MonoBehaviour, IFeedbackService
{
    [SerializeField] private ParticleSystem pickupEffect;

    [Inject] private IAudioService _audio;

    public void PlayPickupFeedback(Vector3 worldPosition)
    {
        if (pickupEffect != null)
        {
            pickupEffect.transform.position = worldPosition;
            pickupEffect.Play();
        }
        _audio?.PlayOneShot(FMODEvents.PickUpTrashPath, worldPosition);
    }
}
