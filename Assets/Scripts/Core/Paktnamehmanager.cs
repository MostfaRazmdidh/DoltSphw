using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using RTLTMPro;
using System.Collections;

public class PaktNamehManager : MonoBehaviour
{
    [Header("پاکت (Pakt)")]
    public Button envelopeButton;      // کامپوننت Button روی خود Pakt
    public RectTransform envelopeRect;

    [Header("نور دور پاکت (Glow) - اختیاری")]
    public CanvasGroup glowCanvasGroup;
    public bool pulseGlow = true;
    public float glowMinAlpha = 0.3f;
    public float glowMaxAlpha = 0.9f;
    public float glowSpeed = 1.5f;

    [Header("نامه (nama)")]
    public GameObject letterObject;     // خود آبجکت nama
    public RTLTextMeshPro letterText;   // میتونه تو Editor خاموش (Inactive) باشه، خودمون روشنش میکنیم

    [Header("متن نامه")]
    [TextArea(3, 6)]
    public string letterMessage = "شما با کسب ۱۶ میلیون رای به عنوان رئیس دولت جدید انتخاب شدید.";

    [Header("صداها")]
    public AudioClip envelopeOpenSound;
    public AudioClip typingSound;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;
    [Range(0f, 1f)] public float typingVolume = 0.5f;

    [Header("تنظیمات انیمیشن")]
    public float envelopeAnimDuration = 0.5f;
    public float letterAnimDuration = 0.6f;
    public float typingSpeed = 0.08f; // عدد بزرگتر = تایپ آهسته‌تر

    [Header("رفتن به صحنه بعدی")]
    public float clickDelay = 5f;
    public string nextScene = "Level1_m1_end";

    private AudioSource audioSource;
    private Coroutine glowCoroutine;
    private bool opened = false;
    private bool canClickToContinue = false;
    private Vector3 letterFinalScale;

    void OnEnable()
    {
        if (!EnhancedTouchSupport.enabled)
            EnhancedTouchSupport.Enable();
    }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        // سایز نهایی نامه رو از همون چیزی که تو صحنه چیدی ذخیره می‌کنیم
        letterFinalScale = letterObject.transform.localScale;

        // حالت اولیه نامه: مخفی و کوچیک، همون جایی که خودت گذاشتیش
        CanvasGroup letterCg = EnsureCanvasGroup(letterObject);
        letterCg.alpha = 0f;
        letterCg.blocksRaycasts = false; // تا وقتی نامرئیه جلوی کلیک روی Pakt رو نگیره
        letterCg.interactable = false;
        letterObject.transform.localScale = Vector3.zero;
        letterObject.SetActive(false);

        // متن هم می‌تونه خاموش باشه، مشکلی نیست، موقع باز شدن پاکت روشنش می‌کنیم
        if (letterText != null)
        {
            letterText.text = "";
        }

        envelopeButton.onClick.AddListener(OnEnvelopeClicked);

        if (glowCanvasGroup != null && pulseGlow)
            glowCoroutine = StartCoroutine(PulseGlow());
    }

    void Update()
    {
        if (!canClickToContinue) return;

        if (IsTapped())
        {
            canClickToContinue = false;

            if (DoorTransition.Instance != null)
                DoorTransition.Instance.GoToScene(nextScene);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
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

    private IEnumerator PulseGlow()
    {
        while (true)
        {
            float t = (Mathf.Sin(Time.time * glowSpeed) + 1f) / 2f; // 0..1
            glowCanvasGroup.alpha = Mathf.Lerp(glowMinAlpha, glowMaxAlpha, t);
            yield return null;
        }
    }

    private void OnEnvelopeClicked()
    {
        if (opened) return;
        opened = true;

        envelopeButton.interactable = false;

        if (glowCoroutine != null) StopCoroutine(glowCoroutine);
        if (glowCanvasGroup != null) glowCanvasGroup.alpha = 0f;

        if (envelopeOpenSound != null)
            audioSource.PlayOneShot(envelopeOpenSound, sfxVolume);

        StartCoroutine(OpenSequence());
    }

    private IEnumerator OpenSequence()
    {
        // انیمیشن محو شدن پاکت
        CanvasGroup envelopeCg = EnsureCanvasGroup(envelopeButton.gameObject);
        Vector3 startScale = envelopeRect.localScale;
        Vector3 endScale = startScale * 1.1f;

        float timer = 0f;
        while (timer < envelopeAnimDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / envelopeAnimDuration);
            float eased = progress * progress * (3f - 2f * progress);

            envelopeRect.localScale = Vector3.Lerp(startScale, endScale, eased);
            envelopeCg.alpha = Mathf.Lerp(1f, 0f, eased);
            yield return null;
        }

        envelopeButton.gameObject.SetActive(false);

        // نامه + متنش با هم روشن و انیمیت میشن
        letterObject.SetActive(true);
        if (letterText != null)
            letterText.gameObject.SetActive(true);

        CanvasGroup letterCg = EnsureCanvasGroup(letterObject);
        letterCg.blocksRaycasts = true;
        letterCg.interactable = true;

        timer = 0f;
        while (timer < letterAnimDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / letterAnimDuration);
            float eased = progress * progress * (3f - 2f * progress);

            letterObject.transform.localScale = Vector3.Lerp(Vector3.zero, letterFinalScale, eased);
            letterCg.alpha = Mathf.Lerp(0f, 1f, eased);
            yield return null;
        }

        letterObject.transform.localScale = letterFinalScale;
        letterCg.alpha = 1f;

        yield return StartCoroutine(TypeLetterText());

        // بعد از تموم شدن تایپ، 5 ثانیه صبر و بعد منتظر کلیک برای رفتن به صحنه بعد
        yield return new WaitForSeconds(clickDelay);
        canClickToContinue = true;
    }

    private IEnumerator TypeLetterText()
    {
        if (letterText == null) yield break;

        letterText.text = "";
        if (typingSound != null)
            audioSource.PlayOneShot(typingSound, typingVolume);

        for (int i = 1; i <= letterMessage.Length; i++)
        {
            letterText.text = letterMessage.Substring(0, i);
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private CanvasGroup EnsureCanvasGroup(GameObject go)
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        if (cg == null) cg = go.AddComponent<CanvasGroup>();
        return cg;
    }
}