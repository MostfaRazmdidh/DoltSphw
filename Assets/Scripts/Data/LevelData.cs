using UnityEngine;

// این attribute باعث میشه بتونیم از منوی یونیتی این نوع داده رو بسازیم
[CreateAssetMenu(fileName = "Level_00", menuName = "DowlatSho/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("اطلاعات مرحله")]
    public int levelNumber;           // شماره مرحله (۱ تا ۹۸)
    public string levelTitle;         // عنوان مرحله، مثلاً «بحران اقتصادی»

    [Header("متن رویداد")]
    [TextArea(3, 6)]
    public string eventText;          // متنی که به بازیکن نمایش داده میشه

    [Header("گزینه‌های انتخاب")]
    public ChoiceData[] choices;      // آرایه‌ای از گزینه‌ها (۲ یا ۳ تا)
}

// این کلاس داده‌ی هر گزینه رو نگه میداره
[System.Serializable]
public class ChoiceData
{
    public string choiceText;         // متن دکمه، مثلاً «افزایش مالیات»

    [TextArea(2, 4)]
    public string outcomeText;        // نتیجه‌ای که بعد از انتخاب نشون داده میشه

    public int rialReward;            // مقدار ریالی که بازیکن بدست میاره
}