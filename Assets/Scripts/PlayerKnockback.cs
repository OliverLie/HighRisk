using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    public float knockbackForce = 15f;    // Justér for kraft
    public float knockbackDamping = 3f;   // Hvor hurtigt knockback fader ud

    private CharacterController controller;
    private Vector3 knockbackVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (knockbackVelocity.magnitude > 0.1f)
        {
            controller.Move(knockbackVelocity * Time.deltaTime);

            // Mindsker knockback over tid, men lidt langsommere end før
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDamping * Time.deltaTime);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Spike"))
        {
            // Skub spilleren væk med fuld kraft med det samme
            Vector3 knockbackDir = (transform.position - hit.transform.position).normalized;

            knockbackVelocity = knockbackDir * knockbackForce;

            // Optional: kan tilføje lidt opad kraft, så spilleren "hopper" lidt
            knockbackVelocity += Vector3.up * (knockbackForce * 0.3f);
        }
    }
}
