using Fusion;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerNetworkBehaviour : NetworkBehaviour, IPlayerLeft
{
    private string hole = "Hole";
    private string finishLine = "FinishLine";
    [SerializeField] private NetworkTransform _networkTransform;
    [SerializeField] private CheckpointTeleportManager _checkpointTeleportManager;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private GameManager _gameManager;
    


    public override void Spawned()
    {
        PlayerRef localPlayer = Runner.LocalPlayer;
        Vector3 spawnPosition = GetSpawnPosition(localPlayer.PlayerId - 1);
        //Debug.Log($"🔢 Spawn position for player {localPlayer}: {spawnPosition}");
        //await Task.Delay(1000); // ⏳ Espera 2 segundos antes de teleportar
        _networkTransform.Teleport(spawnPosition);
        //gameObject.transform.position = spawnPosition;
    }


    public void PlayerLeft(PlayerRef player)
    {
        throw new System.NotImplementedException();
    }

    private void OnEnable()
    {
        _checkpointTeleportManager = FindFirstObjectByType<CheckpointTeleportManager>();
        _gameManager = FindFirstObjectByType<GameManager>();
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

    void OnTriggerEnter(Collider other)
    {
        // Only the state authority to the player can cause them to explode
        if (!HasStateAuthority)
            return;

        if (other.CompareTag(hole) && _characterController.enabled)
        {
            Debug.Log("CAIU NO BURACO");
           
            _characterController.enabled = false;
            transform.position = _checkpointTeleportManager.transformToMovePlayer.position;
            transform.rotation = _checkpointTeleportManager.transformToMovePlayer.rotation;
            _playerMovement.ResetMovementState();
            _characterController.enabled = true;
        }

        if (other.CompareTag(finishLine) && !_gameManager.raceWasFinished)
        {
            Debug.Log("Vitória!");
            _gameManager.raceWasFinished = true;
            
            
            //transform.position = new Vector3(0, 10, 0);
            _gameManager.Win();
            _characterController.enabled = false;
            _playerMovement.ResetMovementState();
            _networkTransform.Teleport(new Vector3(0,10,0));
            _characterController.enabled = true;
        }
    }
}
