using UnityEngine;

public class MainMenuCharacterBackground : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundOption
    {
        [Tooltip("دقیقا همون characterId که تو Level1_m1_R1 / CharacterSelectManager تعریف شده")]
        public string characterId;
        public GameObject backgroundObject;
    }

    [Header("پس‌زمینه ثابت (میز/دفتر) - همیشه فعاله، اختیاری")]
    public GameObject baseBackground;

    [Header("پس‌زمینه‌های مخصوص هر کاراکتر (فقط یکی فعال میشه)")]
    public BackgroundOption[] characterBackgrounds;

    [Header("پیش‌فرض (وقتی هنوز کاراکتری انتخاب نشده، مثلا بار اول بازی)")]
    public GameObject fallbackBackground;

    private const string SaveKey = "SelectedCharacterId";

    void Start()
    {
        if (baseBackground != null)
            baseBackground.SetActive(true);

        // اول همه پس‌زمینه‌های کاراکتری رو خاموش می‌کنیم
        foreach (var bg in characterBackgrounds)
        {
            if (bg.backgroundObject != null)
                bg.backgroundObject.SetActive(false);
        }

        if (fallbackBackground != null)
            fallbackBackground.SetActive(false);

        string selectedId = PlayerPrefs.GetString(SaveKey, "");

        // اگه هنوز هیچی انتخاب نشده (مثلا بار اول اجرای بازی)، فقط پیش‌فرض رو نشون بده، بدون وارنینگ
        if (string.IsNullOrEmpty(selectedId))
        {
            if (fallbackBackground != null)
                fallbackBackground.SetActive(true);
            return;
        }

        bool found = false;
        foreach (var bg in characterBackgrounds)
        {
            if (bg.characterId == selectedId)
            {
                bg.backgroundObject.SetActive(true);
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.LogWarning("MainMenuCharacterBackground: کاراکتر انتخاب‌شده پیدا نشد (" + selectedId + ")");
            if (fallbackBackground != null)
                fallbackBackground.SetActive(true);
        }
    }
}