using Fusion;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public NetworkRunner Runner;
    [SerializeField] string sessionId;
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;

    private void Awake()
    {
        int randomId = Random.Range(1, 1000); // Range é inclusivo no min e exclusivo no max
        sessionId = randomId.ToString();
    }

    public async void StartGame()
    { 

        // The start game arguments setup the game.
        StartGameArgs startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared, // Client GameMode, could be Shared as well
            SessionName = sessionId, // Session to Join
            PlayerCount = 4,
        };

        // We wait for the runner to start the game
        var results = await Runner.StartGame(startGameArgs);

        if (results.Ok)
        {
            FadeOut();
        }
        else
        {
            ShowShutdown(results.ShutdownReason);
        }

    }

    public void OnQuickJoinPressed()
    {
        StartGame();
    }

    internal void ShowShutdown(ShutdownReason shutdownReason)
    {
        //mainMenuCanvas.enabled = true;
        //mainMenuCanvasGroup.alpha = 1f;
        //mainMenuCanvasGroup.interactable = true;

        //// Displays the reason the connection failed.
        //connectionFailText.text = shutdownReason.ToString();
        //connectionFailedPanel.SetActive(true);
    }

    public void FadeIn()
    {
        canvasGroup.DOFade(1f, fadeDuration)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
    }

    public void FadeOut()
    {
        canvasGroup.DOFade(0f, fadeDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            });
        }
    }
