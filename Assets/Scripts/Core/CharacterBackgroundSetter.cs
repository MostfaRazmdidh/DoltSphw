using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections;

public class InterviewManager : MonoBehaviour
{
    [System.Serializable]
    public class QuestionBox
    {
        [Tooltip("خود آبجکت TextBox1 / TextBox2 / TextBox3")]
        public GameObject boxObject;

        [Tooltip("دکمه‌های جواب همین سوال (Javab1, Javab2, Javab3)")]
        public Button[] answerButtons;

        [Tooltip("صدای دیالوگ مصاحبه‌گر مخصوص همین سوال")]
        public AudioClip dialogueSound;
    }

    [Header("۳ سوال، به ترتیب")]
    public QuestionBox[] questions;

    [Header("صدای کلیک روی گزینه‌ها")]
    public AudioClip answerClickSound;
    [Range(0f, 1f)] public float answerClickVolume = 0.8f;

    [Header("انیمیشن تعویض سوال")]
    public float fadeDuration = 0.3f;
    public float scaleFrom = 0.85f;

    [Header("صحنه بعدی (بعد از سوال آخر)")]
    public string nextScene = "Level1_m1_R4";

    private AudioSource dialogueAudioSource;
    private bool waitingForFirstTap = true;
    private int currentIndex = -1;
    private Coroutine transitionCoroutine;

    void OnEnable()
    {
        if (!EnhancedTouchSupport.enabled)
            EnhancedTouchSupport.Enable();
    }

    void Start()
    {
        dialogueAudioSource = gameObject.AddComponent<AudioSource>();

        // همه سوالات اولش مخفی و کاملا شفاف/کوچیک باشن
        foreach (var q in questions)
        {
            EnsureCanvasGroup(q.boxObject).alpha = 0f;
            q.boxObject.transform.localScale = Vector3.one * scaleFrom;
            q.boxObject.SetActive(false);
        }

        // اتصال دکمه‌های هر سوال
        for (int qi = 0; qi < questions.Length; qi++)
        {
            int questionIndex = qi;
            for (int bi = 0; bi < questions[qi].answerButtons.Length; bi++)
            {
                questions[qi].answerButtons[bi].onClick.AddListener(() => OnAnswerClicked(questionIndex));
            }
        }
    }

    void Update()
    {
        if (!waitingForFirstTap) return;

        bool tapped = false;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            tapped = true;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            tapped = true;

        if (tapped)
        {
            waitingForFirstTap = false;
            ShowQuestion(0);
        }
    }

    private void ShowQuestion(int index)
    {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);

        int previousIndex = currentIndex;
        currentIndex = index;

        transitionCoroutine = StartCoroutine(TransitionTo(previousIndex, index));
    }

    private IEnumerator TransitionTo(int previousIndex, int newIndex)
    {
        // محو کردن سوال قبلی (اگه بود)
        if (previousIndex >= 0)
        {
            GameObject prevBox = questions[previousIndex].boxObject;
            CanvasGroup prevCg = EnsureCanvasGroup(prevBox);
            yield return StartCoroutine(FadeAndScale(prevCg, prevBox.transform, 1f, 0f, 1f, scaleFrom, fadeDuration));
            prevBox.SetActive(false);
        }

        // پخش صدای دیالوگ سوال جدید
        dialogueAudioSource.Stop();
        if (questions[newIndex].dialogueSound != null)
            dialogueAudioSource.PlayOneShot(questions[newIndex].dialogueSound);

        // نمایش سوال جدید
        GameObject newBox = questions[newIndex].boxObject;
        CanvasGroup newCg = EnsureCanvasGroup(newBox);
        newBox.SetActive(true);
        newBox.transform.localScale = Vector3.one * scaleFrom;
        newCg.alpha = 0f;

        yield return StartCoroutine(FadeAndScale(newCg, newBox.transform, 0f, 1f, scaleFrom, 1f, fadeDuration));
    }

    private IEnumerator FadeAndScale(CanvasGroup cg, Transform t, float fromAlpha, float toAlpha, float fromScale, float toScale, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);
            float eased = progress * progress * (3f - 2f * progress); // smoothstep

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

    private void OnAnswerClicked(int questionIndex)
    {
        // فقط جوابِ سوالِ فعلی معتبره (جلوگیری از کلیک تصادفی روی جواب قبلی/بعدی)
        if (questionIndex != currentIndex) return;

        if (answerClickSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySound(answerClickSound, answerClickVolume);

        // قطع فوری صدای دیالوگ در صورت پخش بودن
        dialogueAudioSource.Stop();

        int nextIndex = questionIndex + 1;
        if (nextIndex < questions.Length)
        {
            ShowQuestion(nextIndex);
        }
        else
        {
            StartCoroutine(GoToNextSceneAfterFade());
        }
    }

    private IEnumerator GoToNextSceneAfterFade()
    {
        GameObject lastBox = questions[currentIndex].boxObject;
        CanvasGroup cg = EnsureCanvasGroup(lastBox);
        yield return StartCoroutine(FadeAndScale(cg, lastBox.transform, 1f, 0f, 1f, scaleFrom, fadeDuration));
        lastBox.SetActive(false);

        if (DoorTransition.Instance != null)
            DoorTransition.Instance.GoToScene(nextScene);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }
}