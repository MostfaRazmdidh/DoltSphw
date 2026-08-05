using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public AudioClip clickSound;
    [Range(0f, 1f)]
    public float clickVolume = 0.8f;

    public void OnLevel1Button()
    {
        AudioManager.Instance.PlaySound(clickSound, clickVolume);
        DoorTransition.Instance.GoToScene("Level1_m1");
    }

    public void OnBackButton()
    {
        AudioManager.Instance.PlaySound(clickSound, clickVolume);
        DoorTransition.Instance.GoToScene("MainMenu");
    }
}