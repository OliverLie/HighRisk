using UnityEngine;

public class CloudMover : MonoBehaviour
{
    public RectTransform cloud;
    public RectTransform pointA;
    public RectTransform pointB;
    public float speed = 100f;

    [Header("Vertical Float")]
    public float amplitude = 20f; // hvor meget den bevæger sig op/ned
    public float frequency = 2f;  // hvor hurtigt den "bølger"

    private Vector2 targetPos;
    private float baseY;

    void Start()
    {
        // Start ved pointA
        cloud.anchoredPosition = pointA.anchoredPosition;
        targetPos = pointB.anchoredPosition;

        // Gem udgangspunktet for Y
        baseY = cloud.anchoredPosition.y;
    }

    void Update()
    {
        // Flyt hen imod target langs X
        Vector2 current = cloud.anchoredPosition;
        Vector2 next = Vector2.MoveTowards(
            current,
            new Vector2(targetPos.x, current.y),
            speed * Time.deltaTime
        );

        // Tilføj en "sinus wave" på Y
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        next.y = baseY + yOffset;

        cloud.anchoredPosition = next;

        // Skift mellem pointA og pointB når vi rammer target
        if (Mathf.Abs(cloud.anchoredPosition.x - targetPos.x) < 0.1f)
        {
            targetPos = (Mathf.Approximately(targetPos.x, pointB.anchoredPosition.x))
                ? pointA.anchoredPosition
                : pointB.anchoredPosition;
        }
    }
}
