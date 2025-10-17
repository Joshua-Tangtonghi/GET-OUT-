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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PushFlag("umbrella.flag");
            statusText.text = "énigme parapluie envoyée !";
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PushFlag("ball.flag");
            statusText.text = "énigme balle envoyée !";
        }


        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PushFlag("maze.flag");
            statusText.text = "énigme labyrinthe envoyée !";
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PushFlag("code.flag");
            statusText.text = "énigme code envoyée !";
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetFlags();
            statusText.text = "drapeau clé envoyé !";
        }

        {
        }
    }

    void PushFlag(string fileName)
    {
        // Créer un fichier temporaire vide sur PC
        string tempPath = Path.Combine(Application.temporaryCachePath, fileName);
        File.WriteAllText(tempPath, "1"); // contenu optionnel

        // Pousser dans le dossier persistentDataPath de l'app sur la tablette
        string androidPath =
            $"/storage/emulated/0/Android/data/com.UnityTechnologies.com.unity.template.urpblank/files/{fileName}";
        ExecuteADB($"push \"{tempPath}\" {androidPath}");
    }

    void ResetFlags()
    {
        string[] flags =
            { "key.flag", "umbrella.flag", "ball.flag", "codeUV.flag", "maze.flag", "code.flag", "captcha.flag" };
        foreach (var flag in flags)
        {
            string tempPath = Path.Combine(Application.temporaryCachePath, flag);
            if (File.Exists(tempPath))
                File.Delete(tempPath);

            string androidPath =
                $"/storage/emulated/0/Android/data/com.UnityTechnologies.com.unity.template.urpblank/files/{flag}";
            ExecuteADB($"shell rm {androidPath}");
        }
    }

    void ExecuteADB(string arguments)
    {
        try
        {
            // Chemin relatif vers adb
            string adbPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "platform-tools",
                "adb.exe");

            if (!File.Exists(adbPath))
            {
                UnityEngine.Debug.LogError($" ADB introuvable à l'emplacement : {adbPath}");
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = adbPath,
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

            if (!string.IsNullOrEmpty(output))
                UnityEngine.Debug.Log($"[ADB OUT] {output}");

            if (!string.IsNullOrEmpty(error) && !error.Contains("daemon"))
                UnityEngine.Debug.LogWarning($"[ADB ERR] {error}");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($" Erreur ADB: {e.Message}");
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, Screen.height - 80, 400, 80),
            "<size=20><color=white><b>Contrôles:</b>\nF = Flag1 | 1 = Bouton1 | 2 = Bouton2 | R = Reset</color></size>");
    }
}