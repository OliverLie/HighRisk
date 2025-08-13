using UnityEngine;

public class SlipperySurface : MonoBehaviour
{
    public float glideSpeed = 6f;

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonMovement movement = other.GetComponent<ThirdPersonMovement>();
        if (movement != null)
        {
            // Starter glide i slipperysurface-objektets forward retning
            Vector3 glideDirection = transform.forward;
            movement.StartGlide(glideDirection.normalized * glideSpeed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ThirdPersonMovement movement = other.GetComponent<ThirdPersonMovement>();
        if (movement != null)
        {
            movement.StopGlide();
        }
    }
}
