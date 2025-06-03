using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class QrScanner : MonoBehaviour
{
    [SerializeField] RawImage camDisplay;
    Text resultText;

    private WebCamTexture webCamTexture;
    private IBarcodeReader barcodeReader = new BarcodeReader();

    private bool isCameraRunning = false;

    private float scanInterval = 1.0f;
    private float scanTimer = 0f;
    /// 
    /// 
    public GameObject[] stations;

    public int teamID;
    private List<string> baseOrder = new List<string> { "clue_01", "clue_02", "clue_03", "clue_04", "clue_05", "clue_06" };
    private List<string> teamOrder = new List<string>();
    private int currentStep = 0;

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

        camDisplay.rectTransform.localEulerAngles = new Vector3(0, 0, -webCamTexture.videoRotationAngle);
        camDisplay.uvRect = webCamTexture.videoVerticallyMirrored ?
            new Rect(0, 1, 1, -1) :
            new Rect(0, 0, 1, 1);

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
            Destroy(snap); // Free memory!

            if (result != null)
            {
                Debug.Log("Qr Detected: " + result.Text);
                resultText.text = "Qr: " + result.Text;
                HandleQrCode(result.Text);
                StopQR(); // Stop camera if needed
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("QR decode error: " + e.Message);
        }
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

    public void SetTeamOrder()
    {
        teamOrder.Clear();
        for (int i = 0; i < baseOrder.Count; i++)
        {
         int index = (i + teamID) % baseOrder.Count;
         teamOrder.Add(baseOrder[index]);
        }

        currentStep = 0;
        Debug.Log("Team Order Set: " + string.Join(", ", teamOrder));
    }



   void HandleQrCode(string code)
{
    if (teamOrder.Count == 0)
    {
        Debug.LogWarning("Team order not set yet!");
        return;
    }

    if (currentStep >= teamOrder.Count)
    {
        Debug.Log("All clues already scanned.");
        return;
    }

    string expectedCode = teamOrder[currentStep];
    
    if (code == expectedCode)
        {
            Debug.Log($"Correct QR for step {currentStep + 1}! ({code})");

            // Activate corresponding station
            int stationIndex = baseOrder.IndexOf(code);
            if (stationIndex >= 0 && stationIndex < stations.Length)
            {
                stations[stationIndex].SetActive(true);
                Debug.Log($"Activated station: {stations[stationIndex].name}");
            }
            else
            {
                Debug.LogWarning($"Station not found for QR code: {code}");
            }

            currentStep++;

            if (currentStep >= teamOrder.Count)
            {
                Debug.Log("All steps completed!");
            }
        }
        else
        {
            Debug.Log($"Incorrect QR. Expected: {expectedCode}, got: {code}");
        }
}


}
