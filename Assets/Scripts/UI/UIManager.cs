using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UiTimer UiTimer;
    public UiPanelText UiPanelText;
    public UiPannelButtons UiPannelButtons;

    [Header("UI Elements")]
    public Text timerText;
    public Text infoText;

    private float timer;
    private bool isTimerRunning = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (UiTimer == null)
            UiTimer = GetComponentInChildren<UiTimer>();
        if (UiPanelText == null)
            UiPanelText = GetComponentInChildren<UiPanelText>();
        if (UiPannelButtons == null)
            UiPannelButtons = GetComponentInChildren<UiPannelButtons>();
    }
    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            UiTimer.UpdateTimerDisplay(timer);
        }
    }

    public void StartTimer()
    {
        timer = 0f;
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void SetInfoText(string message)
    {
        infoText.text = message;
    }

    public void HideInfoText()
    {
        infoText.text = "";
    }
}
