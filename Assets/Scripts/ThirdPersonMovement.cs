using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform cam;
    public Animator animator; // Base layer: 2D Blend Tree, Jump layer: Jump animation

    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float sprintMultiplier = 1.5f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Jump & Gravity")]
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    [Header("Glide Settings")]
    public float glideFriction = 1f;

    // Animation blend tree input
    private float moveX; // Lokal strafe (venstre/højre)
    private float moveY; // Lokal frem/tilbage

    private Vector3 velocity;
    private Vector3 externalVelocity;
    private Vector3 glideVelocity;
    private bool isGrounded;
    private bool isGliding;
    private bool isJumping; // Lokalt flag til animation
    private bool isKnockedback = false;
    private float knockbackTimer = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;
        HandleGroundCheck();
        HandleInput();
        ApplyGravity();
        ApplyVerticalMovement();

        // Send til Animator (2D Blend Tree)
        if (animator != null)
        {
            animator.SetFloat("MoveX", moveX);
            animator.SetFloat("MoveY", moveY);
            animator.SetBool("IsJumping", isJumping); // Send jump-status til jump layer
        }
    }

    #region Core Movement
    private void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0f, v).normalized;

        // Bestem sprint status
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && inputDir.magnitude >= 0.1f;
        float speedMultiplier = isSprinting ? sprintMultiplier : 1f;

        if (inputDir.magnitude >= 0.1f)
        {
            RotateTowardsCameraDirection();
            if (isGliding)
                HandleGlideMovement(inputDir);
            else
                MoveNormal(inputDir, speedMultiplier);
        }
        else
        {
            HandleIdleMovement();
        }

        // Hop input
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
            isJumping = true; // Starter jump animationen
        }
    }

    private void RotateTowardsCameraDirection()
    {
        float targetAngle = cam.eulerAngles.y;
        float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
    }

    private void MoveNormal(Vector3 inputDir, float speedMultiplier)
    {
        Vector3 worldMove = CameraRelativeDirection(inputDir);
        Vector3 localMove = transform.InverseTransformDirection(worldMove);

        moveX = Mathf.Lerp(moveX, localMove.x * speedMultiplier, Time.deltaTime * 10f);
        moveY = Mathf.Lerp(moveY, localMove.z * speedMultiplier, Time.deltaTime * 10f);

        controller.Move(worldMove * walkSpeed * speedMultiplier * Time.deltaTime);
    }

    private void HandleGlideMovement(Vector3 inputDir)
    {
        Vector3 worldMove = CameraRelativeDirection(inputDir);
        Vector3 localMove = transform.InverseTransformDirection(worldMove);

        moveX = Mathf.Lerp(moveX, localMove.x * 0.5f, Time.deltaTime * 10f);
        moveY = Mathf.Lerp(moveY, localMove.z * 0.5f, Time.deltaTime * 10f);

        Vector3 glideMove = glideVelocity * Time.deltaTime;
        Vector3 inputMove = worldMove * (walkSpeed * 0.5f) * Time.deltaTime;

        controller.Move(glideMove + inputMove);
        ApplyGlideFriction();
    }

    private void HandleIdleMovement()
    {
        moveX = Mathf.Lerp(moveX, 0f, Time.deltaTime * 10f);
        moveY = Mathf.Lerp(moveY, 0f, Time.deltaTime * 10f);

        if (!isGliding)
        {
            controller.Move(Vector3.zero);
        }
        else
        {
            controller.Move(glideVelocity * Time.deltaTime);
            ApplyGlideFriction();
        }
    }

    private Vector3 CameraRelativeDirection(Vector3 inputDir)
    {
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        return (camForward * inputDir.z + camRight * inputDir.x).normalized;
    }
    #endregion

    #region Glide
    private void ApplyGlideFriction()
    {
        glideVelocity = Vector3.Lerp(glideVelocity, Vector3.zero, glideFriction * Time.deltaTime);
        if (glideVelocity.magnitude < 0.1f)
            StopGlide();
    }

    public void StartGlide(Vector3 initialVelocity)
    {
        isGliding = true;
        glideVelocity = initialVelocity;
    }

    public void StopGlide()
    {
        isGliding = false;
        glideVelocity = Vector3.zero;
    }
    #endregion

    #region Jump & Gravity
    private void HandleGroundCheck()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            externalVelocity = Vector3.zero;

            // Stop jump animation, hvis vi er landet
            if (isJumping)
                isJumping = false;

            //if (!isGliding)
            //  glideVelocity = Vector3.zero;
        }
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        StopGlide();
    }

    public void Bounce(float force)
    {
    // giv opadgående fart
    velocity.y = Mathf.Sqrt(force * -2f * gravity);

    // fortæl animatoren at vi hopper nu
    if (animator != null)
    {
        animator.SetTrigger("Bounce");
        animator.SetBool("IsJumping", true); // sæt i luften med det samme
    }

    // lille trick: markér som ikke grounded i samme frame
    isGrounded = false;
    }




    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    private void ApplyVerticalMovement()
    {
        controller.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);

    }
    #endregion

    #region External Forces
    public void AddExternalVelocity(Vector3 force)
    {
        externalVelocity += force;

    }

    public void SetVerticalVelocity(float newYVelocity)
    {
        velocity.y = newYVelocity;

    }



    public void Knockback(Vector3 direction, float force, float duration)
    {
        isKnockedback = true;
        knockbackTimer = duration;

        direction.y = 1f; // giv et hop opad
        velocity = direction.normalized * force;
    }
    


    #endregion
}
