using TMPro;
using UnityEngine;

public class UiPannelButtons : MonoBehaviour
{
    [Header("Buttons")]
    UiButton[] buttons = new UiButton[4];

    TMP_Text questionText;
    private int answer;
    private void Awake()
    {
        if (buttons == null || buttons.Length == 0) ;
        {
            buttons = GetComponentsInChildren<UiButton>();
        }
        if (questionText == null)
        {
            questionText = GetComponentInChildren<TMP_Text>();
        }
    }
    private void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetButton().onClick.AddListener(() => OnButtonClicked(i));
        }
    }
    private void OnButtonClicked(int index)
    {
        Debug.Log($"?? Bouton {index + 1} cliqué !");
        answer = index;
    }
    public int GetPlayerAnswer()
    {
        return answer;
    }
    public void SetQuestion(string setText)
    {
        questionText.text = setText;
    }
    public void SetButtonsText(string setText)
    {
        foreach (UiButton b in buttons)
        {
            b.SetButtonText(setText);
        }
    }
}