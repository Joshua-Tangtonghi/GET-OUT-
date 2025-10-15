using UnityEngine;

public class UiTimer : MonoBehaviour
{
    public UiText timerText;
    private float currentTimer;
    public float loseTimer = 900;

    private void Awake()
    {
        if (timerText == null)
        {
            timerText = GetComponentInChildren<UiText>();
        }
    }
    public void SetTimer(string t)
    {
        timerText.SetText(t);
    }
    public void UpdateTimerDisplay(float timer)
    {
        currentTimer = loseTimer - timer;
        int minutes = Mathf.FloorToInt(currentTimer / 60f);
        int seconds = Mathf.FloorToInt(currentTimer % 60f);
        SetTimer(string.Format("{0:00}:{1:00}", minutes, seconds));
    }
    public void TimerVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
