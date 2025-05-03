using Fusion;
using UnityEngine;

public class CheckPlayerOutOfRoad : SimulationBehaviour
{
    [SerializeField] CheckpointTeleportManager CheckpointTeleportManager;

    private void OnTriggerEnter(Collider other)
    {
       if(other.CompareTag("Player"))
        {
            Debug.Log("Spawning Player on the right position");
            var playerObj = Runner.GetPlayerObject(Runner.LocalPlayer);

            if (playerObj != null && playerObj.HasStateAuthority)
            {
                var controller = playerObj.GetComponent<CharacterController>();

                if (controller != null)
                    controller.enabled = false;

                playerObj.transform.position = CheckpointTeleportManager.transformToMovePlayer.position;
                playerObj.transform.rotation = CheckpointTeleportManager.transformToMovePlayer.rotation;

                playerObj.GetComponent<PlayerMovement>().ResetMovementState();


                if (controller != null)
                    controller.enabled = true;
            }
        }
    }
}
