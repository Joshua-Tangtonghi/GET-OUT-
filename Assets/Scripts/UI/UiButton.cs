using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiButton : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Button button;         // le vrai bouton
    [SerializeField] private TMP_Text buttonText;   // le texte du bouton

    private void Awake()
    {
        // 🔹 Assignation automatique si vide
        if (button == null)
            button = GetComponent<Button>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();

        if (button == null)
            Debug.LogError($"❌ Aucun Button trouvé sur {gameObject.name}");
        if (buttonText == null)
            Debug.LogError($"❌ Aucun TMP_Text trouvé sur {gameObject.name}");
    }

    // Permet à UiPannelButtons d'accéder au composant Button
    public Button GetButton()
    {
        return button;
    }

    // Permet de changer le texte du bouton
    public void SetButtonText(string text)
    {
        if (buttonText != null)
            buttonText.text = text;
        else
            Debug.LogWarning($"⚠️ TMP_Text manquant sur {gameObject.name}");
    }
}