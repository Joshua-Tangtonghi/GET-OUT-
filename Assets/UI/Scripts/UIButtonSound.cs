using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [Header("Optional: Custom Click Sound")]
    public AudioClip customClickSound;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (SoundManager.Instance == null) return;

        if (customClickSound != null)
        {
            SoundManager.Instance.PlaySFX(customClickSound);
        }
        else
        {
            SoundManager.Instance.PlayButtonClick(); // fallback to default
        }
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(PlayClickSound);
    }
}