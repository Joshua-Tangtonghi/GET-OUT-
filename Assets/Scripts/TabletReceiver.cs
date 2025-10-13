using UnityEngine;
using TMPro;
using System.IO;

public class TabletReceiver : MonoBehaviour
{
    public TMP_Text statusText;

    [Header("Configuration")]
    public float checkInterval = 0.5f;  // vérification toutes les 0.5s
    public float displayTime = 1f;      // temps d'affichage avant retour "En attente..."

    private float nextCheckTime = 0f;
    private float lastActionTime = 0f;

    // Chemins exacts des fichiers flag sur la tablette
    private string basePath = "/storage/emulated/0/Android/data/com.tonapp/files/";
    private string flag1Path;
    private string button1Path;
    private string button2Path;

    void Start()
    {
        flag1Path = Path.Combine(basePath, "flag1.flag");
        button1Path = Path.Combine(basePath, "button1.flag");
        button2Path = Path.Combine(basePath, "button2.flag");

        statusText.text = "En attente...";
    }

    void Update()
    {
        // Vérifier les fichiers à intervalle régulier
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            CheckFlags();
        }

        // Touchscreen actif
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                statusText.text = "✋ Touch détecté !";
                lastActionTime = Time.time;
            }
        }

        // Réinitialiser le texte après displayTime
        if (Time.time - lastActionTime > displayTime)
        {
            statusText.text = "En attente...";
        }
    }

    void CheckFlags()
    {
        // FLAG 1
        if (File.Exists(flag1Path))
        {
            statusText.text = "🚩 FLAG 1 ACTIVÉ !";
            lastActionTime = Time.time;
            File.Delete(flag1Path); // supprimer pour trigger qu’une seule fois
        }

        // BOUTON 1
        if (File.Exists(button1Path))
        {
            statusText.text = "🔵 Bouton 1 !";
            lastActionTime = Time.time;
            File.Delete(button1Path);
        }

        // BOUTON 2
        if (File.Exists(button2Path))
        {
            statusText.text = "🔴 Bouton 2 !";
            lastActionTime = Time.time;
            File.Delete(button2Path);
        }
    }
}
