using TMPro;
using UnityEngine;

public class UiPannelButtons : MonoBehaviour
{
    [Header("Buttons")]
    public UiButton[] buttons;

    private TMP_Text questionText;
    private Animator bAnim;
    private int answer = -1;

    private void Awake()
    {
        if (buttons == null || buttons.Length == 0)
            buttons = GetComponentsInChildren<UiButton>(true);

        questionText = GetComponentInChildren<TMP_Text>(true);
        bAnim = GetComponent<Animator>();
    }

    private void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // closure fixée
            buttons[i].GetButton().onClick.AddListener(() => OnButtonClicked(index));
        }
    }

    private void OnButtonClicked(int index)
    {
        Debug.Log($"✅ Bouton {index + 1} cliqué !");
        answer = index;
    }

    public int GetPlayerAnswer() => answer;

    public void SetQuestion(string text)
    {
        if (questionText != null)
            questionText.text = text;
    }

    public void SetButtonsText(string[] texts)
    {
        for (int i = 0; i < buttons.Length && i < texts.Length; i++)
        {
            buttons[i].SetButtonText(texts[i]);
        }
    }

    public void ButtonPanelVisibility(bool visible)
    {
        if (bAnim != null)
            bAnim.SetTrigger(visible ? "Open" : "Close");

        gameObject.SetActive(visible);
    }
}