using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public Transform cam;

    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    Vector3 velocity;
    Vector3 externalVelocity;

    bool isGrounded;

    bool isGliding = false;
    float glideFriction = 1f;  // Jo højere, jo hurtigere stopper glidebevægelsen
    Vector3 glideVelocity = Vector3.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            externalVelocity = Vector3.zero;

            // Stop glide når man er på jorden og ikke i glide-zone
            if (!isGliding)
                glideVelocity = Vector3.zero;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            if (!isGliding)
            {
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }
            else
            {
                // Hvis man glider, kan man stadig styre lidt, men glideVelocity er vigtigst
                Vector3 glideMove = glideVelocity * Time.deltaTime;
                Vector3 inputMove = moveDir.normalized * speed * 0.5f * Time.deltaTime; // Halv kontrol
                controller.Move(glideMove + inputMove);

                // Glide friktion - sæt glideVelocity gradvist mod 0
                glideVelocity = Vector3.Lerp(glideVelocity, Vector3.zero, glideFriction * Time.deltaTime);

                // Stop glide når glideVelocity er meget lav
                if (glideVelocity.magnitude < 0.1f)
                {
                    StopGlide();
                }
            }
        }
        else
        {
            // Ingen input
            if (!isGliding)
            {
                // Bare stå stille, uden glidebevægelse
                controller.Move(Vector3.zero);
            }
            else
            {
                // Når man glider uden input
                Vector3 glideMove = glideVelocity * Time.deltaTime;
                controller.Move(glideMove);

                glideVelocity = Vector3.Lerp(glideVelocity, Vector3.zero, glideFriction * Time.deltaTime);

                if (glideVelocity.magnitude < 0.1f)
                {
                    StopGlide();
                }
            }
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            StopGlide();  // Stop glide når man hopper
        }

        velocity.y += gravity * Time.deltaTime;

        // Y-aksens bevægelse (jump/gravity)
        controller.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    public void StartGlide(Vector3 initialGlideVelocity)
    {
        isGliding = true;
        glideVelocity = initialGlideVelocity;
    }

    public void StopGlide()
    {
        isGliding = false;
        glideVelocity = Vector3.zero;
    }

    public void AddExternalVelocity(Vector3 force)
    {
        externalVelocity += force;
    }


    public void SetVerticalVelocity(float newYVelocity)
    {
    velocity.y = newYVelocity;
    }

}
