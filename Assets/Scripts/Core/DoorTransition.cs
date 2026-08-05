using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorTransition : MonoBehaviour
{
    public static DoorTransition Instance;

    [Header("در ها")]
    public RectTransform leftDoor;
    public RectTransform rightDoor;

    [Header("تنظیمات")]
    public float duration = 1.5f;
    public AudioClip doorSound;
    [Range(0f, 1f)]
    public float doorVolume = 0.8f;

    private AudioSource audioSource;
    private Vector2 leftStart;
    private Vector2 rightStart;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = gameObject.AddComponent<AudioSource>();

        leftStart = leftDoor.anchoredPosition;
        rightStart = rightDoor.anchoredPosition;

        leftDoor.gameObject.SetActive(false);
        rightDoor.gameObject.SetActive(false);
    }

    public void GoToScene(string sceneName)
    {
        StartCoroutine(LoadAndOpenDoors(sceneName));
    }

    private IEnumerator LoadAndOpenDoors(string sceneName)
    {
        // اول Scene لود میشه
        SceneManager.LoadScene(sceneName);

        // یه فریم صبر میکنه تا Scene لود بشه
        yield return null;

        // بعد درها باز میشن
        leftDoor.gameObject.SetActive(true);
        rightDoor.gameObject.SetActive(true);

        leftDoor.anchoredPosition = leftStart;
        rightDoor.anchoredPosition = rightStart;

        if (doorSound != null)
            audioSource.PlayOneShot(doorSound, doorVolume);

        float timer = 0;
        Vector2 leftEnd = new Vector2(-1920, leftStart.y);
        Vector2 rightEnd = new Vector2(1920, rightStart.y);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            t = t * t * (3f - 2f * t);
            leftDoor.anchoredPosition = Vector2.Lerp(leftStart, leftEnd, t);
            rightDoor.anchoredPosition = Vector2.Lerp(rightStart, rightEnd, t);
            yield return null;
        }

        leftDoor.gameObject.SetActive(false);
        rightDoor.gameObject.SetActive(false);
    }
}