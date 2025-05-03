using Fusion;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public List<Vector3> spawnPositions;

    //// Posições fixas de spawn
    //public Vector3 pos1 = new Vector3(-13, 0, 0);
    //public Vector3 pos2 = new Vector3(-5, 0, 0);
    //public Vector3 pos3 = new Vector3(5, 0, 0);
    //public Vector3 pos4 = new Vector3(13, 0, 0);

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            // Log da posição antes de spawnar
           // Debug.Log($"🔢 Spawn position for player {player}: {spawnPosition}");

            // Spawn o jogador
            NetworkObject playerNetworkObj = Runner.Spawn(
                PlayerPrefab,
                Vector3.zero,
                Quaternion.identity,
                player
            );

            Runner.SetPlayerObject(player, playerNetworkObj);
            //Debug.Log($"✅ Spawned and registered player object for {player} at {spawnPosition}");
        }
    }



    
}
