using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text Timer;
    public TMP_Text Views;
    public TMP_Text Group;
    public TMP_Dropdown GroupDropdown;
    public GameObject Button;

    [Tooltip("Stations or other objects you might activate later")]
    public GameObject[] stations;

    [Header("References")]
    [SerializeField] private QrScanner qrScanner; // Drag your QrScanner here

    void GroupSelection()
    {
        int selectedIndex = GroupDropdown.value;
        Debug.Log("Group selection changed to: " + selectedIndex);

        // If you want to pass to QrScanner later:
        // qrScanner.teamID = selectedIndex;
        // qrScanner.SetTeamOrder();
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
        Button.SetActive(false);
        GroupDropdown.gameObject.SetActive(false);
        Group.gameObject.SetActive(true);
    }
}
