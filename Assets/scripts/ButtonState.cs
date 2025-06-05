using UnityEngine;
using UnityEngine.UI;

public class ButtonState : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] private Image ImgQr;
    [SerializeField] private Image ImgMap;

    [Header("Materials")]
    [SerializeField] private Material ActiveMat;
    [SerializeField] private Material InactiveMat;

    [Header("Icons")]
    [SerializeField] private GameObject MapIcon;
    [SerializeField] private GameObject QrIcon;

    [Header("Map / QR Windows")]
    [SerializeField] private GameObject MapWindow;
    [SerializeField] private GameObject QrWindow;

    [Header("QR Scanner Reference")]
    [Tooltip("Drag your QrScanner GameObject here")]
    [SerializeField] public QrScanner qrScanner;

    private bool MapActive;

    void Start()
    {
        // By default, we start showing the Map
        MapActive = true;
        ImgMap.material = ActiveMat;
        ImgQr.material = InactiveMat;
        MapIcon.transform.localScale = new Vector2(1.76f, 1.76f);
        QrIcon.transform.localScale = new Vector2(0.6f, 0.6f);
        QrWindow.SetActive(false);
        MapWindow.SetActive(true);
        
        //stop phone from rotating
         Screen.orientation = ScreenOrientation.Portrait;
         Screen.autorotateToLandscapeLeft = false;
         Screen.autorotateToLandscapeRight = false;
         Screen.autorotateToPortraitUpsideDown = false;
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

            // If the camera was running, stop it
            qrScanner?.StopQR();
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

            // Start the camera when entering QR mode
            qrScanner?.StartQR();
        }
    }
}
