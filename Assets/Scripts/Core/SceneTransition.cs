using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;
    public RectTransform canvasRect;
    public float slideDuration = 0.4f;

    void Awake()
    {
        Instance = this;
    }

    public void GoToScene(string sceneName, AudioClip clickSound)
    {
        StartCoroutine(SlideAndLoad(sceneName, clickSound));
    }

    private IEnumerator SlideAndLoad(string sceneName, AudioClip clickSound)
    {
        AudioManager.Instance.PlaySound(clickSound);

        // صفحه به چپ میره
        float timer = 0;
        Vector3 startPos = canvasRect.position;
        Vector3 endPos = new Vector3(-Screen.width, startPos.y, 0);

        while (timer < slideDuration)
        {
            timer += Time.deltaTime;
            float t = timer / slideDuration;
            // انیمیشن نرم‌تر
            t = t * t * (3f - 2f * t);
            canvasRect.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}