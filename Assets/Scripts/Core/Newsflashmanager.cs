using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using RTLTMPro;
using System.Collections;

public class NewsFlashManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject textBox;
    public RTLTextMeshPro dialogText;

    [Header("متن دیالوگ")]
    [TextArea(3, 6)]
    public string dialog = "امروز مردم برای انتخابات رئیس دولت جدید پای صندوق‌های رای رفتند. مراکز رای‌گیری شلوغ گزارش شد و میلیون‌ها نفر در حال شرکت در انتخابات هستند و به زودی نتایج رای‌گیری اعلام می‌شود.";

    [Header("تنظیمات تایپ")]
    public float typingSpeed = 0.05f;
    public AudioClip typingSound;
    [Range(0f, 1f)] public float typingVolume = 0.5f;

    [Header("صدای اخبار (فورا پخش میشه)")]
    public AudioClip newsAmbience;
    [Range(0f, 1f)] public float newsVolume = 0.2f;

    [Header("انیمیشن ظاهر شدن TextBox")]
    public float fadeDuration = 0.3f;
    public float scaleFrom = 0.85f;

    [Header("زمان انتظار بعد از پایان تایپ")]
    public float clickDelay = 5f;

    [Header("صحنه بعدی")]
    public string nextScene = "Level1_m1_R5";

    private bool waitingForFirstTap = true;
    private bool canClickToContinue = false;
    private AudioSource typingAudioSource;
    private AudioSource newsAudioSource;

    void OnEnable()
    {
        if (!EnhancedTouchSupport.enabled)
            EnhancedTouchSupport.Enable();
    }

    void Start()
    {
        typingAudioSource = gameObject.AddComponent<AudioSource>();
        newsAudioSource = gameObject.AddComponent<AudioSource>();

        // TextBox اولش کاملا مخفی و کوچیک
        CanvasGroup cg = EnsureCanvasGroup(textBox);
        cg.alpha = 0f;
        textBox.transform.localScale = Vector3.one * scaleFrom;
        textBox.SetActive(false);

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();

        // صدای اخبار فورا پخش میشه
        if (newsAmbience != null)
        {
            newsAudioSource.clip = newsAmbience;
            newsAudioSource.loop = true;
            newsAudioSource.volume = newsVolume;
            newsAudioSource.Play();
        }
    }

    void Update()
    {
        bool tapped = IsTapped();

        if (waitingForFirstTap)
        {
            if (tapped)
            {
                waitingForFirstTap = false;
                StartCoroutine(ShowBoxAndType());
            }
            return;
        }

        if (canClickToContinue && tapped)
        {
            SceneManager_LoadNext();
        }
    }

    private bool IsTapped()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        return false;
    }

    private IEnumerator ShowBoxAndType()
    {
        textBox.SetActive(true);
        CanvasGroup cg = EnsureCanvasGroup(textBox);

        yield return StartCoroutine(FadeAndScale(cg, textBox.transform, 0f, 1f, scaleFrom, 1f, fadeDuration));

        yield return StartCoroutine(TypeText());

        yield return new WaitForSeconds(clickDelay);
        canClickToContinue = true;
    }

    private IEnumerator TypeText()
    {
        dialogText.text = "";
        if (typingSound != null)
            typingAudioSource.PlayOneShot(typingSound, typingVolume);

        for (int i = 1; i <= dialog.Length; i++)
        {
            dialogText.text = dialog.Substring(0, i);
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator FadeAndScale(CanvasGroup cg, Transform t, float fromAlpha, float toAlpha, float fromScale, float toScale, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);
            float eased = progress * progress * (3f - 2f * progress);

            cg.alpha = Mathf.Lerp(fromAlpha, toAlpha, eased);
            t.localScale = Vector3.one * Mathf.Lerp(fromScale, toScale, eased);
            yield return null;
        }
        cg.alpha = toAlpha;
        t.localScale = Vector3.one * toScale;
    }

    private CanvasGroup EnsureCanvasGroup(GameObject go)
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        if (cg == null) cg = go.AddComponent<CanvasGroup>();
        return cg;
    }

    private void SceneManager_LoadNext()
    {
        typingAudioSource.Stop();
        newsAudioSource.Stop();

        if (DoorTransition.Instance != null)
            DoorTransition.Instance.GoToScene(nextScene);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }
}