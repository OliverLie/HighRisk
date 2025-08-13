using UnityEngine;

public class CloudBounce : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceForce = 12f; // Hvor højt spilleren skal hoppe
    public bool destroyOnBounce = false; // Skal skyen forsvinde efter hop?

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonMovement movement = other.GetComponent<ThirdPersonMovement>();
        if (movement != null)
        {
            // Sæt vertikal hastighed til bounceForce
            movement.SetVerticalVelocity(bounceForce);

            // Valgfrit: fjern skyen efter hop
            if (destroyOnBounce)
            {
                Destroy(gameObject);
            }
        }
    }
}
