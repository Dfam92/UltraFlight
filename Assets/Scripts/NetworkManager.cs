using Fusion;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] NetworkRunner runner;
    [SerializeField] string sessionId;
    //[Networked, Capacity(8), Tooltip("The Network Behaviour references for the players in their starting position order.")]
    //public NetworkArray<NetworkBehaviourId> playerList => default;

    private void Awake()
    {
        int randomId = Random.Range(1, 1000); // Range é inclusivo no min e exclusivo no max
        string sessionId = $"Sala{randomId}";
    }

    public void StartGame()
    {
        runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared, // Client GameMode, could be Shared as well
            SessionName = sessionId, // Session to Join
            PlayerCount = 4,
                                        // ...
        });
    }
}
