using UnityEngine;
using UnityEngine.UI;

public class PannelQuestion : MonoBehaviour
{
    private Button[] buttons;
    private int selectedIndex = -1;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonClicked(index));
        }
    }

    private void OnButtonClicked(int index)
    {
        if (selectedIndex == index) return;

        selectedIndex = index;

        Debug.Log("Bouton s�lectionn� : " + selectedIndex);

        // Tu peux g�rer la mise � jour visuelle toi-m�me ici ou ailleurs
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    public void ResetSelection()
    {
        selectedIndex = -1;
        // Mise � jour visuelle � faire de ton c�t�
    }
}