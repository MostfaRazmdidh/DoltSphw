using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{
    [System.Serializable]
    public class CharacterOption
    {
        public string characterId;   // مثلا "candidate_1"
        public Button characterButton; // خود کاراکتر (char_1 تا char_4)
    }

    [Header("کاراکترها")]
    public CharacterOption[] characters;

    [Header("دکمه‌های تایید (دو حالت)")]
    public GameObject antecabNo;   // دکمه خاکستری، غیرفعال، پیش‌فرض روشن
    public Button antecab;         // دکمه سبز، فعال، پیش‌فرض خاموش

    [Header("فلش نشانگر کاراکتر انتخابی")]
    public RectTransform craecter;
    public float craecterYOffset = 40f; // فاصله فلش از بالای کاراکتر

    [Header("صدا")]
    public AudioClip selectSound;
    public AudioClip confirmSound;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    [Header("صحنه بعدی")]
    public string nextScene = "Level1_m1_R2";

    private const string SaveKey = "SelectedCharacterId";

    // برای خوندن سریع توی اسکریپت‌های بعدی همون سشن
    public static string SelectedCharacterId { get; private set; }

    private int selectedIndex = -1;

    void Start()
    {
        // حالت اولیه
        antecabNo.SetActive(true);
        antecab.gameObject.SetActive(false);
        craecter.gameObject.SetActive(false);

        for (int i = 0; i < characters.Length; i++)
        {
            int index = i; // جلوگیری از closure bug
            characters[i].characterButton.onClick.AddListener(() => OnCharacterClicked(index));
        }

        antecab.onClick.AddListener(OnConfirmClicked);
    }

    private void OnCharacterClicked(int index)
    {
        if (selectSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySound(selectSound, sfxVolume);

        selectedIndex = index;

        // جابجایی فلش بالای سر کاراکتر انتخاب‌شده
        RectTransform charRect = characters[index].characterButton.GetComponent<RectTransform>();
        craecter.gameObject.SetActive(true);
        craecter.position = new Vector3(charRect.position.x, charRect.position.y + charRect.rect.height / 2f + craecterYOffset, charRect.position.z);

        // سوییچ بین دکمه‌ی خاکستری و سبز
        antecabNo.SetActive(false);
        antecab.gameObject.SetActive(true);

        // ذخیره انتخاب
        SelectedCharacterId = characters[selectedIndex].characterId;
        PlayerPrefs.SetString(SaveKey, SelectedCharacterId);
        PlayerPrefs.Save();
    }

    private void OnConfirmClicked()
    {
        if (selectedIndex < 0) return;

        if (confirmSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySound(confirmSound, sfxVolume);

        // به‌جای SceneManager.LoadScene مستقیم، از انیمیشن در استفاده می‌کنیم
        if (DoorTransition.Instance != null)
            DoorTransition.Instance.GoToScene(nextScene);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }
}