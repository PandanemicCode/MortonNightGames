using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class QrScanner : MonoBehaviour
{
    [Header("Camera Display")]
    [SerializeField] private RawImage camDisplay;

    [Header("UI / ButtonState")]
    [Tooltip("Assign the ButtonState script so we can tell it to switch back to MapView")]
    [SerializeField] private ButtonState buttonState;

    [Header("Station Activation")]
    [Tooltip("Assign your station GameObjects here, in the same order as baseOrder")]
    [SerializeField] public GameObject[] stations;

    [Header("QR Order Configuration")]
    [Tooltip("List all QR strings (e.g. \"clue_01\", \"clue_02\", …) in their base sequence")]
    [SerializeField] private string[] baseOrder;

    [Tooltip("This is set by UIManager when the team is chosen (0-based index)")]
    [SerializeField] public int teamID = 0;

    private string[] teamOrder;
    private int currentStep = 0;

    private WebCamTexture webCamTexture;
    private IBarcodeReader barcodeReader = new BarcodeReader();
    private bool isCameraRunning = false;
    private float scanInterval = 1.0f;
    private float scanTimer = 0f;

    [SerializeField] UIManager uIManager;

    void Awake()
    {
        camDisplay.texture = null;
    }

    public void StartQR()
    {
        if (isCameraRunning) return;

        WebCamDevice[] devices = WebCamTexture.devices;
        string backCamName = null;
        foreach (var device in devices)
        {
            if (!device.isFrontFacing)
            {
                backCamName = device.name;
                break;
            }
        }

        webCamTexture = backCamName != null
            ? new WebCamTexture(backCamName, Screen.width, Screen.height)
            : new WebCamTexture();

        webCamTexture.Play();
        isCameraRunning = true;

        camDisplay.texture = webCamTexture;
        camDisplay.material.mainTexture = webCamTexture;
        AdjustCameraSize();
    }

    public void StopQR()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
            isCameraRunning = false;
            camDisplay.texture = null;
        }
    }

    void Update()
    {
        if (!isCameraRunning || webCamTexture == null || webCamTexture.width <= 100)
            return;

        // Correct the RawImage orientation
        camDisplay.rectTransform.localEulerAngles = new Vector3(0, 0, -webCamTexture.videoRotationAngle);
        camDisplay.uvRect = webCamTexture.videoVerticallyMirrored
            ? new Rect(0, 1, 1, -1)
            : new Rect(0, 0, 1, 1);

        // Only attempt a decode every scanInterval seconds
        scanTimer += Time.deltaTime;
        if (scanTimer >= scanInterval)
        {
            scanTimer = 0f;
            TryDecodeQr();
        }
    }

    private void TryDecodeQr()
    {
        try
        {
            Texture2D snap = new Texture2D(webCamTexture.width, webCamTexture.height);
            snap.SetPixels(webCamTexture.GetPixels());
            snap.Apply();

            var result = barcodeReader.Decode(snap.GetPixels32(), snap.width, snap.height);
            Destroy(snap);

            if (result != null)
            {
                Debug.Log("QR Detected: " + result.Text);
                HandleQrCode(result.Text);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("QR decode error: " + e.Message);
        }
    }

    private void HandleQrCode(string code)
    {
        if (teamOrder == null || teamOrder.Length == 0)
        {
            Debug.LogWarning("[QrScanner] Team order not set.");
            return;
        }

        code = code.Trim();
        string expected = teamOrder[currentStep];
        Debug.Log($"[QrScanner] Scanned \"{code}\". Expecting \"{expected}\" (step {currentStep}).");

        if (code.Equals(expected, StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"✅ QR Match for step {currentStep + 1}: {code}");

            // 1) Determine which station corresponds to this QR string
            int stationIndex = Array.IndexOf(baseOrder, code);
            if (stationIndex < 0 || stationIndex >= stations.Length)
            {
                Debug.LogWarning($"[QrScanner] No station found for QR string \"{code}\".");
            }
            else
            {
                // 2) Disable all stations first
                for (int i = 0; i < stations.Length; i++)
                {
                    stations[i].SetActive(false);
                }

                // 3) Enable only the current station
                stations[stationIndex].SetActive(true);
                Debug.Log($"Activated station GameObject: {stations[stationIndex].name}");
            }

            currentStep++;

            // 4) Stop the camera and switch back to map
            StopQR();
            //5) update view score
                ViewsUpdate();
            buttonState?.ChangeStateMap();
        }
        else
        {
            Debug.Log($"❌ QR \"{code}\" did not match expected \"{expected}\".");
        }
    }

    public void SetTeamOrder()
    {
        if (baseOrder == null || baseOrder.Length == 0)
        {
            Debug.LogWarning("[QrScanner] Base order not set.");
            return;
        }

        teamOrder = new string[baseOrder.Length];
        int offset = teamID % baseOrder.Length;

        for (int i = 0; i < baseOrder.Length; i++)
        {
            teamOrder[i] = baseOrder[(i + offset) % baseOrder.Length];
        }

        currentStep = 0;
        Debug.Log($"[QrScanner] Team order for team {teamID}: {string.Join(", ", teamOrder)}");
    }

    private void AdjustCameraSize()
    {
        if (webCamTexture == null || camDisplay == null) return;

        float videoRatio = (float)webCamTexture.width / webCamTexture.height;
        float displayRatio = camDisplay.rectTransform.rect.width / camDisplay.rectTransform.rect.height;

        if (videoRatio > displayRatio)
        {
            float height = camDisplay.rectTransform.rect.width / videoRatio;
            camDisplay.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
        else
        {
            float width = camDisplay.rectTransform.rect.height * videoRatio;
            camDisplay.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }
    }

    //Ui Updater

    void ViewsUpdate()
    {
        int score = uIManager.ViewCount;
        score += 500;
        uIManager.ViewCount = score;
        uIManager.ViewsTMP.text = score.ToString();
    }
}
