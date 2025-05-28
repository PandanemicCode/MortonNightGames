using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class SampleQrScanner : MonoBehaviour
{
    [SerializeField] RawImage camDisplay;
    [SerializeField] Text resultText;

    private WebCamTexture webCamTexture;
    private IBarcodeReader barcodeReader = new BarcodeReader();

    void Start()
    {
        webCamTexture = new WebCamTexture();
        camDisplay.texture = webCamTexture;
        camDisplay.material.mainTexture = webCamTexture;
        webCamTexture.Play();
    }
    void Update()
    {
        if (webCamTexture.isPlaying && webCamTexture.width > 100)
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
            catch
            {
                //ignore errors
            }
        }
    }

    void HandleQrCode(string code)
    {
        if (code == "clue_01")
        {
            // dose something 
            Debug.Log("Cue 1 Triggerd");
        }
    }
}
