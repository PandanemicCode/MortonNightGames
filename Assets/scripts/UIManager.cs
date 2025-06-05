using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text Timer;           // Assign your TextMeshPro text here
    public TMP_Text ViewsTMP;
    public int ViewCount;
    public TMP_Text Group;
    public TMP_Dropdown GroupDropdown;
    public GameObject Panel;

    [Header("References")]
    [SerializeField] private QrScanner qrScanner; // Drag your QrScanner here

    [Header("Timer Settings")]
    [Tooltip("Duration in seconds (e.g., 3600 = 1 hour).")]
    public float totalTime = 3600f;
    private float currentTime;
    private bool isTimerRunning = false;
    int viewValues = 0;

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                isTimerRunning = false;
            }

            int hours = Mathf.FloorToInt(currentTime / 3600);
            int minutes = Mathf.FloorToInt((currentTime % 3600) / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);

            Timer.text = $"{hours:00}:{minutes:00}:{seconds:00}";
        }
    }

    #region GroupSelect

    void GroupSelection()
    {
        int selectedIndex = GroupDropdown.value;
        Debug.Log("Group selection changed to: " + selectedIndex);
    }

    public void setDisapearSelection()
    {
        // 1) Select team
        int selectedIndex = GroupDropdown.value;
        Group.text = GroupDropdown.options[selectedIndex].text;

        // 2) Pass to QrScanner
        qrScanner.teamID = selectedIndex;
        qrScanner.SetTeamOrder();

        // 3) Hide selection UI
        Panel.SetActive(false);
        Group.gameObject.SetActive(true);

        // 4) Start timer
        StartTimer();
    }

    #endregion
    #region Timer
    public void StartTimer()
    {
        currentTime = totalTime;
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = totalTime;
        isTimerRunning = false;
        int hours = Mathf.FloorToInt(totalTime / 3600);
        Timer.text = $"{hours:00}:00:00";
    }
    #endregion
   
}
