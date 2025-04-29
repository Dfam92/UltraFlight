using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            // Defina a posição inicial de spawn
            Vector3 spawnPosition = new Vector3(0, 1, 0);

            // Faça o spawn do player com autoridade atribuída ao PlayerRef
            NetworkObject playerNetworkObj = Runner.Spawn(
                PlayerPrefab,
                spawnPosition,
                Quaternion.identity,
                player
            );

            // Associa o objeto do jogador ao PlayerRef (necessário para GetPlayerObject funcionar)
            Runner.SetPlayerObject(player, playerNetworkObj);

            Debug.Log($"Spawned and registered player object for {player}");
        }
            

    }
}
