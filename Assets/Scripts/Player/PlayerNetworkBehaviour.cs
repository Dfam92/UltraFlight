using Fusion;
using System.Threading.Tasks;
using UnityEngine;
public class PlayerNetworkBehaviour : NetworkBehaviour, IPlayerLeft
{
    public override async void Spawned()
    {
        if (HasStateAuthority)
        {
            PlayerRef localPlayer = Runner.LocalPlayer;
            Vector3 spawnPosition = GetSpawnPosition(localPlayer.PlayerId - 1);
            Debug.Log($"🔢 Spawn position for player {localPlayer}: {spawnPosition}");
            NetworkTransform netTransform = GetComponent<NetworkTransform>();
            await Task.Delay(1000); // ⏳ Espera 2 segundos antes de teleportar
            netTransform.Teleport(spawnPosition);
            //gameObject.transform.position = spawnPosition;
        }
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
        Debug.Log("PlayerIndex: " + playerIndex);
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
