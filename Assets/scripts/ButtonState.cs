using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class ButtonState : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] Image ImgQr;
    [SerializeField] Image ImgMap;
    
    [Header("Materials")]
    [SerializeField] Material ActiveMat;
    [SerializeField] Material InactiveMat;

    [Header("Icons")]
    [SerializeField] GameObject MapIcon;
    [SerializeField] GameObject QrIcon;

    [Header("Map/Qr")]
    [SerializeField] GameObject MapWindow;
    [SerializeField] GameObject QrWindow;

    [Header("QR Scanner")]
    [SerializeField] QrScanner qrScanner; // Reference to your QrScanner script

    bool MapActive;

    void Start()
    {
        MapActive = true;
        ImgMap.material = ActiveMat;
        ImgQr.material = InactiveMat;
        MapIcon.transform.localScale = new Vector2(1.76f, 1.76f);
        QrIcon.transform.localScale = new Vector2(0.6f, 0.6f);
        QrWindow.SetActive(false);
        MapWindow.SetActive(true);
    }

    public void ChangeStateMap()
    {
        if (!MapActive)
        {
            MapActive = true;
            ImgMap.material = ActiveMat;
            ImgQr.material = InactiveMat;
            MapIcon.transform.localScale = new Vector2(1.76f, 1.76f);
            QrIcon.transform.localScale = new Vector2(0.6f, 0.6f);
            QrWindow.SetActive(false);
            MapWindow.SetActive(true);

            qrScanner?.StopQR(); // Stop camera when switching out of QR mode
        }
    }

    public void ChangeStateQr()
    {
        if (MapActive)
        {
            MapActive = false;
            ImgMap.material = InactiveMat;
            ImgQr.material = ActiveMat;
            MapIcon.transform.localScale = new Vector2(1, 1);
            QrIcon.transform.localScale = new Vector2(1, 1);
            QrWindow.SetActive(true);
            MapWindow.SetActive(false);

            qrScanner?.StartQR(); // Start camera when entering QR mode
        }
    }
}
