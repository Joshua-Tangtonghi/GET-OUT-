
using UnityEngine;
using TMPro;
using System.IO;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.UI;
using _project.Scripts.Managers;

public class TabletReceiver : MonoBehaviour
{
    [Header("UI")] public TMP_Text maxDialogText;
    public UiPannelButtons questionPanel;

    [Header("Configuration")] public float checkInterval = 0.5f;
    public float displayTime = 2f;
    public int maxTouchBeforeGameOver = 5;
    public float tipsTime = 30f;

    [Header("Suspicion")] public float maxSuspicious = 10f;

    private float nextCheckTime = 0f;
    private float lastActionTime = 0f;
    private float suspicious = 0f;
    private int touchCount = 0;

    private enum GameState
    {
        Intro,
        Playing,
        WaitingForAnswer,
        ShowingTip,
        GameWin,
        GameOverSuspicion,
        GameOverTimeout,
        GameOverTouch
    }

    private GameState currentState = GameState.Intro;

    private string basePath =
        "/storage/emulated/0/Android/data/com.UnityTechnologies.com.unity.template.urpblank/files/";

    private string umbrellaPath;
    private string ballPath;
    private string mazePath;
    private string codePath;

    private bool umbrellaCompleted = false;
    private bool ballCompleted = false;
    private bool mazeCompleted = false;
    private bool codeCompleted = false;

    private bool question1Asked = false;
    private bool question2Asked = false;
    private bool question3Asked = false;
    private bool question4Asked = false;
    private bool question5Asked = false;
    private bool question6Asked = false;

    private int currentCorrectAnswer = -1;
    private int lastAnswer = -1;

    // Audio/Speak lock
    private bool isSpeaking = false;

    // Pour gérer les tips planifiés
    private Coroutine currentTipCoroutine = null;

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

    void InitializePaths()
    {
        umbrellaPath = Path.Combine(basePath, "umbrella.flag");
        ballPath = Path.Combine(basePath, "ball.flag");
        mazePath = Path.Combine(basePath, "maze.flag");
        codePath = Path.Combine(basePath, "code.flag");

        string directory = Path.GetDirectoryName(basePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    void CleanupOldFlags()
    {
        if (File.Exists(umbrellaPath)) File.Delete(umbrellaPath);
        if (File.Exists(ballPath)) File.Delete(ballPath);
        if (File.Exists(mazePath)) File.Delete(mazePath);
        if (File.Exists(codePath)) File.Delete(codePath);
    }

    // Speak helper - évite les chevauchements audio
    IEnumerator Speak(string text, string audioClipName)
    {
        while (isSpeaking) yield return null;

        isSpeaking = true;

        if (!string.IsNullOrEmpty(text))
        {
            // S'assurer que le panel est visible avant d'afficher le texte
            if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
            {
                UIManager.Instance.UiPanelText.PanelTextVisibility(true);
            }

            ShowMaxDialog(text);
        }

        if (AudioManager.Instance != null && !string.IsNullOrEmpty(audioClipName))
        {
            AudioManager.Instance.Play(audioClipName);
            yield return StartCoroutine(AudioManager.Instance.WaitForSoundEnd(audioClipName));
        }
        else if (!string.IsNullOrEmpty(text))
        {
            float fallback = Mathf.Clamp(1f + text.Length * 0.03f, 1f, 6f);
            yield return new WaitForSeconds(fallback);
        }

        if (maxDialogText != null && !string.IsNullOrEmpty(text))
            maxDialogText.text = "";

        isSpeaking = false;
    }

    IEnumerator IntroSequence()
    {
        currentState = GameState.Intro;

        yield return StartCoroutine(Speak("Hello. I am M.A.X. I am here to be sure you are qualified to enter.",
            "start01"));

        yield return StartCoroutine(Speak(
            "Of course, you are suppose to know the steps to unlock me.\nPlease press the button to go to the next step. Every step is separated by a button like this one",
            "start02"));

        yield return StartCoroutine(Speak(
            "Well let's see if you are really authorized to enter. You know you need an umbrella right ? You might want to check in that umbrella holder if you forgot yours.",
            "start03"));

        yield return StartCoroutine(Speak(
            "It is basic knowledge to know which key is which to start your day right ?\nAnd a good day starts with an umbrella",
            "startTrials01"));

        maxDialogText.text = "";
        currentState = GameState.Playing;

        // Démarrer le timer APRÈS l'intro
        if (UIManager.Instance != null)
        {
            UIManager.Instance.StartTimer();
            if (UIManager.Instance.UiTimer != null)
            {
                UIManager.Instance.UiTimer.TimerVisibility(true);
            }
        }

        Debug.Log("⏱️ Timer démarré via UIManager APRÈS l'intro");

        // Démarrer le premier tip après tipsTime secondes
        currentTipCoroutine = StartCoroutine(ShowTipAfterDelay(1, tipsTime));
    }

    IEnumerator ShowTipAfterDelay(int tipNumber, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Vérifier qu'on est bien en mode Playing (pas en train de répondre à une question)
        if (currentState == GameState.Playing)
        {
            yield return StartCoroutine(ShowTip(tipNumber));
        }
        else
        {
            Debug.Log($"⏸️ Tip {tipNumber} annulé car état = {currentState}");
        }
    }

    IEnumerator ShowTip(int tipNumber)
    {
        currentState = GameState.ShowingTip;

        string tipMessage = "";
        string audioClip = "";

        switch (tipNumber)
        {
            case 1:
                tipMessage =
                    "You...You're alright ? Hard to remember something so obvious ? Well I can understand... Or not. How do you want to open a lock without a key?";
                audioClip = "tipsTrials01";
                break;
            case 2:
                tipMessage = "You know that an umbrella can get you to place much higher?";
                audioClip = "tipsTrials02";
                break;
            case 3:
                tipMessage = "Maybe you can try to use what you just got on the painting ?";
                audioClip = "tipsTrials03";
                break;
            case 4:
                tipMessage = "Roses are red, Violets are blue, the colors are shifted, glasses you should use.";
                audioClip = "tipsTrials04";
                break;
            case 5:
                tipMessage = "Come on ! What's in the box ? You should know.... It has not changed since yesterday.";
                audioClip = "tipsTrials05";
                break;
        }

        yield return StartCoroutine(Speak(tipMessage, audioClip));
        currentState = GameState.Playing;
    }

    IEnumerator LoadMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene("UI_MainEnd");
    }

    void Update()
    {
        // Vérifier le timeout via UIManager
        if (UIManager.Instance != null &&
            (currentState == GameState.Playing || currentState == GameState.WaitingForAnswer))
        {
            if (UIManager.Instance.currentTimer <= 0f)
            {
                GameOverTimeout();
                return;
            }
        }

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
    }

    void CancelScheduledTip()
    {
        if (currentTipCoroutine != null)
        {
            StopCoroutine(currentTipCoroutine);
            currentTipCoroutine = null;
            Debug.Log("🚫 Tip planifié annulé");
        }
    }

    void OnUmbrellaCompleted()
    {
        lastActionTime = Time.time;
        CancelScheduledTip(); // Annuler le tip en attente
        Debug.Log("✅ Trial 01 - Umbrella completed");

        StartCoroutine(SpeakThenQuestionTip(
            "Ah ! I knew you forgot your umbrella ! Well now you have one. And a magnetic one with that ! To be honest, I am a little clogged... Maybe you can help me with that thing inside this pipe ?",
            "finishTrial01", 1, 2));
    }

    void OnBallCompleted()
    {
        lastActionTime = Time.time;
        CancelScheduledTip(); // Annuler le tip en attente
        Debug.Log("✅ Trial 02 - Ball completed");

        StartCoroutine(SpeakThenQuestionTip(
            "So you get this useless lamp. Crazy that with your eyes only you can't see a message that obvious on the door. That make me think that the employes flash it on the painting a lot",
            "finishTrial02", 2, 3));
    }

    void OnMazeCompleted()
    {
        lastActionTime = Time.time;
        CancelScheduledTip(); // Annuler le tip en attente
        Debug.Log("✅ Trial 03 - Maze completed");

        StartCoroutine(SpeakThenQuestionTip(
            "Congrats ! You are not the slowest human but not by far ! For me, it is easy to see it, but you might need something more to see the true beauty of the best employes",
            "finishTrial03", 3, 4));
    }

    void OnCodeCompleted()
    {
        lastActionTime = Time.time;
        CancelScheduledTip(); // Annuler le tip en attente
        Debug.Log("✅ Trial 04 - Code completed");

        StartCoroutine(SpeakThenQuestionTip(
            "Keep it up ! Now i'm sure that you know what's behind that hole. But before that, Security question !",
            "finishTrial04", 4, 5));
    }

    IEnumerator SpeakThenQuestionTip(string text, string audioClip, int questionNumber, int tipNumber)
    {
        yield return StartCoroutine(Speak(text, audioClip));
        yield return new WaitForSeconds(1f);

        // Poser la question
        AskQuestion(questionNumber);

        // Attendre que la question soit répondue
        while (currentState == GameState.WaitingForAnswer)
        {
            yield return null;
        }

        // Attendre 1 seconde après la réponse
        yield return new WaitForSeconds(1f);

        // Planifier le prochain tip
        currentTipCoroutine = StartCoroutine(ShowTipAfterDelay(tipNumber, tipsTime));
    }

    void AskQuestion(int questionNumber)
    {
        Debug.Log($"🎯 AskQuestion({questionNumber}) appelée");

        switch (questionNumber)
        {
            case 1:
                if (question1Asked)
                {
                    Debug.LogWarning("⚠️ Question 1 déjà posée!");
                    return;
                }

                question1Asked = true;
                StartCoroutine(SpeakThenShowQuestion("", "question01", 1));
                break;
            case 2:
                if (question2Asked)
                {
                    Debug.LogWarning("⚠️ Question 2 déjà posée!");
                    return;
                }

                question2Asked = true;
                StartCoroutine(SpeakThenShowQuestion("", "question02", 2));
                break;
            case 3:
                if (question3Asked)
                {
                    Debug.LogWarning("⚠️ Question 3 déjà posée!");
                    return;
                }

                question3Asked = true;
                StartCoroutine(SpeakThenShowQuestion("", "question03", 3));
                break;
            case 4:
                if (question4Asked)
                {
                    Debug.LogWarning("⚠️ Question 4 déjà posée!");
                    return;
                }

                question4Asked = true;
                StartCoroutine(SpeakThenShowQuestion("", "question04", 4));
                break;
            case 5:
                if (question5Asked)
                {
                    Debug.LogWarning("⚠️ Question 5 déjà posée!");
                    return;
                }

                question5Asked = true;
                StartCoroutine(SpeakThenShowQuestion("", "question05", 5));
                break;
            case 6:
                if (question6Asked)
                {
                    Debug.LogWarning("⚠️ Question 6 déjà posée!");
                    return;
                }

                question6Asked = true;
                StartCoroutine(SpeakThenShowQuestion("", "Trial05", 6));
                break;
        }
    }

    IEnumerator SpeakThenShowQuestion(string text, string audioClip, int questionNum)
    {
        if (!string.IsNullOrEmpty(audioClip))
        {
            yield return StartCoroutine(Speak(text, audioClip));
        }

        yield return new WaitForSeconds(0.5f);

        switch (questionNum)
        {
            case 1: AskQuestion1(); break;
            case 2: AskQuestion2(); break;
            case 3: AskQuestion3(); break;
            case 4: AskQuestion4(); break;
            case 5: AskQuestion5(); break;
            case 6: AskQuestion6(); break;
        }
    }

    void AskQuestion1()
    {
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.NeutralEye();
        Debug.Log("=== AskQuestion1 START ===");

        currentState = GameState.WaitingForAnswer;
        currentCorrectAnswer = 2;
        lastAnswer = -1;

        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(false);
        }

        if (questionPanel != null)
        {
            questionPanel.gameObject.SetActive(true);
            questionPanel.ResetAnswer();
            questionPanel.SetQuestion(
                "All right! Let's see your knowledge about D.O.O.R.H.! How many keys are there on me?");

            string[] answers = { "5", "10", "20", "50" };
            questionPanel.SetButtonsText(answers);

            questionPanel.ButtonPanelVisibility(true);
        }
    }

    void AskQuestion2()
    {
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.NeutralEye();
        Debug.Log("=== AskQuestion2 START ===");

        currentState = GameState.WaitingForAnswer;
        currentCorrectAnswer = 0;
        lastAnswer = -1;

        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(false);
        }

        if (questionPanel != null)
        {
            questionPanel.gameObject.SetActive(true);
            questionPanel.ResetAnswer();
            questionPanel.SetQuestion(
                "Daily survey of evolution within our large company D.O.O.R.H, please give us your thoughts! Is the marble on top of me magnetic?");

            string[] answers = { "Yes", "No", "Maybe", "I don't know" };
            questionPanel.SetButtonsText(answers);

            questionPanel.ButtonPanelVisibility(true);
        }
    }

    void AskQuestion3()
    {
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.NeutralEye();
        Debug.Log("=== AskQuestion3 START ===");

        currentState = GameState.WaitingForAnswer;
        currentCorrectAnswer = 1;
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
                ? "Well well well, I think you're hiding things from us, answer to this! Just a little basic security investigation! *polite laugh* Nothing dangerous! "
                : "Well, I would need to collect some information, just a quick satisfaction survey! ";

            questionPanel.SetQuestion(questionPrefix + "Who are you?");

            string[] answers = { "The boss", "An employee", "A Thief", "MAX" };
            questionPanel.SetButtonsText(answers);

            questionPanel.ButtonPanelVisibility(true);
        }
    }

    void AskQuestion4()
    {
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.NeutralEye();
        Debug.Log("=== AskQuestion4 START ===");

        currentState = GameState.WaitingForAnswer;
        currentCorrectAnswer = 1;
        lastAnswer = -1;

        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(false);
        }

        if (questionPanel != null)
        {
            questionPanel.gameObject.SetActive(true);
            questionPanel.ResetAnswer();
            questionPanel.SetQuestion(
                "On the employees of the month board, who has the most chances of being promoted?");

            string[] answers = { "A", "B", "C", "D" };
            questionPanel.SetButtonsText(answers);

            questionPanel.ButtonPanelVisibility(true);
        }
    }

    void AskQuestion5()
    {
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.NeutralEye();
        Debug.Log("=== AskQuestion5 START ===");

        currentState = GameState.WaitingForAnswer;
        currentCorrectAnswer = 3; // "Please don't"
        lastAnswer = -1;

        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(false);
        }

        if (questionPanel != null)
        {
            questionPanel.gameObject.SetActive(true);
            questionPanel.ResetAnswer();
            questionPanel.SetQuestion(
                "In the hole, if you put your hand in it, do you think you risk being electrocuted?");

            string[] answers = { "Yes", "No", "Maybe", "Please don't" };
            questionPanel.SetButtonsText(answers);

            questionPanel.ButtonPanelVisibility(true);
        }

        Debug.Log("⚠️ Question 5 posée - Après réponse → Question 6");
    }

    void AskQuestion6()
    {
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.NeutralEye();
        Debug.Log("=== AskQuestion6 START (FINAL QUESTION - Trial05) ===");

        currentState = GameState.WaitingForAnswer;
        currentCorrectAnswer = 2; // C
        lastAnswer = -1;

        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(false);
        }

        if (questionPanel != null)
        {
            questionPanel.gameObject.SetActive(true);
            questionPanel.ResetAnswer();
            questionPanel.SetQuestion("What object IS in the hole between these choices:");

            string[] answers = { "A Coin", "B A Rat", "C Fan", "D Key" };
            questionPanel.SetButtonsText(answers);

            questionPanel.ButtonPanelVisibility(true);
        }

        Debug.Log("⚠️ ATTENTION: Répondre à cette question déclenche la suite!");
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
        Debug.Log($"📝 OnAnswerSelected! IsCorrect: {isCorrect}, Q4: {question4Asked}, Q5: {question5Asked}, Q6: {question6Asked}");
        
        if (questionPanel != null)
        {
            questionPanel.ButtonPanelVisibility(false);
            questionPanel.gameObject.SetActive(false);
        }
        
        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(true);
        }
        
        if (!isCorrect)
        {
            AddSuspicion(5f);
            Debug.Log("❌ Mauvaise réponse - Suspicion ajoutée");
            
            if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
                UIManager.Instance.UiEye.SusEye();
        }
        else
        {
            Debug.Log("✅ Bonne réponse !");
            if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
                UIManager.Instance.UiEye.HappyEye();
        }
        
        // CORRECTION ICI : Vérifier quelle question vient d'être répondue
        // Question 4 → déclencher Question 5
        if (question4Asked && !question5Asked)
        {
            Debug.Log("🎯 Question 4 répondue → Déclenchement Question 5");
            StartCoroutine(ReturnToPlayingThenAskQuestion(6f, 5));
        }
        // Question 5 → déclencher Question 6
        else if (question5Asked && !question6Asked)
        {
            Debug.Log("🎯 Question 5 répondue → Déclenchement Question 6");
            StartCoroutine(ReturnToPlayingThenAskQuestion(6f, 6));
        }
        // Question 6 → déclencher la victoire
        else if (question6Asked)
        {
            Debug.Log("🎉 Question 6 répondue → VICTOIRE!");
            StartCoroutine(ReturnToPlayingThenWin(1f));
        }
        else
        {
            StartCoroutine(ReturnToPlayingAfterDelay(1f));
        }
    }

    IEnumerator ReturnToPlayingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentState = GameState.Playing;
        lastActionTime = Time.time;
        currentCorrectAnswer = -1;
        lastAnswer = -1;

        Debug.Log("🎮 Answer processed, returning to Playing state");
    }

    IEnumerator ReturnToPlayingThenAskQuestion(float delay, int questionNumber)
    {
        yield return new WaitForSeconds(delay);

        currentState = GameState.Playing;
        lastActionTime = Time.time;
        currentCorrectAnswer = -1;
        lastAnswer = -1;

        Debug.Log($"🎮 Déclenchement de la Question {questionNumber}!");

        // Déclencher la question
        AskQuestion(questionNumber);
    }

    IEnumerator ReturnToPlayingThenWin(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentState = GameState.Playing;
        lastActionTime = Time.time;
        currentCorrectAnswer = -1;
        lastAnswer = -1;

        Debug.Log("🎮 Answer processed, triggering WIN!");

        // Déclencher immédiatement la victoire
        GameWin();
    }

    void GameWin()
    {
        if (currentState == GameState.GameWin) return;

        currentState = GameState.GameWin;

        // Annuler tout tip planifié
        CancelScheduledTip();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.StopTimer();
            if (UIManager.Instance.UiTimer != null)
            {
                UIManager.Instance.UiTimer.TimerVisibility(false);
            }
        }

        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        yield return StartCoroutine(
            Speak("Well done! You succeeded all the verification steps! Enjoy your day at work!", "finishTrial05"));

        yield return StartCoroutine(Speak("Super! I feel like you're ready to climb the career ladder! Keep going!",
            "end"));

        Debug.Log("🎉 GAME WIN !");
        GameData.win = true;
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.HappyEye();

        yield return new WaitForSeconds(2f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("UI_MainEnd");
    }

    void GameOverSuspicion()
    {
        if (currentState != GameState.Playing && currentState != GameState.WaitingForAnswer) return;

        currentState = GameState.GameOverSuspicion;

        // Annuler tout tip planifié
        CancelScheduledTip();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.StopTimer();
            if (UIManager.Instance.UiTimer != null)
            {
                UIManager.Instance.UiTimer.TimerVisibility(false);
            }
        }

        if (questionPanel != null)
        {
            questionPanel.ButtonPanelVisibility(false);
            questionPanel.gameObject.SetActive(false);
        }

        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
            UIManager.Instance.UiPanelText.PanelTextVisibility(true);
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.EndingEye(false);

        StartCoroutine(GameOverSequence(
            "An intruder has been detected in front of our grand company D.O.O.R.H. Please do not panic,\nour teams will take care of it. Stay close to your station post and keep serving our society.",
            "gameOverSuspicion", false));

        Debug.Log("💀 GAME OVER - Suspicion");
        GameData.win = false;
    }

    void GameOverTimeout()
    {
        if (currentState == GameState.GameOverTimeout) return;

        currentState = GameState.GameOverTimeout;

        // Annuler tout tip planifié
        CancelScheduledTip();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.StopTimer();
            if (UIManager.Instance.UiTimer != null)
            {
                UIManager.Instance.UiTimer.TimerVisibility(false);
            }
        }

        if (questionPanel != null)
        {
            questionPanel.ButtonPanelVisibility(false);
            questionPanel.gameObject.SetActive(false);
        }

        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
            UIManager.Instance.UiPanelText.PanelTextVisibility(true);
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.EndingEye(false);
        StartCoroutine(GameOverSequence(
            "Please excuse us, but you did not meet our basic security quotaasked by the company to each employee.\nWe will sadly have to send a security team to evacuate you.",
            "gameOverTimeout", false));

        Debug.Log("💀 GAME OVER - Timeout");
        GameData.win = false;
    }

    void GameOverTouch()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.GameOverTouch;

        // Annuler tout tip planifié
        CancelScheduledTip();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.StopTimer();
            if (UIManager.Instance.UiTimer != null)
            {
                UIManager.Instance.UiTimer.TimerVisibility(false);
            }
        }

        ShowMaxDialog("Stop touching everything! Security has been alerted!");

        Debug.Log($"💀 GAME OVER - Touch limit ({touchCount} touches)");
        GameData.win = false;

        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.EndingEye(false);

        StartCoroutine(LoadMainMenuAfterDelay(5f));
    }

    IEnumerator GameOverSequence(string text, string audioClip, bool happy)
    {
        yield return StartCoroutine(Speak(text, audioClip));

        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.EndingEye(happy);

        yield return new WaitForSeconds(3f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("UI_MainEnd");
    }

    public void AddSuspicion(float amount = 1f)
    {
        if (currentState != GameState.Playing && currentState != GameState.WaitingForAnswer) return;

        suspicious += amount;
        lastActionTime = Time.time;

        Debug.Log($"Suspicion ajoutée: {amount} (Total: {suspicious}/{maxSuspicious})");

        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.SusEye();
    }

    void ShowMaxDialog(string text)
    {
        if (maxDialogText && UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            // S'assurer que le panel est visible avant d'appeler SetPanelText
            UIManager.Instance.UiPanelText.PanelTextVisibility(true);
            string textF = UIManager.Instance.UiPanelText.SetPanelText(text, 0.05f);
            maxDialogText.text = "MAX: " + textF;
        }
    }

    void UpdateStatusDisplay()
    {
        if (currentState == GameState.Playing)
        {
            int completed = 0;
            if (umbrellaCompleted) completed++;
            if (ballCompleted) completed++;
            if (mazeCompleted) completed++;
            if (codeCompleted) completed++;
        }
    }

    // void OnGUI()
    // {
    //     float currentTimer = UIManager.Instance != null ? UIManager.Instance.currentTimer : 0f;
    //     float maxTimer = UIManager.Instance != null ? UIManager.Instance.loseTimer : 900f;
    //     
    //     GUI.Label(new Rect(10, 10, 400, 260),
    //         $"<size=16><color=white>" +
    //         $"État: {currentState}\n" +
    //         $"Timer: {currentTimer:F1}s / {maxTimer}s\n" +
    //         $"Umbrella: {umbrellaCompleted}\n" +
    //         $"Ball: {ballCompleted}\n" +
    //         $"Maze: {mazeCompleted}\n" +
    //         $"Code: {codeCompleted}\n" +
    //         $"Touch: {touchCount}/{maxTouchBeforeGameOver}\n" +
    //         $"Suspicion: {suspicious:F1}/{maxSuspicious}\n" +
    //         $"Q1: {question1Asked} | Q2: {question2Asked}\n" +
    //         $"Q3: {question3Asked} | Q4: {question4Asked}\n" +
    //         $"Q5: {question5Asked} | Q6: {question6Asked}\n" +
    //         $"Is Speaking: {isSpeaking}" +
    //         $"</color></size>");
    // }
}