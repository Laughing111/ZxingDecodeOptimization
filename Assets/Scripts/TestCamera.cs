using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCamera : MonoBehaviour {

    private WebCamTexture textureCam;
    public Text textDebug;
    public GameObject btn;
    private void Awake()
    {
         textureCam = CameraManager.Instan().CameraOpen();
         gameObject.GetComponent<RawImage>().texture = textureCam;
         if(textureCam!=null)
         {
            gameObject.GetComponent<RawImage>().rectTransform.sizeDelta = new Vector2(textureCam.width, textureCam.height);
         }
         
    }
    // Use this for initialization
    void Start () {
        QRManager.Ins.e_QRScanFinished += QRScanSuccess;
        QRManager.Ins.StartDecode(textureCam);
	}

    public void QRScanSuccess(string data)
    {
        Debug.Log(data);
        textDebug.text =data; btn.SetActive(true);
    }

    public void ResumeQRScan()
    {
        QRManager.Ins.ReStartDecode();
        textDebug.text = "";
    }
	
	
}
