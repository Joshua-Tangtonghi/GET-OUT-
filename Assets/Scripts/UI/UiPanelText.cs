using UnityEngine;

public class UiPanelText : MonoBehaviour
{
    public UiText panelText;
    private Animator pAnim;

    private void Awake()
    {
        if (panelText == null)
            panelText = GetComponentInChildren<UiText>();
        pAnim = GetComponent<Animator>();
    }
    public void SetPanelText(string t)
    {
        panelText.SetText(t);
    }
    public void SetPanelText(string t, float f)
    {  panelText.SetText(t, f); }
    public void PanelTextVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }
    public void PanelTextAnim(bool up)
    {
        if (up)
        {
            pAnim.SetTrigger("Open");
        }
        else
        {
            pAnim.SetTrigger("Close");
        }
    }
}
