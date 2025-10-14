using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UiTimer UiTimer;
    public UiPanelText UiPanelText;
    public UiPannelButtons UiPannelButtons;
    public UiEye UiEye;
    public UiCredits UiCredits;

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
        if (UiEye == null)
            UiEye = GetComponentInChildren<UiEye>();
        if (UiCredits == null)
        {
            UiCredits = GetComponentInChildren<UiCredits>();
        }
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
    public void PlayCredit()
    {
        UiPanelText.PanelTextVisibility(false);
        UiPannelButtons.ButtonPanelVisibility(false);
        UiEye.gameObject.SetActive(false);
        StopTimer();
        UiCredits.gameObject.SetActive(true);
    }
}
