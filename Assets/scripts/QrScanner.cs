using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class QrScanner : MonoBehaviour
{
    [SerializeField] RawImage camDisplay;
    [SerializeField] Text resultText;

    private WebCamTexture webCamTexture;
    private IBarcodeReader barcodeReader = new BarcodeReader();

    void Start()
    {
        // Get all camera devices
        WebCamDevice[] devices = WebCamTexture.devices;

        // Find the first back-facing camera
        string backCamName = null;
        foreach (var device in devices)
        {
            if (!device.isFrontFacing)
            {
                backCamName = device.name;
                break;
            }
        }

        if (backCamName != null)
        {
            webCamTexture = new WebCamTexture(backCamName);
        }
        else
        {
            Debug.LogWarning("No back camera found, using default camera.");
            webCamTexture = new WebCamTexture(); // fallback
        }

        camDisplay.texture = webCamTexture;
        camDisplay.material.mainTexture = webCamTexture;
        webCamTexture.Play();
    }

    void Update()
    {
        if (webCamTexture != null && webCamTexture.isPlaying && webCamTexture.width > 100)
        {
            try
            {
                var snap = new Texture2D(webCamTexture.width, webCamTexture.height);
                snap.SetPixels(webCamTexture.GetPixels());
                snap.Apply();

                var result = barcodeReader.Decode(snap.GetPixels32(), snap.width, snap.height);
                if (result != null)
                {
                    Debug.Log("Qr Detected: " + result.Text);
                    resultText.text = "Qr: " + result.Text;
                    HandleQrCode(result.Text);
                    webCamTexture.Stop();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("QR decode error: " + e.Message);
            }
        }
    }

    void HandleQrCode(string code)
    {
        if (code == "clue_01")
        {
            Debug.Log("Clue 1 Triggered");
        }
    }
}
