using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text Timer;
    public TMP_Text Views;
    public TMP_Text Group;
    public TMP_Dropdown GroupDropdown;
    public GameObject Button;

    public QrScanner qrScanner;

    //functions
    void GroupSelection()
{
    int teamId = GroupDropdown.value;
    Debug.Log("Selected Group: " + teamId);

    // Set team in QR scanner
    qrScanner.teamID = teamId;
    qrScanner.SetTeamOrder(); // You may need to make this public in QrScanner
}


    public void setDisapearSelection()
    {
        GroupSelection(); // <-- Call this first
        
        Button.SetActive(false);
        GroupDropdown.gameObject.SetActive(false);
        Group.gameObject.SetActive(true);
        Group.text = GroupDropdown.options[GroupDropdown.value].text;
    }

    
}
