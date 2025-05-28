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
    bool MapActive;
    void Start()
    {
        MapActive = true;
        ImgMap.material = ActiveMat;
        ImgQr.material = InactiveMat;
        MapIcon.transform.localScale = new Vector2(1.76f, 1.76f);
        QrIcon.transform.localScale = new Vector2(0.6f, 0.6f);
    }
    ///////////////// Functions for the Btn's///////////////////////////
    public void ChangeStateMap()
    {
        MapActive = true;
        ImgMap.material = ActiveMat;
        ImgQr.material = InactiveMat;
        MapIcon.transform.localScale = new Vector2(1.76f, 1.76f);
        QrIcon.transform.localScale = new Vector2(0.6f, 0.6f);
    }
    public void ChangeStateQr()
    {
        MapActive = false;
        ImgMap.material = InactiveMat;
        ImgQr.material = ActiveMat;
        MapIcon.transform.localScale = new Vector2(1, 1);
        QrIcon.transform.localScale = new Vector2(1, 1);
    }
    

}