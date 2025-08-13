using UnityEngine;

public class LavaRespawn : MonoBehaviour
{
    public GameObject player;
    public Transform RespawnPoint;

    

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Lava")) // Din overflade skal have tagget "Lava"
        {
            player.transform.position = RespawnPoint.position;

        }
        

    }
}
