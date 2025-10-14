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
    public void SetText(string t, float f)
    {
        for (int i = 0; i < text.text.Length; i++)
        {
            text.text += t[i];
            new WaitForSeconds(f);
        }
    }
}