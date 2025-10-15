using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiButton : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text buttonText;

    private void Awake()
    {
        Debug.Log($"🔧 UiButton.Awake() sur {gameObject.name}");
        
        // Assignation automatique si vide
        if (button == null)
        {
            button = GetComponent<Button>();
            Debug.Log($"   Auto-assignation Button: {(button != null ? "✅" : "❌")}");
        }

        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<TMP_Text>();
            Debug.Log($"   Auto-assignation TMP_Text: {(buttonText != null ? "✅" : "❌")}");
        }

        if (button == null)
            Debug.LogError($"❌ Aucun Button trouvé sur {gameObject.name}");
        else
            Debug.Log($"✅ Button OK sur {gameObject.name}, Interactable: {button.interactable}");
            
        if (buttonText == null)
            Debug.LogError($"❌ Aucun TMP_Text trouvé sur {gameObject.name}");
        else
            Debug.Log($"✅ TMP_Text OK sur {gameObject.name}");
    }

    private void Start()
    {
        // TEST: Ajouter un listener de test directement ici
        if (button != null)
        {
            button.onClick.AddListener(TestClick);
            Debug.Log($"🎯 Listener de TEST ajouté sur {gameObject.name}");
        }
    }

    private void TestClick()
    {
        Debug.Log($"🎉🎉🎉 BOUTON {gameObject.name} CLIQUÉ (TEST LISTENER) 🎉🎉🎉");
    }

    public Button GetButton()
    {
        return button;
    }

    public void SetButtonText(string text)
    {
        if (buttonText != null)
        {
            buttonText.text = text;
            Debug.Log($"✅ Texte du bouton {gameObject.name} changé en: '{text}'");
        }
        else
        {
            Debug.LogWarning($"⚠️ TMP_Text manquant sur {gameObject.name}");
        }
    }
    
    private void OnEnable()
    {
        Debug.Log($"👁️ {gameObject.name} activé (OnEnable)");
    }
    
    private void OnDisable()
    {
        Debug.Log($"🚫 {gameObject.name} désactivé (OnDisable)");
    }
}