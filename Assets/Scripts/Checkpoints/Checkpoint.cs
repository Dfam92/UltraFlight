using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] CheckpointTeleportManager manager;
    public GameObject tranformToTeleportPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.CheckLastCheckpointAchieved(gameObject.transform);
        }
    }
}
