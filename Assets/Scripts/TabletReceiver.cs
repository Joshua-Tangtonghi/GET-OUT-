using UnityEngine;
using TMPro;
using System.IO;
using System.Collections;

public class TabletReceiver : MonoBehaviour
{
    [Header("UI")] 
    public TMP_Text statusText;
    public TMP_Text maxDialogText;
    public GameObject questionPanel; // Panel contenant les 4 boutons
    public TMP_Text questionText;
    public UnityEngine.UI.Button[] answerButtons; // 4 boutons de réponse

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
        WaitingForAnswer,
        GameWin,
        GameOverSuspicion,
        GameOverTimeout,
        GameOverTouch
    }

    private GameState currentState = GameState.Intro;

    // Chemins des fichiers flag
    private string basePath =
        "/storage/emulated/0/Android/data/com.UnityTechnologies.com.unity.template.urpblank/files/";

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

    // Questions posées
    private bool question1Asked = false;
    private bool question2Asked = false;
    private bool question3Asked = false;
    private bool question4Asked = false;
    private bool question5Asked = false;

    void Start()
    {
        InitializePaths();
        CleanupOldFlags();
        
        // Masquer le panel de questions au départ
        if (questionPanel != null)
            questionPanel.SetActive(false);
        
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

        ShowMaxDialog("Hello. I am M.A.X. I am here to be sure you are qualified to enter.");
        yield return new WaitForSeconds(2f);

        ShowMaxDialog(
            "Of course, you are suppose to know the steps to unlock me.\nPlease press the button to go to the next step. Every step is separated by a button like this one");
        yield return new WaitForSeconds(2f);

        ShowMaxDialog(
            "Well let's see if you are really authorized to enter. You know you need an umbrella right ?\nYou might want to check in that umbrella holder if you forgot yours.");
        yield return new WaitForSeconds(3f);

        ShowMaxDialog(
            "It is basic knowledge to know which key is which to start your day right ?\nAnd a good day starts with an umbrella");
        yield return new WaitForSeconds(3f);

        currentState = GameState.Playing;
        statusText.text = "En attente...";
        maxDialogText.text = "";
    }

    IEnumerator LoadMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene("UI_MainMenu");
    }

    void Update()
    {
        if (currentState != GameState.Playing)
            return;

        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            CheckFlags();
        }

        HandleTouch();
        CheckWinCondition();

        if (suspicious >= maxSuspicious)
        {
            GameOverSuspicion();
        }

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

                if (touchCount >= maxTouchBeforeGameOver)
                {
                    GameOverTouch();
                }
            }
        }
    }

    void CheckFlags()
    {
        if (true)
        {
            keyCompleted = true;
            OnKeyCompleted();
            File.Delete(keyPath);
        }

        if (!umbrellaCompleted && File.Exists(umbrellaPath))
        {
            umbrellaCompleted = true;
            OnUmbrellaCompleted();
            File.Delete(umbrellaPath);
        }

        if (!ballCompleted && File.Exists(ballPath))
        {
            ballCompleted = true;
            OnBallCompleted();
            File.Delete(ballPath);
        }

        if (!codeUVCompleted && File.Exists(codeUVPath))
        {
            codeUVCompleted = true;
            OnCodeUVCompleted();
            File.Delete(codeUVPath);
        }

        if (!mazeCompleted && File.Exists(mazePath))
        {
            mazeCompleted = true;
            OnMazeCompleted();
            File.Delete(mazePath);
        }

        if (!codeCompleted && File.Exists(codePath))
        {
            codeCompleted = true;
            OnCodeCompleted();
            File.Delete(codePath);
        }

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
        
        // Poser la question 1 après un délai
        StartCoroutine(AskQuestionAfterDelay(1, 2f));
    }

    void OnUmbrellaCompleted()
    {
        statusText.text = "☂️ Parapluie obtenu !";
        ShowMaxDialog(
            "Ah ! I knew you forgot your umbrella ! Well now you have one. And a magnetic one with that !\nTo be honest, I am a little clogged... Maybe you can help me with that thing inside this pipe ?");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 01 Finish - Umbrella completed");
        
        StartCoroutine(AskQuestionAfterDelay(2, 2f));
    }

    void OnBallCompleted()
    {
        statusText.text = "🔮 Bille récupérée !";
        ShowMaxDialog(
            "So you get this useless lamp. Crazy that with your eyes only you can't see a message that\nobvious on the door. That make me think that the employes flash it on the painting a lot");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 02 - Ball completed");
        
        StartCoroutine(AskQuestionAfterDelay(3, 2f));
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
        ShowMaxDialog(
            "Congrats ! You are not the slowest human but not by far ! For me, it is easy to see it, but you might need something more to see the true beauty of the best employes");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 03 Finish - Maze completed");
        
        StartCoroutine(AskQuestionAfterDelay(4, 2f));
    }

    void OnCodeCompleted()
    {
        statusText.text = "👔 Employés identifiés !";
        ShowMaxDialog(
            "Keep it up ! Now i'm sure that you know what's behind that hole. But before that, Security question !");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 04 - Code completed");
        
        StartCoroutine(AskQuestionAfterDelay(5, 2f));
    }

    void OnCaptchaCompleted()
    {
        statusText.text = "✅ Forme trouvée !";
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 05 - Captcha completed");
        CheckWinCondition();
    }

    // ============================================
    // SYSTÈME DE QUESTIONS
    // ============================================

    IEnumerator AskQuestionAfterDelay(int questionNumber, float delay)
    {
        yield return new WaitForSeconds(delay);
        AskQuestion(questionNumber);
    }

    void AskQuestion(int questionNumber)
    {
        // Vérifier si la question a déjà été posée
        switch (questionNumber)
        {
            case 1:
                if (question1Asked) return;
                question1Asked = true;
                AskQuestion1();
                break;
            case 2:
                if (question2Asked) return;
                question2Asked = true;
                AskQuestion2();
                break;
            case 3:
                if (question3Asked) return;
                question3Asked = true;
                AskQuestion3();
                break;
            case 4:
                if (question4Asked) return;
                question4Asked = true;
                AskQuestion4();
                break;
            case 5:
                if (question5Asked) return;
                question5Asked = true;
                AskQuestion5();
                break;
        }
    }

    void AskQuestion1()
    {
        currentState = GameState.WaitingForAnswer;
        
        Debug.Log("=== AskQuestion1 called ===");
        
        // Masquer le panel de texte MAX
        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(false);
            Debug.Log("Panel text hidden");
        }
        
        // Afficher le panel de questions
        if (questionPanel != null)
        {
            questionPanel.SetActive(true);
            Debug.Log("Question panel shown");
        }
        
        if (questionText != null)
        {
            questionText.text = "All right! Let's see your knowledge about D.O.O.R.H.! How many keys are there on me?";
            Debug.Log("Question text set");
        }
        
        SetupAnswerButton(0, "5", false);
        SetupAnswerButton(1, "10", false);
        SetupAnswerButton(2, "20", true);
        SetupAnswerButton(3, "50", false);
        
        Debug.Log("All buttons setup for Question 1");
    }

    void AskQuestion2()
    {
        currentState = GameState.WaitingForAnswer;
        UIManager.Instance.UiPanelText.PanelTextVisibility(false);
        
        questionPanel.SetActive(true);
        questionText.text = "Daily survey of evolution within our large company D.O.O.R.H, please give us your thoughts! Is the marble on top of me magnetic?";
        
        SetupAnswerButton(0, "Yes", true);
        SetupAnswerButton(1, "No", false);
        SetupAnswerButton(2, "", false); // Bouton désactivé
        SetupAnswerButton(3, "", false); // Bouton désactivé
        
        answerButtons[2].gameObject.SetActive(false);
        answerButtons[3].gameObject.SetActive(false);
    }

    void AskQuestion3()
    {
        currentState = GameState.WaitingForAnswer;
        UIManager.Instance.UiPanelText.PanelTextVisibility(false);
        
        questionPanel.SetActive(true);
        
        string questionPrefix = suspicious > 5f 
            ? "Well well well, I think you're hiding things from us, answer to this! Just a little basic security investigation! *polite laugh* Nothing dangerous!\n\n"
            : "Well, I would need to collect some information, just a quick satisfaction survey!\n\n";
        
        questionText.text = questionPrefix + "Who are you?";
        
        SetupAnswerButton(0, "The boss", false);
        SetupAnswerButton(1, "An employee", true);
        SetupAnswerButton(2, "A security agent", false);
        SetupAnswerButton(3, "An intern", false);
    }

    void AskQuestion4()
    {
        currentState = GameState.WaitingForAnswer;
        UIManager.Instance.UiPanelText.PanelTextVisibility(false);
        
        questionPanel.SetActive(true);
        questionText.text = "On the employees of the month board, who has the most chances of being promoted?";
        
        SetupAnswerButton(0, "A", false);
        SetupAnswerButton(1, "B", true);
        SetupAnswerButton(2, "C", false);
        SetupAnswerButton(3, "D", false);
    }

    void AskQuestion5()
    {
        currentState = GameState.WaitingForAnswer;
        UIManager.Instance.UiPanelText.PanelTextVisibility(false);
        
        questionPanel.SetActive(true);
        questionText.text = "In the hole, if you put your hand in it, do you think you risk being electrocuted?";
        
        SetupAnswerButton(0, "Yes", false);
        SetupAnswerButton(1, "No", false);
        SetupAnswerButton(2, "Maybe", false);
        SetupAnswerButton(3, "Please don't", true);
    }

    void SetupAnswerButton(int index, string text, bool isCorrect)
    {
        if (index >= answerButtons.Length) return;
        
        answerButtons[index].gameObject.SetActive(true);
        
        TMP_Text buttonText = answerButtons[index].GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = text;
        }
        
        // Retirer tous les listeners précédents
        answerButtons[index].onClick.RemoveAllListeners();
        
        // Ajouter le nouveau listener
        int buttonIndex = index;
        answerButtons[index].onClick.AddListener(() => {
            Debug.Log($"Button {buttonIndex} clicked! Text: {text}, IsCorrect: {isCorrect}");
            OnAnswerSelected(isCorrect);
        });
        
        // Vérifier que le bouton est interactable
        answerButtons[index].interactable = true;
        
        Debug.Log($"Button {index} setup: '{text}' - Correct: {isCorrect}");
    }

    void OnAnswerSelected(bool isCorrect)
    {
        Debug.Log($"OnAnswerSelected called! IsCorrect: {isCorrect}");
        
        // Masquer le panel de questions
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
            Debug.Log("Question panel hidden");
        }
        
        // Réafficher le panel de texte MAX
        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(true);
            Debug.Log("Panel text shown");
        }
        
        if (isCorrect)
        {
            ShowMaxDialog("Very good! Let's continue!");
            Debug.Log("✅ Bonne réponse !");
        }
        else
        {
            AddSuspicion(2f);
            ShowMaxDialog("Hmm... That's not quite right. Suspicious...");
            Debug.Log("❌ Mauvaise réponse - Suspicion ajoutée");
        }
        
        // Retourner au mode Playing
        currentState = GameState.Playing;
        lastActionTime = Time.time;
        
        Debug.Log("Answer processed, returning to Playing state");
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
        ShowMaxDialog(
            "Well done! You succeeded all the verification steps! Enjoy your day at work!\nSuper! I feel like you're ready to climb the career ladder! Keep going!");

        Debug.Log("🎉 GAME WIN !");
        StartCoroutine(LoadMainMenuAfterDelay(5f));
    }

    void GameOverSuspicion()
    {
        if (currentState != GameState.Playing && currentState != GameState.WaitingForAnswer) return;

        currentState = GameState.GameOverSuspicion;
        questionPanel.SetActive(false);
        UIManager.Instance.UiPanelText.PanelTextVisibility(true);
        
        statusText.text = "💀 GAME OVER - Suspicion";
        ShowMaxDialog(
            "An intruder has been detected in front of our grand company D.O.O.R.H. Please do not panic,\nour teams will take care of it. Stay close to your station post and keep serving our society.");

        Debug.Log("💀 GAME OVER - Suspicion");
        StartCoroutine(LoadMainMenuAfterDelay(5f));
    }

    void GameOverTouch()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.GameOverTouch;
        statusText.text = "💀 GAME OVER - Trop de touches !";
        ShowMaxDialog("Stop touching everything! Security has been alerted!");

        Debug.Log($"💀 GAME OVER - Touch limit ({touchCount} touches)");
        StartCoroutine(LoadMainMenuAfterDelay(5f));
        UIManager.Instance.UiEye.EndingEye(false);
    }

    // ============================================
    // SYSTÈME DE SUSPICION
    // ============================================

    public void AddSuspicion(float amount = 1f)
    {
        if (currentState != GameState.Playing && currentState != GameState.WaitingForAnswer) return;

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
        if (maxDialogText)
        {
            string textF = UIManager.Instance.UiPanelText.SetPanelText(text, 0.1f);
            maxDialogText.text = "MAX: " + textF;
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
        GUI.Label(new Rect(10, 10, 400, 250),
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
            $"Suspicion: {suspicious:F1}/{maxSuspicious}\n" +
            $"Q1: {question1Asked} | Q2: {question2Asked} | Q3: {question3Asked}\n" +
            $"Q4: {question4Asked} | Q5: {question5Asked}" +
            $"</color></size>");
    }
}