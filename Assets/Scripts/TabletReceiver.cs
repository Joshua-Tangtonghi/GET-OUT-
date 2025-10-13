using UnityEngine;
using TMPro;
using System.IO;
using System.Collections;

public class TabletReceiver : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text statusText;
    public TMP_Text maxDialogText;
    
    [Header("Configuration")]
    public float checkInterval = 0.5f;
    public float displayTime = 2f;
    public int maxTouchBeforeGameOver = 5;
    
    [Header("Suspicion")]
    public float maxSuspicious = 10f;
    
    private float nextCheckTime = 0f;
    private float lastActionTime = 0f;
    private float suspicious = 0f;
    private int touchCount = 0;
    
    // États du jeu
    private enum GameState
    {
        Intro,
        Playing,
        GameWin,
        GameOverSuspicion,
        GameOverTimeout,
        GameOverTouch
    }
    
    private GameState currentState = GameState.Intro;
    
    // Chemins des fichiers flag
    // CHANGE CE CHEMIN selon ton Package Name dans Player Settings !
    private string basePath = "/storage/emulated/0/Android/data/com.UnityTechnologies.com.unitytemplate.urpblank/files/";
    private string keyPath;
    private string umbrellaPath;
    private string ballPath;
    private string codeUVPath;
    private string mazePath;
    private string codePath;
    private string captchaPath;
    
    // Progression des énigmes
    private bool keyCompleted = false;
    private bool umbrellaCompleted = false;
    private bool ballCompleted = false;
    private bool codeUVCompleted = false;
    private bool mazeCompleted = false;
    private bool codeCompleted = false;
    private bool captchaCompleted = false;

    void Start()
    {

        // Initialiser les chemins
        InitializePaths();
        
        // Nettoyer les anciens flags
        CleanupOldFlags();
        
        // Démarrer l'introduction
        StartCoroutine(IntroSequence());
    }

    void InitializePaths()
    {
        keyPath = Path.Combine(basePath, "key.flag");
        umbrellaPath = Path.Combine(basePath, "umbrella.flag");
        ballPath = Path.Combine(basePath, "ball.flag");
        codeUVPath = Path.Combine(basePath, "codeUV.flag");
        mazePath = Path.Combine(basePath, "maze.flag");
        codePath = Path.Combine(basePath, "code.flag");
        captchaPath = Path.Combine(basePath, "captcha.flag");
        
        // Créer le dossier s'il n'existe pas
        string directory = Path.GetDirectoryName(basePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    void CleanupOldFlags()
    {
        if (File.Exists(keyPath)) File.Delete(keyPath);
        if (File.Exists(umbrellaPath)) File.Delete(umbrellaPath);
        if (File.Exists(ballPath)) File.Delete(ballPath);
        if (File.Exists(codeUVPath)) File.Delete(codeUVPath);
        if (File.Exists(mazePath)) File.Delete(mazePath);
        if (File.Exists(codePath)) File.Delete(codePath);
        if (File.Exists(captchaPath)) File.Delete(captchaPath);
    }

    IEnumerator IntroSequence()
    {
        currentState = GameState.Intro;
        
        // Start01
        ShowMaxDialog("Hello. I am M.A.X. I am here to be sure you are qualified to enter.");
        yield return new WaitForSeconds(3f);
        
        // Start02
        ShowMaxDialog("Of course, you are suppose to know the steps to unlock me.\nPlease press the button to go to the next step. Every step is separated by a button like this one");
        yield return new WaitForSeconds(4f);
        
        // Start03
        ShowMaxDialog("Well let's see if you are really authorized to enter. You know you need an umbrella right ?\nYou might want to check in that umbrella holder if you forgot yours.");
        yield return new WaitForSeconds(4f);
        
        // StartTrial01
        ShowMaxDialog("It is basic knowledge to know which key is which to start your day right ?\nAnd a good day starts with an umbrella");
        yield return new WaitForSeconds(3f);
        
        // Commencer le jeu
        currentState = GameState.Playing;
        statusText.text = "En attente...";
        maxDialogText.text = "";
    }

    void Update()
    {
        // Ne rien faire si pas en mode Playing
        if (currentState != GameState.Playing)
            return;
        
        // Vérifier les flags à intervalle régulier
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            CheckFlags();
        }
        
        // Gestion du touchscreen
        HandleTouch();
        
        // Vérifier la victoire
        CheckWinCondition();
        
        // Vérifier Game Over par suspicion
        if (suspicious >= maxSuspicious)
        {
            GameOverSuspicion();
        }
        
        // Réinitialiser le texte après displayTime
        if (Time.time - lastActionTime > displayTime)
        {
            UpdateStatusDisplay();
        }
    }

    void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchCount++;
                statusText.text = $"✋ Touch {touchCount}/{maxTouchBeforeGameOver}";
                lastActionTime = Time.time;
                
                Debug.Log($"Touch détecté ! Total: {touchCount}");
                
                // Game Over si trop de touch
                if (touchCount >= maxTouchBeforeGameOver)
                {
                    GameOverTouch();
                }
            }
        }
    }

    void CheckFlags()
    {
        // Trial 01 - Key
        if (!keyCompleted && File.Exists(keyPath))
        {
            keyCompleted = true;
            OnKeyCompleted();
            File.Delete(keyPath);
        }
        
        // Trial 01 Finish - Umbrella
        if (!umbrellaCompleted && File.Exists(umbrellaPath))
        {
            umbrellaCompleted = true;
            OnUmbrellaCompleted();
            File.Delete(umbrellaPath);
        }
        
        // Trial 02 - Ball (magnetic)
        if (!ballCompleted && File.Exists(ballPath))
        {
            ballCompleted = true;
            OnBallCompleted();
            File.Delete(ballPath);
        }
        
        // Trial 03 - UV Code
        if (!codeUVCompleted && File.Exists(codeUVPath))
        {
            codeUVCompleted = true;
            OnCodeUVCompleted();
            File.Delete(codeUVPath);
        }
        
        // Trial 03 Finish - Maze
        if (!mazeCompleted && File.Exists(mazePath))
        {
            mazeCompleted = true;
            OnMazeCompleted();
            File.Delete(mazePath);
        }
        
        // Trial 04 - Code
        if (!codeCompleted && File.Exists(codePath))
        {
            codeCompleted = true;
            OnCodeCompleted();
            File.Delete(codePath);
        }
        
        // Trial 05 - Captcha (Final)
        if (!captchaCompleted && File.Exists(captchaPath))
        {
            captchaCompleted = true;
            OnCaptchaCompleted();
            File.Delete(captchaPath);
        }
    }

    // ============================================
    // ÉVÉNEMENTS DES ÉNIGMES
    // ============================================
    
    void OnKeyCompleted()
    {
        statusText.text = "🔑 Clé trouvée !";
        ShowMaxDialog("Good! You found the key. Now let's see about that umbrella...");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 01 - Key completed");
    }

    void OnUmbrellaCompleted()
    {
        statusText.text = "☂️ Parapluie obtenu !";
        ShowMaxDialog("Ah ! I knew you forgot your umbrella ! Well now you have one. And a magnetic one with that !\nTo be honest, I am a little clogged... Maybe you can help me with that thing inside this pipe ?");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 01 Finish - Umbrella completed");
    }

    void OnBallCompleted()
    {
        statusText.text = "🔮 Bille récupérée !";
        ShowMaxDialog("So you get this useless lamp. Crazy that with your eyes only you can't see a message that\nobvious on the door. That make me think that the employes flash it on the painting a lot");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 02 - Ball completed");
    }

    void OnCodeUVCompleted()
    {
        statusText.text = "💡 Lampe UV obtenue !";
        ShowMaxDialog("Now use it wisely on the painting...");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 03 - UV Code completed");
    }

    void OnMazeCompleted()
    {
        statusText.text = "🎨 Labyrinthe résolu !";
        ShowMaxDialog("Congrats ! You are not the slowest human but not by far ! For me, it is easy to see it, but you might need something more to see the true beauty of the best employes");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 03 Finish - Maze completed");
    }

    void OnCodeCompleted()
    {
        statusText.text = "👔 Employés identifiés !";
        ShowMaxDialog("Keep it up ! Now i'm sure that you know what's behind that hole. But before that, Security question !");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 04 - Code completed");
    }

    void OnCaptchaCompleted()
    {
        statusText.text = "✅ Forme trouvée !";
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 05 - Captcha completed");
        
        // Vérifier la victoire immédiatement
        CheckWinCondition();
    }

    // ============================================
    // CONDITIONS DE FIN
    // ============================================
    
    void CheckWinCondition()
    {
        if (keyCompleted && umbrellaCompleted && ballCompleted && 
            codeUVCompleted && mazeCompleted && codeCompleted && captchaCompleted)
        {
            GameWin();
        }
    }

    void GameWin()
    {
        if (currentState == GameState.GameWin) return;
        
        currentState = GameState.GameWin;
        statusText.text = "🎉 VICTOIRE !";
        ShowMaxDialog("Well done ! You succeded all the verification steps ! Enjoy your day at work !\nSuper ! I feel like you're ready to climb the carreer lader ! Keep going !");
        
        Debug.Log("🎉 GAME WIN !");
    }

    void GameOverSuspicion()
    {
        if (currentState != GameState.Playing) return;
        
        currentState = GameState.GameOverSuspicion;
        statusText.text = "💀 GAME OVER - Suspicion";
        ShowMaxDialog("An intruder has been detected in front of our grand company D.O.O.R.H. Please do not panic,\nour teams will take care of it. Stay close to you station post and keep serve our society.");
        
        Debug.Log("💀 GAME OVER - Suspicion");
    }

    void GameOverTouch()
    {
        if (currentState != GameState.Playing) return;
        
        currentState = GameState.GameOverTouch;
        statusText.text = "💀 GAME OVER - Trop de touches !";
        ShowMaxDialog("Stop touching everything! Security has been alerted!");
        
        Debug.Log($"💀 GAME OVER - Touch limit ({touchCount} touches)");
    }

    // ============================================
    // SYSTÈME DE SUSPICION
    // ============================================
    
    public void AddSuspicion(float amount = 1f)
    {
        if (currentState != GameState.Playing) return;
        
        suspicious += amount;
        statusText.text = $"⚠️ Suspicion : {suspicious:F1}/{maxSuspicious}";
        lastActionTime = Time.time;
        
        Debug.Log($"Suspicion ajoutée: {amount} (Total: {suspicious}/{maxSuspicious})");
    }

    // ============================================
    // AFFICHAGE
    // ============================================
    
    void ShowMaxDialog(string text)
    {
        if (maxDialogText != null)
        {
            maxDialogText.text = "MAX: " + text;
        }
    }

    void UpdateStatusDisplay()
    {
        if (currentState == GameState.Playing)
        {
            int completed = 0;
            if (keyCompleted) completed++;
            if (umbrellaCompleted) completed++;
            if (ballCompleted) completed++;
            if (codeUVCompleted) completed++;
            if (mazeCompleted) completed++;
            if (codeCompleted) completed++;
            if (captchaCompleted) completed++;
            
            statusText.text = $"Énigmes: {completed}/7 | Touch: {touchCount}/{maxTouchBeforeGameOver}";
        }
    }

    // ============================================
    // DEBUG
    // ============================================
    
    void OnGUI()
    {
        // Afficher les infos de debug
        GUI.Label(new Rect(10, 10, 400, 200), 
            $"<size=16><color=white>" +
            $"État: {currentState}\n" +
            $"Key: {keyCompleted}\n" +
            $"Umbrella: {umbrellaCompleted}\n" +
            $"Ball: {ballCompleted}\n" +
            $"UV Code: {codeUVCompleted}\n" +
            $"Maze: {mazeCompleted}\n" +
            $"Code: {codeCompleted}\n" +
            $"Captcha: {captchaCompleted}\n" +
            $"Touch: {touchCount}/{maxTouchBeforeGameOver}\n" +
            $"Suspicion: {suspicious:F1}/{maxSuspicious}" +
            $"</color></size>");
    }
}