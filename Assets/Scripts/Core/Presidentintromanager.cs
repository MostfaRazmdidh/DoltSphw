using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using RTLTMPro;
using System.Collections;

public class PresidentIntroManager : MonoBehaviour
{
    [Header("UI - دیالوگ")]
    public GameObject dialogBox;
    public RTLTextMeshPro dialogText;

    [Header("متن دیالوگ")]
    [TextArea(3, 6)]
    public string dialog = "جناب رئیس‌جمهور، مسئولیت هدایت کشور اکنون بر عهده شماست. کشور با چالش‌های فراوانی روبه‌روست؛ پیش از هر اقدامی، گزارش وضعیت کشور را بررسی کنیم.";

    [Header("تنظیمات تایپ")]
    public float typingSpeed = 0.05f;
    public AudioClip typingSound;
    [Range(0f, 1f)] public float typingVolume = 0.5f;

    [Header("صدای دیالوگ (ساند افکت گفتار)")]
    public AudioClip voiceSound;
    [Range(0f, 1f)] public float voiceVolume = 0.8f;

    [Header("زمان‌بندی مرحله اول (بعد از شروع تایپ)")]
    public float clickDelayAfterStart = 5f;

    [Header("UI - مانیتور")]
    public Image monitorImage;
    [Range(0f, 2f)] public float monitorFadeDuration = 0.5f;

    [Header("زمان‌بندی مرحله دوم (بعد از ظاهر شدن عکس)")]
    public float clickDelayAfterImage = 3f;

    [Header("UI - دکمه بازگشت")]
    public GameObject backButtonObject;
    public Button backButton;
    [Range(0f, 2f)] public float backButtonFadeDuration = 0.4f;

    [Header("صحنه بعدی")]
    public string nextScene = "MainMenu";

    private AudioSource typingAudioSource;
    private AudioSource voiceAudioSource;

    private enum Stage { Typing, WaitingFirstTap, ShowingImage, WaitingSecondTap, Done }
    private Stage currentStage = Stage.Typing;

    void OnEnable()
    {
        if (!EnhancedTouchSupport.enabled)
            EnhancedTouchSupport.Enable();
    }

    void Start()
    {
        typingAudioSource = gameObject.AddComponent<AudioSource>();
        voiceAudioSource = gameObject.AddComponent<AudioSource>();

        // دیالوگ‌باکس
        dialogBox.SetActive(true);

        // مانیتور - اول خالی
        CanvasGroup monitorCg = EnsureCanvasGroup(monitorImage.gameObject);
        monitorCg.alpha = 0f;
        monitorImage.gameObject.SetActive(false);

        // دکمه بازگشت - اول غیرفعال و مخفی
        CanvasGroup backCg = EnsureCanvasGroup(backButtonObject);
        backCg.alpha = 0f;
        backCg.interactable = false;
        backCg.blocksRaycasts = false;
        backButtonObject.SetActive(false);

        backButton.onClick.AddListener(OnBackButtonClicked);

        // شروع فوری دیالوگ
        if (voiceSound != null)
            voiceAudioSource.PlayOneShot(voiceSound, voiceVolume);

        StartCoroutine(TypeText());
        StartCoroutine(EnableFirstTapAfterDelay());
    }

    void Update()
    {
        bool tapped = IsTapped();
        if (!tapped) return;

        if (currentStage == Stage.WaitingFirstTap)
        {
            currentStage = Stage.ShowingImage;
            OnFirstTap();
        }
        else if (currentStage == Stage.WaitingSecondTap)
        {
            currentStage = Stage.Done;
            OnSecondTap();
        }
    }

    private bool IsTapped()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) return true;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) return true;
        return false;
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

    private IEnumerator EnableFirstTapAfterDelay()
    {
        yield return new WaitForSeconds(clickDelayAfterStart);
        currentStage = Stage.WaitingFirstTap;
    }

    private void OnFirstTap()
    {
        // قطع متن و صدا
        StopAllCoroutines(); // متوقف کردن تایپ در صورت ادامه داشتن
        typingAudioSource.Stop();
        voiceAudioSource.Stop();
        dialogBox.SetActive(false);

        StartCoroutine(ShowMonitorImage());
    }

    private IEnumerator ShowMonitorImage()
    {
        monitorImage.gameObject.SetActive(true);
        CanvasGroup monitorCg = EnsureCanvasGroup(monitorImage.gameObject);

        float timer = 0f;
        while (timer < monitorFadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / monitorFadeDuration);
            monitorCg.alpha = Mathf.Lerp(0f, 1f, t * t * (3f - 2f * t));
            yield return null;
        }
        monitorCg.alpha = 1f;

        yield return new WaitForSeconds(clickDelayAfterImage);
        currentStage = Stage.WaitingSecondTap;
    }

    private void OnSecondTap()
    {
        StartCoroutine(ShowBackButton());
    }

    private IEnumerator ShowBackButton()
    {
        backButtonObject.SetActive(true);
        CanvasGroup backCg = EnsureCanvasGroup(backButtonObject);
        backCg.interactable = true;
        backCg.blocksRaycasts = true;

        float timer = 0f;
        while (timer < backButtonFadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / backButtonFadeDuration);
            backCg.alpha = Mathf.Lerp(0f, 1f, t * t * (3f - 2f * t));
            yield return null;
        }
        backCg.alpha = 1f;
    }

    private void OnBackButtonClicked()
    {
        if (DoorTransition.Instance != null)
            DoorTransition.Instance.GoToScene(nextScene);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }

    private CanvasGroup EnsureCanvasGroup(GameObject go)
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        if (cg == null) cg = go.AddComponent<CanvasGroup>();
        return cg;
    }
}