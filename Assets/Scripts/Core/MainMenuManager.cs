using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public AudioClip clickSound;
    [Range(0f, 1f)]
    public float clickVolume = 0.8f;

    public void OnStartButton()
    {
        AudioManager.Instance.PlaySound(clickSound, clickVolume);
        DoorTransition.Instance.GoToScene("LevelSelect");
    }

    public void OnSettingsButton()
    {
        AudioManager.Instance.PlaySound(clickSound, clickVolume);
        DoorTransition.Instance.GoToScene("Settings");
    }

    public void OnNewsButton()
    {
        AudioManager.Instance.PlaySound(clickSound, clickVolume);
        DoorTransition.Instance.GoToScene("Rozname");
    }
}