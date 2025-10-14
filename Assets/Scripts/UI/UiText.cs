using System.Collections;
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
        StopAllCoroutines();
        StartCoroutine(TypeTextCoroutine(t, f));
    }

    private IEnumerator TypeTextCoroutine(string t, float f)
    {
        text.text = "";  // on vide avant d’écrire
        for (int i = 0; i < t.Length; i++)
        {
            text.text += t[i];
            yield return new WaitForSeconds(f);
        }
    }
}