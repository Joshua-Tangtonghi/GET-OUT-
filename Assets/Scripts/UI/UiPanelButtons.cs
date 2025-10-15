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
        Debug.Log("🔧 UiPannelButtons Awake START");
        
        if (buttons == null || buttons.Length == 0)
        {
            buttons = GetComponentsInChildren<UiButton>(true);
            Debug.Log($"✅ {buttons.Length} boutons trouvés");
        }

        questionText = GetComponentInChildren<TMP_Text>(true);
        if (questionText != null)
            Debug.Log($"✅ QuestionText trouvé: {questionText.name}");
        else
            Debug.LogError("❌ Aucun TMP_Text trouvé pour la question!");

        bAnim = GetComponent<Animator>();
        if (bAnim != null)
            Debug.Log("✅ Animator trouvé");
        
        Debug.Log("🔧 UiPannelButtons Awake END");
    }

    private void Start()
    {
        Debug.Log("🔧 UiPannelButtons Start - Configuration des listeners");
        
        if (buttons == null || buttons.Length == 0)
        {
            Debug.LogError("❌ Aucun bouton disponible dans Start!");
            return;
        }
        
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                Debug.LogError($"❌ Button[{i}] est NULL!");
                continue;
            }
            
            var btn = buttons[i].GetButton();
            if (btn == null)
            {
                Debug.LogError($"❌ GetButton() retourne NULL pour bouton {i}!");
                continue;
            }
            
            int index = i; // closure fixée
            btn.onClick.AddListener(() => OnButtonClicked(index));
            Debug.Log($"✅ Listener ajouté au bouton {index} ({buttons[i].name})");
        }
        
        Debug.Log($"🎮 {buttons.Length} boutons configurés avec succès");
    }

    private void OnButtonClicked(int index)
    {
        Debug.Log($"🔘🔘🔘 BOUTON {index + 1} CLIQUÉ ! 🔘🔘🔘");
        answer = index;
    }

    public int GetPlayerAnswer() 
    { 
        return answer;
    }

    public void ResetAnswer()
    {
        answer = -1;
        Debug.Log("🔄 Réponse réinitialisée à -1");
    }

    public void SetQuestion(string text)
    {
        if (questionText != null)
        {
            questionText.text = text;
            Debug.Log($"✅ Question définie: {text.Substring(0, Mathf.Min(50, text.Length))}...");
        }
        else
        {
            Debug.LogError("❌ Impossible de définir la question - questionText est NULL!");
        }
    }

    public void SetButtonsText(string[] texts)
    {
        Debug.Log($"🔤 SetButtonsText appelé avec {texts.Length} textes");
        
        for (int i = 0; i < buttons.Length && i < texts.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].SetButtonText(texts[i]);
                Debug.Log($"✅ Bouton {i} texte: '{texts[i]}'");
            }
            else
            {
                Debug.LogError($"❌ Bouton {i} est NULL!");
            }
        }
    }

    public void ButtonPanelVisibility(bool visible)
    {
        Debug.Log($"👁️ ButtonPanelVisibility appelé: {visible}");
        
        if (bAnim != null)
        {
            bAnim.SetTrigger(visible ? "Open" : "Close");
            Debug.Log($"✅ Animation trigger: {(visible ? "Open" : "Close")}");
        }
        
        // S'assurer que le gameObject est actif
        if (!gameObject.activeSelf && visible)
        {
            gameObject.SetActive(true);
            Debug.Log("✅ GameObject activé manuellement");
        }
        
        // Vérifier l'état des boutons
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                var btn = buttons[i].GetButton();
                Debug.Log($"📊 Bouton {i}: Active={btn.gameObject.activeSelf}, Interactable={btn.interactable}");
            }
        }
    }
}   