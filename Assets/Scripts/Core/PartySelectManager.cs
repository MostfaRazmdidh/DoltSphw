using UnityEngine;
using UnityEngine.UI;

public class PartySelectManager : MonoBehaviour
{
    [System.Serializable]
    public class PartyOption
    {
        public string partyId;      // مثلا "osoulgara" یا "eslahtalab"
        public Button partyButton;  // دکمه/عکس همون حزب
    }

    [Header("احزاب")]
    public PartyOption[] parties;

    [Header("صدا")]
    public AudioClip selectSound;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    [Header("صحنه بعدی")]
    public string nextScene = "Level1_m1_R3";

    private const string SaveKey = "SelectedParty";

    public static string SelectedParty { get; private set; }

    void Start()
    {
        for (int i = 0; i < parties.Length; i++)
        {
            int index = i; // جلوگیری از closure bug
            parties[i].partyButton.onClick.AddListener(() => OnPartyClicked(index));
        }
    }

    private void OnPartyClicked(int index)
    {
        if (selectSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySound(selectSound, sfxVolume);

        SelectedParty = parties[index].partyId;
        PlayerPrefs.SetString(SaveKey, SelectedParty);
        PlayerPrefs.Save();

        // مستقیم و بدون دکمه تایید جدا میره صحنه بعدی
        if (DoorTransition.Instance != null)
            DoorTransition.Instance.GoToScene(nextScene);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }
}