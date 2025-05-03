using Fusion;
using System.Linq;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerNetworkBehaviour : NetworkBehaviour, IPlayerLeft
{


    public override void Spawned()
    {
        // Determine a posição do jogador com base na sua ordem de chegada
        PlayerRef localPlayer = Runner.LocalPlayer;
        var activePlayersList = Runner.ActivePlayers.ToList();
        int playerIndex = activePlayersList.IndexOf(localPlayer);
        Vector3 spawnPosition = GetSpawnPosition(playerIndex);
        Runner.GetPlayerObject(localPlayer).transform.position = spawnPosition;
    }

    public void PlayerLeft(PlayerRef player)
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 GetSpawnPosition(int playerIndex)
    {
        var spawner = Runner.gameObject.GetComponent<PlayerSpawner>();

        switch (playerIndex)
        {
            case 0: return spawner.spawnPositions[playerIndex];
            case 1: return spawner.spawnPositions[playerIndex];
            case 2: return spawner.spawnPositions[playerIndex];
            case 3: return spawner.spawnPositions[playerIndex];
            default: return Vector3.zero;
        }
    }
}
