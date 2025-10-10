using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UiButton : MonoBehaviour
{
    public Button button;

    [Header("🎬 Animator")]
    public Animator animator;
    public string clickTrigger = "Click";

    [Header("🔊 Sound")]
    public AudioClip customClickSound;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();

        if (animator == null)
            animator = GetComponent<Animator>();

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // 🔸 Joue l’animation
        if (animator && !string.IsNullOrEmpty(clickTrigger))
            animator.SetTrigger(clickTrigger);

        // 🔸 Joue le son
        if (SoundManager.Instance)
        {
            if (customClickSound != null)
                SoundManager.Instance.PlaySFX(customClickSound);
            else
                SoundManager.Instance.PlayButtonClick();
        }
    }
}