using UnityEngine;
using TMPro;
using System.IO;

// Solution alternative qui fonctionne avec Unity Remote
public class TabletScript : MonoBehaviour
{
    public TMP_Text phaseDisplayText;
    private string flagFilePath;
    private float timeTouchEnded;
    private float displayTime = 0.5f;
    private float lastCheckTime = 0f;
    private float checkInterval = 1f;

    void Start()
    {
        flagFilePath = Path.Combine(Application.persistentDataPath, "flag.txt");
        phaseDisplayText.text = "En attente...";
        Debug.Log("Chemin du fichier flag : " + flagFilePath);
    }

    void Update()
    {
       
        if (Time.time - lastCheckTime > checkInterval)
        {
            lastCheckTime = Time.time;
            CheckFlagFile();
        }
        
     
        
        // Support souris pour test PC
        if (Input.GetMouseButtonDown(0))
        {
            phaseDisplayText.text = "Clic souris !";
            timeTouchEnded = Time.time;
        }
        
      
        void CheckFlagFile()
        {
        
            if (File.Exists(flagFilePath))
            {
                string content = File.ReadAllText(flagFilePath).Trim();
                Debug.Log("Contenu du flag : " + content);
                if (content == "1")
                    phaseDisplayText.text = "FLAG DÉTECTÉ ✅";
                else
                    phaseDisplayText.text = "FLAG NON VALIDE ❌";
            }
            else
            {
                Debug.LogWarning("Fichier flag introuvable : " + flagFilePath);
            }
        }
    }
}