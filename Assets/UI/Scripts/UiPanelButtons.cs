using UnityEngine;
using UnityEngine.UI;

public class UiPannelButtons : MonoBehaviour
{
    [Header("?? Grid")]
    public GridLayoutGroup gridLayout;

    [Header("?? Buttons")]
    public UiButton[] buttons = new UiButton[4];

    private void Awake()
    {
        if (gridLayout == null)
            gridLayout = GetComponent<GridLayoutGroup>();

        if (buttons == null || buttons.Length == 0)
            buttons = GetComponentsInChildren<UiButton>();
    }

    private void Start()
    {
        // Exemple : abonne un comportement à chaque bouton
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].button.onClick.AddListener(() => OnButtonClicked(index));
        }
    }

    private void OnButtonClicked(int index)
    {
        Debug.Log($"?? Bouton {index + 1} cliqué !");
        // Ici tu peux ajouter ton code : ouvrir un menu, lancer une action, etc.
    }
}