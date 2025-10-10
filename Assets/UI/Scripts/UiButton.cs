using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class UiButton : MonoBehaviour
{
    public Button button;
    public TMP_Text text;

    [Header("🔊 Sound")]
    public AudioClip customClickSound;

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponentInChildren<TMP_Text>();
        }
        GetButton().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // 🔸 Joue le son
        if (SoundManager.Instance)
        {
            if (customClickSound != null)
                SoundManager.Instance.PlaySFX(customClickSound);
            else
                SoundManager.Instance.PlayButtonClick();
        }
    }
    public Button GetButton()
    {
        return button = GetComponentInChildren<Button>();
    }
    public TMP_Text GetButtonText()
        { return text; }
    public void SetButtonText(string setText)
    {
        GetButtonText().text = setText;
    }
}