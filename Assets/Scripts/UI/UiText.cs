using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiText : MonoBehaviour
{
    public TMP_Text text;

    private void Awake()
    {
        if (text == null )
        {
            text = GetComponentInChildren<TMP_Text>();
        }
    }
    public void SetText(string t)
    {
        text.text = t;
    }
    public string GetText()
        {
        return text.text;
        }
    public void EraseText()
    {
        SetText(string.Empty);
    }
}
