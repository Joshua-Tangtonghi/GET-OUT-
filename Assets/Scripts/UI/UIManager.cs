using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UiTimer UiTimer;
    public UiPanelText UiPanelText;
    public UiPannelButtons UiPannelButtons;
    public UiEye UiEye;

    private float timer;
    private bool isTimerRunning = false;
    public float currentTimer;
    public float loseTimer = 900;
    public bool win = true;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
        if (UiTimer == null)
            UiTimer = GetComponentInChildren<UiTimer>();
        if (UiPanelText == null)
            UiPanelText = GetComponentInChildren<UiPanelText>();
        if (UiPannelButtons == null)
            UiPannelButtons = GetComponentInChildren<UiPannelButtons>();
        if (UiEye == null)
            UiEye = GetComponentInChildren<UiEye>();
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
            currentTimer = loseTimer - timer;
            UiTimer.UpdateTimerDisplay(currentTimer);
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
    public void PlayCredits()
    {
        gameObject.SetActive(false);
    }
}
public static class GameData
{
    public static bool win = true;
}
