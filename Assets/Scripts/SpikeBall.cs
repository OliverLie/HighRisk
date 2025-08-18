using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZoneSpawner : MonoBehaviour
{
    [Header("Spike prefabs to spawn")]
    public GameObject[] spikePrefabs;

    [Header("Spawn positions (optional)")]
    public Transform[] spawnPoints;

    private List<GameObject> spawnedSpikes = new List<GameObject>();

    private bool isSpikesActive = false;

    [SerializeField] float DestroyTime = 10f;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float knockbackDuration = 0.5f;


    private void OnTriggerEnter(Collider other)
    {
        // Hvis spikes allerede er aktive, gør ikke noget (undgå dobbelt spawn)
        if (isSpikesActive)
            return;

        SpawnSpikes();
    }

    private void SpawnSpikes()
    {
        isSpikesActive = true;

        // Spawn spikes baseret på spikePrefabs og spawnPoints
        for (int i = 0; i < spikePrefabs.Length; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            Quaternion spawnRot = Quaternion.identity;

            // Hvis der er en spawnPoint til den index, brug den, ellers spawn på trigger zone position
            if (spawnPoints != null && spawnPoints.Length > i && spawnPoints[i] != null)
            {
                spawnPos = spawnPoints[i].position;
                spawnRot = spawnPoints[i].rotation;
            }
            else
            {
                spawnPos = transform.position;
                spawnRot = Quaternion.identity;
            }

            GameObject spike = Instantiate(spikePrefabs[i], spawnPos, spawnRot);
            spawnedSpikes.Add(spike);

            // Aktivér mesh renderer hvis nødvendigt
            var mr = spike.GetComponent<MeshRenderer>();
            if (mr != null)
                mr.enabled = true;

            // Aktivér useGravity hvis spike har Rigidbody
            var rb = spike.GetComponent<Rigidbody>();
            if (rb != null)
                rb.useGravity = true;

            // Start coroutine der sletter spikes efter 6 sekunder
            StartCoroutine(DestroySpikeAfterSeconds(spike, DestroyTime));
        }
    }

    private IEnumerator DestroySpikeAfterSeconds(GameObject spike, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (spawnedSpikes.Contains(spike))
            spawnedSpikes.Remove(spike);

        Destroy(spike);

        // Hvis alle spikes er destroyed, sæt isSpikesActive = false så man kan spawne igen
        if (spawnedSpikes.Count == 0)
            isSpikesActive = false;
    }


     private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            ThirdPersonMovement player = hit.gameObject.GetComponent<ThirdPersonMovement>();
            if (player != null)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                player.Knockback(dir, launchForce, knockbackDuration);
            }
        }
    }
}
