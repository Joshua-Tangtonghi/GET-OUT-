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
    public float tipsTime = 30f;

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
        ShowingTip,
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

    // --- Audio/Speak lock
    private bool isSpeaking = false;
    public bool flag = true; 

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

    // ---------------------------
    // Speak helper - affiche le texte et joue l'audio, sans chevauchement
    // ---------------------------
    IEnumerator Speak(string text, string audioClipName)
    {
        // Si une autre phrase est en cours, attend qu'elle finisse
        while (isSpeaking)
            yield return null;

        isSpeaking = true;

        // Afficher le texte via UiPanelText (progressif ou direct selon impl)
        ShowMaxDialog(text);

        // Si on a un AudioManager et un clip, joue et attends la fin via WaitForSoundEnd (doit exister)
        if (AudioManager.Instance != null && !string.IsNullOrEmpty(audioClipName))
        {
            AudioManager.Instance.Play(audioClipName);

            // Utiliser la coroutine fournie par AudioManager pour attendre la fin du clip
            // (AudioManager.WaitForSoundEnd doit être implémentée comme IEnumerator)
            yield return StartCoroutine(AudioManager.Instance.WaitForSoundEnd(audioClipName));
        }
        else
        {
            // Fallback : durée approximative basée sur longueur du texte
            float fallback = Mathf.Clamp(1f + text.Length * 0.03f, 1f, 6f);
            yield return new WaitForSeconds(fallback);
        }

        // Nettoyage textuel
        if (maxDialogText != null)
            maxDialogText.text = "";

        isSpeaking = false;
    }

    // ---------------------------
    // Intro sequence (utilise Speak pour synchroniser audio + texte)
    // ---------------------------
    IEnumerator IntroSequence()
    {
        currentState = GameState.Intro;

        yield return StartCoroutine(Speak("Hello. I am M.A.X. I am here to be sure you are qualified to enter.", "start01"));
        yield return StartCoroutine(Speak("Of course, you are suppose to know the steps to unlock me.\nPlease press the button to go to the next step. Every step is separated by a button like this one", "start02"));
        yield return StartCoroutine(Speak("Well let's see if you are really authorized to enter. You know you need an umbrella right ? You might want to check in that umbrella holder if you forgot yours.", "start03"));
        yield return StartCoroutine(Speak("It is basic knowledge to know which key is which to start your day right ?\nAnd a good day starts with an umbrella", "startTrials01"));

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
        
        // Démarrer la coroutine pour afficher le premier tip après tipsTime
        StartCoroutine(ShowTipAfterDelay(1, tipsTime));
    }

    IEnumerator ShowTipAfterDelay(int tipNumber, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Vérifier si on est toujours en jeu avant d'afficher le tip
        if (currentState == GameState.Playing)
        {
            yield return StartCoroutine(ShowTip(tipNumber));
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
                tipMessage = "You...You're alright ? Hard to remember something so obvious ? Well I can understand... Or not. \nHow do want to open a lock without a key ?";
                audioClip = "tipsTrials01";
                break;
            case 2:
                tipMessage = "You know that an umbrella can get you to place much higher ?";
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
        if (flag)
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
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 01 - Key completed");
        
        StartCoroutine(QuestionTipSequence(1, 2));
    }

    void OnUmbrellaCompleted()
    {
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 01 Finish - Umbrella completed");
        
        // Attendre la fin de l'audio avant la question
        StartCoroutine(PlayAudioThenQuestionTip("finishTrial01", 2, 3));
    }

    void OnBallCompleted()
    {
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 02 - Ball completed");
        
        // Attendre la fin de l'audio avant la question
        StartCoroutine(PlayAudioThenQuestionTip("finishTrial02", 3, 4));
    }

    void OnCodeUVCompleted()
    {
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 03 - UV Code completed");
        
        // Pas de question ici, juste démarrer le tip après tipsTime
        StartCoroutine(ShowTipAfterDelay(4, tipsTime));
    }

    void OnMazeCompleted()
    {
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 03 Finish - Maze completed");
        
        // Attendre la fin de l'audio avant la question
        StartCoroutine(PlayAudioThenQuestionTip("finishTrial03", 4, 5));
    }

    void OnCodeCompleted()
    {
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 04 - Code completed");
        
        // Attendre la fin de l'audio avant la question
        StartCoroutine(PlayAudioThenQuestion("finishTrial04", 5));
    }

    void OnCaptchaCompleted()
    {
        lastActionTime = Time.time;
        Debug.Log("✅ Trial 05 - Captcha completed");
        
        // Jouer l'audio de fin puis vérifier la condition de victoire
        if (AudioManager.Instance != null)
        {
            // utiliser Speak pour garantir l'absence de chevauchement
            StartCoroutine(FinishCaptchaAndCheckWin());
        }
        else
        {
            CheckWinCondition();
        }
    }

    IEnumerator FinishCaptchaAndCheckWin()
    {
        yield return StartCoroutine(Speak("", "finishTrial05"));
        CheckWinCondition();
    }

    IEnumerator PlayAudioThenQuestionTip(string audioClip, int questionNumber, int tipNumber)
    {
        if (!string.IsNullOrEmpty(audioClip) && AudioManager.Instance != null)
        {
            // joue l'audio et attends sa fin proprement
            yield return StartCoroutine(Speak("", audioClip));
        }
        
        // Puis lancer la séquence question + tip
        yield return StartCoroutine(QuestionTipSequence(questionNumber, tipNumber));
    }

    IEnumerator PlayAudioThenQuestion(string audioClip, int questionNumber)
    {
        if (!string.IsNullOrEmpty(audioClip) && AudioManager.Instance != null)
        {
            yield return StartCoroutine(Speak("", audioClip));
        }
        
        // Puis poser la question
        yield return new WaitForSeconds(2f);
        AskQuestion(questionNumber);
    }

    IEnumerator QuestionTipSequence(int questionNumber, int tipNumber)
    {
        yield return new WaitForSeconds(2f);
        AskQuestion(questionNumber);
        
        // Attendre que la question soit répondue
        while (currentState == GameState.WaitingForAnswer)
        {
            yield return null;
        }
        
        // Attendre le délai de ReturnToPlayingAfterDelay (simulate 3s wait)
        yield return new WaitForSeconds(3f);
        
        // Afficher le tip après tipsTime
        StartCoroutine(ShowTipAfterDelay(tipNumber, tipsTime));
    }

    void AskQuestion(int questionNumber)
    {
        switch (questionNumber)
        {
            case 1:
                if (question1Asked) return;
                question1Asked = true;
                // jouer audio via Speak when showing question (we play here to match previous flow)
                StartCoroutine(Speak("", "question01"));
                AskQuestion1();
                break;
            case 2:
                if (question2Asked) return;
                question2Asked = true;
                StartCoroutine(Speak("", "question02"));
                AskQuestion2();
                break;
            case 3:
                if (question3Asked) return;
                question3Asked = true;
                StartCoroutine(Speak("", "question03"));
                AskQuestion3();
                break;
            case 4:
                if (question4Asked) return;
                question4Asked = true;
                StartCoroutine(Speak("", "question04"));
                AskQuestion4();
                break;
            case 5:
                if (question5Asked) return;
                question5Asked = true;
                StartCoroutine(Speak("", "question05"));
                AskQuestion5();
                break;
        }
    }

    void AskQuestion1()
    {
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
            questionPanel.SetQuestion("All right! Let's see your knowledge about D.O.O.R.H.! How many keys are there on me?");
            
            string[] answers = { "5", "10", "20", "50" };
            questionPanel.SetButtonsText(answers);
            
            questionPanel.ButtonPanelVisibility(true);
        }
    }

    void AskQuestion2()
    {
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
            questionPanel.SetQuestion("Daily survey of evolution within our large company D.O.O.R.H, please give us your thoughts! Is the marble on top of me magnetic?");
            
            string[] answers = { "Yes", "No", "Maybe", "I don't know" };
            questionPanel.SetButtonsText(answers);
            
            questionPanel.ButtonPanelVisibility(true);
        }
    }

    void AskQuestion3()
    {
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
                ? "Well well well, I think you're hiding things from us, answer to this! Just a little basic security investigation! *polite laugh* Nothing dangerous!\n\n"
                : "Well, I would need to collect some information, just a quick satisfaction survey!\n\n";
            
            questionPanel.SetQuestion(questionPrefix + "Who are you?");
            
            string[] answers = { "The boss", "An employee", "A security agent", "An intern" };
            questionPanel.SetButtonsText(answers);
            
            questionPanel.ButtonPanelVisibility(true);
        }
    }

    void AskQuestion4()
    {
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
            questionPanel.SetQuestion("On the employees of the month board, who has the most chances of being promoted?");
            
            string[] answers = { "A", "B", "C", "D" };
            questionPanel.SetButtonsText(answers);
            
            questionPanel.ButtonPanelVisibility(true);
        }
    }

    void AskQuestion5()
    {
        Debug.Log("=== AskQuestion5 START ===");
        
        currentState = GameState.WaitingForAnswer;
        currentCorrectAnswer = 3;
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
        }
        
        if (UIManager.Instance != null && UIManager.Instance.UiPanelText != null)
        {
            UIManager.Instance.UiPanelText.PanelTextVisibility(true);
        }
        
        if (!isCorrect)
        {
            AddSuspicion(5f);
            Debug.Log("❌ Mauvaise réponse - Suspicion ajoutée");
        }
        else
        {
            Debug.Log("✅ Bonne réponse !");
        }
        
        StartCoroutine(ReturnToPlayingAfterDelay(1f));
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
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.StopTimer();
            if (UIManager.Instance.UiTimer != null)
            {
                UIManager.Instance.UiTimer.TimerVisibility(false);
            }
        }
        
        ShowMaxDialog("Well done! You succeeded all the verification steps! Enjoy your day at work!\nSuper! I feel like you're ready to climb the career ladder! Keep going!");
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.Play("end");
            
        Debug.Log("🎉 GAME WIN !");
        
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.HappyEye();
            
        StartCoroutine(LoadMainMenuAfterDelay(5f));
    }

    void GameOverSuspicion()
    {
        if (currentState != GameState.Playing && currentState != GameState.WaitingForAnswer) return;

        currentState = GameState.GameOverSuspicion;
        
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
        
        ShowMaxDialog("An intruder has been detected in front of our grand company D.O.O.R.H. Please do not panic,\nour teams will take care of it. Stay close to your station post and keep serving our society.");
        
        if (AudioManager.Instance != null)
            StartCoroutine(Speak("", "gameOverSuspicion"));
            
        Debug.Log("💀 GAME OVER - Suspicion");
        
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.EndingEye(false);
            
        StartCoroutine(LoadMainMenuAfterDelay(5f));
    }

    void GameOverTimeout()
    {
        if (currentState == GameState.GameOverTimeout) return;

        currentState = GameState.GameOverTimeout;
        
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
        
        ShowMaxDialog("Please excuse us, but you did not meet our basic security quota asked by the company to each employee.\nWe will sadly have to send a security team to evacuate you.");
        
        if (AudioManager.Instance != null)
            StartCoroutine(Speak("", "gameOverTimeout"));
            
        Debug.Log("💀 GAME OVER - Timeout");
        
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.EndingEye(false);
            
        StartCoroutine(LoadMainMenuAfterDelay(3f));
    }

    void GameOverTouch()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.GameOverTouch;
        
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
        
        if (UIManager.Instance != null && UIManager.Instance.UiEye != null)
            UIManager.Instance.UiEye.EndingEye(false);
            
        StartCoroutine(LoadMainMenuAfterDelay(5f));
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
            string textF = UIManager.Instance.UiPanelText.SetPanelText(text, 0.05f);
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
}
