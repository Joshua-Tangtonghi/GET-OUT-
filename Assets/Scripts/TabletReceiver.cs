using UnityEngine;
using TMPro;
using System.IO;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.UI;
using _project.Scripts.Managers;

public class TabletReceiver : MonoBehaviour
{
    [Header("UI")] 
    public TMP_Text maxDialogText;
    public UiPannelButtons questionPanel;

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

    private string basePath =
        "/storage/emulated/0/Android/data/com.UnityTechnologies.com.unity.template.urpblank/files/";

    private string keyPath;
    private string umbrellaPath;
    private string ballPath;
    private string codeUVPath;
    private string mazePath;
    private string codePath;
    private string captchaPath;

    private bool keyCompleted = false;
    private bool umbrellaCompleted = false;
    private bool ballCompleted = false;
    private bool codeUVCompleted = false;
    private bool mazeCompleted = false;
    private bool codeCompleted = false;
    private bool captchaCompleted = false;

    private bool question1Asked = false;
    private bool question2Asked = false;
    private bool question3Asked = false;
    private bool question4Asked = false;
    private bool question5Asked = false;

    private int currentCorrectAnswer = -1;
    private int lastAnswer = -1;
    bool flag = true;
    

    void Start()
    {

        InitializePaths();
        CleanupOldFlags(); 

        if (questionPanel != null)
        {
            questionPanel.gameObject.SetActive(false);
            Debug.Log("✅ Question panel initialisé et caché");
        }
        else
        {
            Debug.LogError("❌ questionPanel est NULL dans Start!");
        }
        
        StartCoroutine(IntroSequence());
    }
    private void OnClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("UI_MainScene");
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
        AudioManager.Instance.Play("start01");
        yield return new WaitForSeconds(AudioManager.Instance.GetLength("start01"));

        ShowMaxDialog(
            "Of course, you are suppose to know the steps to unlock me.\nPlease press the button to go to the next step. Every step is separated by a button like this one");
        AudioManager.Instance.Play("start02");
        yield return new WaitForSeconds(AudioManager.Instance.GetLength("start02"));

        ShowMaxDialog(
            "Well let's see if you are really authorized to enter. You know you need an umbrella right ?\nYou might want to check in that umbrella holder if you forgot yours.");
        AudioManager.Instance.Play("start03");
        yield return new WaitForSeconds(AudioManager.Instance.GetLength("start03"));

        ShowMaxDialog(
            "It is basic knowledge to know which key is which to start your day right ?\nAnd a good day starts with an umbrella");
        AudioManager.Instance.Play("startTrial01");
        yield return new WaitForSeconds(AudioManager.Instance.GetLength("startTrial01"));

        currentState = GameState.Playing;
        maxDialogText.text = "";
    }

    IEnumerator LoadMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene("UI_MainMenu");
    }

    void Update()
    {
        if (currentState == GameState.WaitingForAnswer)
        {
            CheckPlayerAnswer();
            return;
        }

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
        if (!keyCompleted && File.Exists(keyPath))
        {
            keyCompleted = true;
            OnKeyCompleted();
            File.Delete(keyPath);
            flag = false;
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

    void OnKeyCompleted()
    {
        ShowMaxDialog("Good! You found the key. Now let's see about that umbrella...");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 01 - Key completed");
        
        StartCoroutine(AskQuestionAfterDelay(1, 2f));
    }

    void OnUmbrellaCompleted()
    {
        ShowMaxDialog(
            "Ah ! I knew you forgot your umbrella ! Well now you have one. And a magnetic one with that !\nTo be honest, I am a little clogged... Maybe you can help me with that thing inside this pipe ?");
        AudioManager.Instance.Play("finishTrial01");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 01 Finish - Umbrella completed");
        
        StartCoroutine(AskQuestionAfterDelay(2, 2f));
    }

    void OnBallCompleted()
    {
        ShowMaxDialog(
            "So you get this useless lamp. Crazy that with your eyes only you can't see a message that\nobvious on the door. That make me think that the employes flash it on the painting a lot");
        AudioManager.Instance.Play("finishTrial02");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 02 - Ball completed");
        
        StartCoroutine(AskQuestionAfterDelay(3, 2f));
    }

    void OnCodeUVCompleted()
    {
        ShowMaxDialog("Now use it wisely on the painting...");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 03 - UV Code completed");
    }

    void OnMazeCompleted()
    {
        ShowMaxDialog(
            "Congrats ! You are not the slowest human but not by far ! For me, it is easy to see it, but you might need something more to see the true beauty of the best employes");
        AudioManager.Instance.Play("finishTrial03");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 03 Finish - Maze completed");
        
        StartCoroutine(AskQuestionAfterDelay(4, 2f));
    }

    void OnCodeCompleted()
    {
        ShowMaxDialog(
            "Keep it up ! Now i'm sure that you know what's behind that hole. But before that, Security question !");
        lastActionTime = Time.time;
        AudioManager.Instance.Play("finishTrial04");
        Debug.Log("✅ Trial 04 - Code completed");
        
        StartCoroutine(AskQuestionAfterDelay(5, 2f));
    }

    void OnCaptchaCompleted()
    {
        AudioManager.Instance.Play("finishTrial05");
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 05 - Captcha completed");
        CheckWinCondition();
    }

    IEnumerator AskQuestionAfterDelay(int questionNumber, float delay)
    {
        yield return new WaitForSeconds(delay);
        AskQuestion(questionNumber);
    }

    void AskQuestion(int questionNumber)
    {
        switch (questionNumber)
        {
            case 1:
                if (question1Asked) return;
                question1Asked = true;
                AudioManager.Instance.Play("question01");
                AskQuestion1();
                break;
            case 2:
                if (question2Asked) return;
                AudioManager.Instance.Play("question02");
                question2Asked = true;
                AskQuestion2();
                break;
            case 3:
                if (question3Asked) return;
                AudioManager.Instance.Play("question03");
                question3Asked = true;
                AskQuestion3();
                break;
            case 4:
                if (question4Asked) return;
                AudioManager.Instance.Play("question04");
                question4Asked = true;
                AskQuestion4();
                break;
            case 5:
                if (question5Asked) return;
                AudioManager.Instance.Play("question05");
                question5Asked = true;
                AskQuestion5();
                break;
        }
    }

    void AskQuestion1()
    {
        Debug.Log("=== AskQuestion1 START ===");
        
        currentState = GameState.WaitingForAnswer;
        currentCorrectAnswer = 2; // Index de "20"
        lastAnswer = -1;
        
        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(false);
            Debug.Log("✅ Panel text caché");
        }
        
        if (questionPanel != null)
        {
            // IMPORTANT : Activer le panel AVANT de set les textes
            questionPanel.gameObject.SetActive(true);
            Debug.Log("✅ Question panel activé");
            
            // Reset la réponse
            questionPanel.ResetAnswer();
            
            // Définir la question
            questionPanel.SetQuestion("All right! Let's see your knowledge about D.O.O.R.H.! How many keys are there on me?");
            Debug.Log("✅ Question définie");
            
            // Définir les textes des boutons
            string[] answers = { "10", "15", "20", "25" };
            questionPanel.SetButtonsText(answers);
            Debug.Log("✅ Textes des boutons définis");
            
            // Rendre visible (animation si nécessaire)
            questionPanel.ButtonPanelVisibility(true);
            Debug.Log("✅ Panel visible (animation lancée)");
        }
        else
        {
            Debug.LogError("❌ questionPanel est NULL!");
        }
        
        Debug.Log("=== AskQuestion1 END ===");
    }

    void AskQuestion2()
    {
        Debug.Log("=== AskQuestion2 START ===");
        
        currentState = GameState.WaitingForAnswer;
        currentCorrectAnswer = 0; // Index de "Yes"
        lastAnswer = -1;
        
        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(false);
        }
        
        if (questionPanel != null)
        {
            questionPanel.gameObject.SetActive(true);
            questionPanel.ResetAnswer();
            questionPanel.SetQuestion("Daily survey of evolution within our large company D.O.O.R.H, please give us your thoughts! Is the marble on top of me magnetic?");
            
            string[] answers = { "Yes", "No", "Maybe", "I don't know" };
            questionPanel.SetButtonsText(answers);
            
            questionPanel.ButtonPanelVisibility(true);
            Debug.Log("✅ Question 2 affichée");
        }
    }

    void AskQuestion3()
    {
        Debug.Log("=== AskQuestion3 START ===");
        
        currentState = GameState.WaitingForAnswer;
        currentCorrectAnswer = 1; // Index de "An employee"
        lastAnswer = -1;
        
        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(false);
        }
        
        if (questionPanel != null)
        {
            questionPanel.gameObject.SetActive(true);
            questionPanel.ResetAnswer();
            
            string questionPrefix = suspicious > 5f 
                ? "Well well well, I think you're hiding things from us, answer to this! Just a little basic security investigation! *polite laugh* Nothing dangerous!\n\n"
                : "Well, I would need to collect some information, just a quick satisfaction survey!\n\n";
            
            questionPanel.SetQuestion(questionPrefix + "Who are you?");
            
            string[] answers = { "A visitor", "An employee", "A thief", "MAX" };
            questionPanel.SetButtonsText(answers);
            
            questionPanel.ButtonPanelVisibility(true);
            Debug.Log("✅ Question 3 affichée");
        }
    }

    void AskQuestion4()
    {
        Debug.Log("=== AskQuestion4 START ===");
        
        currentState = GameState.WaitingForAnswer;
        currentCorrectAnswer = 1; // Index de "B"
        lastAnswer = -1;
        
        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(false);
        }
        
        if (questionPanel != null)
        {
            questionPanel.gameObject.SetActive(true);
            questionPanel.ResetAnswer();
            questionPanel.SetQuestion("On the employees of the month board, who has the most chances of being promoted?");
            
            string[] answers = { "A", "B", "C", "D" };
            questionPanel.SetButtonsText(answers);
            
            questionPanel.ButtonPanelVisibility(true);
            Debug.Log("✅ Question 4 affichée");
        }
    }

    void AskQuestion5()
    {
        Debug.Log("=== AskQuestion5 START ===");
        
        currentState = GameState.WaitingForAnswer;
        currentCorrectAnswer = 3; // Index de "Please don't"
        lastAnswer = -1;
        
        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(false);
        }
        
        if (questionPanel != null)
        {
            questionPanel.gameObject.SetActive(true);
            questionPanel.ResetAnswer();
            questionPanel.SetQuestion("In the hole, if you put your hand in it, do you think you risk being electrocuted?");
            
            string[] answers = { "Yes", "No", "Maybe", "Please don't" };
            questionPanel.SetButtonsText(answers);
            
            questionPanel.ButtonPanelVisibility(true);
            Debug.Log("✅ Question 5 affichée");
        }
    }

    void CheckPlayerAnswer()
    {
        if (questionPanel == null) return;

        int playerAnswer = questionPanel.GetPlayerAnswer();
        
        if (playerAnswer != lastAnswer && playerAnswer >= 0)
        {
            lastAnswer = playerAnswer;
            Debug.Log($"🔘 Player answered: {playerAnswer}, Correct: {currentCorrectAnswer}");
            
            OnAnswerSelected(playerAnswer == currentCorrectAnswer);
        }
    }

    void OnAnswerSelected(bool isCorrect)
    {
        Debug.Log($"📝 OnAnswerSelected called! IsCorrect: {isCorrect}");
        
        if (questionPanel != null)
        {
            questionPanel.ButtonPanelVisibility(false);
            questionPanel.gameObject.SetActive(false);
            Debug.Log("❌ Question panel hidden");
        }
        
        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(true);
            Debug.Log("✅ Panel text shown");
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
        
        currentState = GameState.Playing;
        lastActionTime = Time.time;
        currentCorrectAnswer = -1;
        lastAnswer = -1;
        
        Debug.Log("🎮 Answer processed, returning to Playing state");
    }

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
        ShowMaxDialog(
            "Well done! You succeeded all the verification steps! Enjoy your day at work!\nSuper! I feel like you're ready to climb the career ladder! Keep going!");
        AudioManager.Instance.Play("end");
        Debug.Log("🎉 GAME WIN !");
        StartCoroutine(LoadMainMenuAfterDelay(5f));
    }

    void GameOverSuspicion()
    {
        if (currentState != GameState.Playing && currentState != GameState.WaitingForAnswer) return;

        currentState = GameState.GameOverSuspicion;
        
        if (questionPanel != null)
        {
            questionPanel.ButtonPanelVisibility(false);
            questionPanel.gameObject.SetActive(false);
        }
            
        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
            UIManager.Instance.UiPanelText.PanelTextVisibility(true);
        
        ShowMaxDialog(
            "An intruder has been detected in front of our grand company D.O.O.R.H. Please do not panic,\nour teams will take care of it. Stay close to your station post and keep serving our society.");
        AudioManager.Instance.Play("gameOverSuspicion");
        Debug.Log("💀 GAME OVER - Suspicion");
        StartCoroutine(LoadMainMenuAfterDelay(5f));
    }

    void GameOverTouch()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.GameOverTouch;
        ShowMaxDialog("Stop touching everything! Security has been alerted!");

        Debug.Log($"💀 GAME OVER - Touch limit ({touchCount} touches)");
        StartCoroutine(LoadMainMenuAfterDelay(5f));
        
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.EndingEye(false);
    }

    public void AddSuspicion(float amount = 1f)
    {
        if (currentState != GameState.Playing && currentState != GameState.WaitingForAnswer) return;

        suspicious += amount;
        lastActionTime = Time.time;

        Debug.Log($"Suspicion ajoutée: {amount} (Total: {suspicious}/{maxSuspicious})");
    }

    void ShowMaxDialog(string text)
    {
        if (maxDialogText && UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            string textF = UIManager.Instance.UiPanelText.SetPanelText(text,0.05f);
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
        }
    }

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