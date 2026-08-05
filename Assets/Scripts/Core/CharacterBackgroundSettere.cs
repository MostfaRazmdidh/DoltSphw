using UnityEngine;

public class CharacterBackgroundSetter : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundOption
    {
        public string characterId;       // باید دقیقا مثل characterId هایی باشه که تو Level1_m1_R1 تعریف کردی
        public GameObject backgroundObject; // آبجکت پس‌زمینه مخصوص همون کاراکتر
    }

    [Header("پس‌زمینه‌های مخصوص هر کاراکتر")]
    public BackgroundOption[] backgrounds;

    [Header("پیش‌فرض (اگه چیزی پیدا نشد)")]
    public GameObject fallbackBackground;

    private const string SaveKey = "SelectedCharacterId";

    void Start()
    {
        // اول همه پس‌زمینه‌ها رو خاموش می‌کنیم
        foreach (var bg in backgrounds)
        {
            if (bg.backgroundObject != null)
                bg.backgroundObject.SetActive(false);
        }

        if (fallbackBackground != null)
            fallbackBackground.SetActive(false);

        string selectedId = PlayerPrefs.GetString(SaveKey, "");

        bool found = false;
        foreach (var bg in backgrounds)
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
            Debug.LogWarning("CharacterBackgroundSetter: کاراکتر انتخاب‌شده پیدا نشد (" + selectedId + ")، پس‌زمینه پیش‌فرض نمایش داده می‌شود.");
            if (fallbackBackground != null)
                fallbackBackground.SetActive(true);
        }
    }
}