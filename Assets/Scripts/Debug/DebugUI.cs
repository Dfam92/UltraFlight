using Fusion;
using TMPro;
using UnityEngine;
using System.Collections;

public class DebugUI : SimulationBehaviour
{
    public PlayerMovement playerMovement;
    public TextMeshProUGUI debugText;

    private IEnumerator Start()
    {
        // Espera até o Player Object estar disponível
        while (Runner == null || Runner.GetPlayerObject(Runner.LocalPlayer) == null)
            yield return null;

        var playerObj = Runner.GetPlayerObject(Runner.LocalPlayer);
        playerMovement = playerObj.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (playerMovement == null || debugText == null) return;

        string state = playerMovement.AllowBackwardMovement ? "ON" : "OFF";
        debugText.text = $"[Debug] Backward Movement: {state}";
    }
}
