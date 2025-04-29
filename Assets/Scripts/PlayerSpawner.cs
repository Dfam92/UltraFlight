using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            // Defina a posi��o inicial de spawn
            Vector3 spawnPosition = new Vector3(0, 1, 0);

            // Fa�a o spawn do player com autoridade atribu�da ao PlayerRef
            NetworkObject playerNetworkObj = Runner.Spawn(
                PlayerPrefab,
                spawnPosition,
                Quaternion.identity,
                player
            );

            // Associa o objeto do jogador ao PlayerRef (necess�rio para GetPlayerObject funcionar)
            Runner.SetPlayerObject(player, playerNetworkObj);

            Debug.Log($"Spawned and registered player object for {player}");
        }
            

    }
}
