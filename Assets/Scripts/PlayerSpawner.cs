using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    public Vector3 playerPos;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Vector3 spawnPosition = playerPos;

            NetworkObject playerNetworkObj = Runner.Spawn(
                PlayerPrefab,
                spawnPosition,
                Quaternion.identity,
                player
            );

            Runner.SetPlayerObject(player, playerNetworkObj);

            Debug.Log($"Spawned and registered player object for {player}");
        }
    }
}