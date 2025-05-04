using Fusion;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public List<Vector3> spawnPositions;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {

            // Spawn o jogador
            NetworkObject playerNetworkObj = Runner.Spawn(
                PlayerPrefab,
                Vector3.zero,
                Quaternion.identity,
                player
            );

            Runner.SetPlayerObject(player, playerNetworkObj);
            
        }
    }



    
}
