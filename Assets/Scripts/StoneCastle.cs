using UnityEngine;

public class StoneCastleSpawn : MonoBehaviour
{
    public GameObject player;
    public Transform NextPoint;

    

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("StoneCastle")) // Din overflade skal have tagget "Lava"
        {
            player.transform.position = NextPoint.position;

        }
        

    }
}
