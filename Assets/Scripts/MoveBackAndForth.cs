using UnityEngine;

public class MoveBackAndForthWithPush : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 1f;

    public float pushHorizontalForce = 10f;  // Hvor langt den skubber horisontalt
    public float pushVerticalForce = 15f;    // Hvor højt den skubber op

    private float startTime;
    private float journeyLength;

    private Vector3 lastPosition;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("PointA eller PointB er ikke sat!");
            enabled = false;
            return;
        }

        startTime = Time.time;
        journeyLength = Vector3.Distance(pointA.position, pointB.position);
        lastPosition = transform.position;
    }

    void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = Mathf.PingPong(distCovered, journeyLength) / journeyLength;

        Vector3 newPos = Vector3.Lerp(pointA.position, pointB.position, fracJourney);
        lastPosition = transform.position;
        transform.position = newPos;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            ThirdPersonMovement playerMovement = hit.gameObject.GetComponent<ThirdPersonMovement>();
            if (playerMovement != null)
            {
                // Retningen af bevægelsen (frem og tilbage)
                Vector3 pushDir = (transform.position - lastPosition).normalized;

                if (pushDir == Vector3.zero) pushDir = transform.forward; // fallback

                // Vi giver spilleren en impuls via en public funktion i ThirdPersonMovement
                Vector3 pushVelocity = pushDir * pushHorizontalForce + Vector3.up * pushVerticalForce;
                playerMovement.AddExternalVelocity(pushVelocity);
            }
        }
    }
}
