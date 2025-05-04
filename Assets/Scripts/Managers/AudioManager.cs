using UnityEngine;
using DG.Tweening; // Certifique-se de ter DOTween instalado

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource shipAccel;
    private float shipAccelDefaultPitch;
    private float shipAccelDefaultVolume;
    private Tween shipFadeTween;

    private void Start()
    {
        shipAccelDefaultPitch = shipAccel.pitch;
        shipAccelDefaultVolume = shipAccel.volume;
        shipAccel.loop = true;
    }

    public void UpdateAccelPitch(float pitch)
    {
        if (!shipAccel.isPlaying)
        {
            shipAccel.volume = 0f;
            shipAccel.Play();

            shipFadeTween?.Kill();
            shipFadeTween = shipAccel.DOFade(shipAccelDefaultVolume, 0.5f); // Fade-in
        }

        shipAccel.pitch = pitch;

        // Caso estivesse em fade-out, cancele e retorne o volume normal
        if (shipFadeTween != null && shipFadeTween.IsActive() && shipFadeTween.IsPlaying())
        {
            if (shipAccel.volume < shipAccelDefaultVolume)
            {
                shipFadeTween.Kill();
                shipAccel.DOFade(shipAccelDefaultVolume, 0.2f); // Restaura rapidamente se estava em fade-out
            }
        }
    }

    public void StopAccel()
    {
        if (shipAccel.isPlaying)
        {
            shipFadeTween = shipAccel.DOFade(0f, 0.5f).OnComplete(() =>
            {
                shipAccel.Stop();
                shipAccel.volume = shipAccelDefaultVolume;
                shipAccel.pitch = shipAccelDefaultPitch;
            });
        }
    }
}
