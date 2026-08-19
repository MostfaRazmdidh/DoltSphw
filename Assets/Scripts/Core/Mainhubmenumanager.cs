using UnityEngine;

public class MainHubMenuManager : MonoBehaviour
{
    [Header("صدا")]
    public AudioClip clickSound;
    [Range(0f, 1f)] public float clickVolume = 0.8f;

    [Header("صحنه‌های مقصد")]
    public string settingsScene = "Settings";
    public string nextLevelScene = "Level2_m1";

    public void OnSettingsButton()
    {
        PlayClick();
        GoToScene(settingsScene);
    }

    public void OnStartButton()
    {
        PlayClick();
        GoToScene(nextLevelScene);
    }

    public void OnNewsButton()
    {
        PlayClick();
        // فعلا صحنه‌ی اخبار ساخته نشده، فقط لاگ می‌گیریم
        Debug.Log("دکمه اخبار کلیک شد - صحنه اخبار هنوز ساخته نشده.");
    }

    private void PlayClick()
    {
        if (clickSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySound(clickSound, clickVolume);
    }

    private void GoToScene(string sceneName)
    {
        if (DoorTransition.Instance != null)
            DoorTransition.Instance.GoToScene(sceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}