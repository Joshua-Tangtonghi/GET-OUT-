using UnityEngine;
using TMPro;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PCController : MonoBehaviour
{
    public TMP_Text statusText;

    void Start()
    {
        statusText.text = "🖥️ Mode PC (ADB)";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            PushFlag("flag1.flag");
            statusText.text = "🚩 Flag 1 envoyé !";
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PushFlag("button1.flag");
            statusText.text = "🔵 Bouton 1 envoyé !";
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PushFlag("button2.flag");
            statusText.text = "🔴 Bouton 2 envoyé !";
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetFlags();
            statusText.text = "🔄 Flags réinitialisés";
        }
    }

    void PushFlag(string fileName)
    {
        // Créer un fichier temporaire vide sur PC
        string tempPath = Path.Combine(Application.temporaryCachePath, fileName);
        File.WriteAllText(tempPath, "1"); // contenu optionnel

        // Pousser dans le dossier persistentDataPath de l'app sur la tablette
        string androidPath = $"/storage/emulated/0/Android/data/com.tonapp/files/{fileName}";
        ExecuteADB($"push \"{tempPath}\" {androidPath}");
    }

    void ResetFlags()
    {
        string[] flags = { "flag1.flag", "button1.flag", "button2.flag" };
        foreach (var flag in flags)
        {
            string tempPath = Path.Combine(Application.temporaryCachePath, flag);
            if (File.Exists(tempPath))
                File.Delete(tempPath);

            string androidPath = $"/storage/emulated/0/Android/data/com.tonapp/files/{flag}";
            ExecuteADB($"shell rm {androidPath}");
        }
    }

    void ExecuteADB(string arguments)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "adb", // ADB doit être dans le PATH Windows
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            Process process = Process.Start(startInfo);
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error) && !error.Contains("daemon"))
                Debug.LogWarning("ADB: " + error);
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Erreur ADB: " + e.Message);
            statusText.text = "Erreur ADB !";
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, Screen.height - 80, 400, 80),
            "<size=20><color=white><b>Contrôles:</b>\nF = Flag1 | 1 = Bouton1 | 2 = Bouton2 | R = Reset</color></size>");
    }
}
