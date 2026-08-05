using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public AudioClip clickSound;
    [Range(0f, 1f)]
    public float clickVolume = 0.8f;

    public void OnBackButton()
    {
        AudioManager.Instance.PlaySound(clickSound, clickVolume);
        DoorTransition.Instance.GoToScene("MainMenu");
    }
}