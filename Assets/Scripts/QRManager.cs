/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.IO;
using UnityEngine.UI;

public class QRManager : MonoBehaviour
{

    public delegate void QRScanFinished(string str);
    public event QRScanFinished e_QRScanFinished;

    bool decoding = false;
    string dataText = null;
    private Color32[] orginalc;
    private byte[] targetbyte;
    private int W, H, WxH;
    private WebCamTexture webCamera;
    public RawImage tempRaw;
    int z = 0;

    public Texture2D texTemp;
    private BarcodeReader codeReader;
    private static QRManager qRManager;
    public static QRManager Ins
    {
        get
        {
            if (qRManager == null)
            {
                GameObject temp = GameObject.Find("QRManager");
                if (temp == null)
                {
                    temp = new GameObject("QRManager");
                    temp.AddComponent<QRManager>();
                }
                qRManager = temp.GetComponent<QRManager>();
            }

            return qRManager;
        }
    }



    IEnumerator Decode()
    {
        while(true)
        {

            if(decoding&& webCamera!=null)
            {   
                orginalc = webCamera.GetPixels32();
                z = 0;
                // convert the image color data
                for (int y = H - 1; y >= 0; y--)
                {
                    for (int x = 0; x < W; x++)
                    {

                        targetbyte[z++] = (byte)(((int)orginalc[y * W + x].r) << 16 | ((int)orginalc[y * W + x].g) << 8 | ((int)orginalc[y * W + x].b));
                    }
                }

                //RGBLuminanceSource luminancesource = new RGBLuminanceSource(targetbyte, W, H, RGBLuminanceSource.BitmapFormat.Gray8);
                PlanarYUVLuminanceSource planarYUV = new PlanarYUVLuminanceSource(targetbyte, W, H,0,0,W,H,false);
                var bitmap = new BinaryBitmap(new HybridBinarizer(planarYUV));
                Result data;
                var reader = new MultiFormatReader();

                data = reader.decode(bitmap);
                if (data != null)
                {
                    {
                        decoding = false;
                        dataText = data.Text;
                        Debug.Log(dataText+ e_QRScanFinished.ToString());
                        e_QRScanFinished(dataText);
                    }
                }
                else
                {
                    Debug.Log("进入图像解析");
                    yield return new WaitForEndOfFrame();
                    Color[] color = webCamera.GetPixels();
                    float res=0;
                    for(int i = 0; i < color.Length; i++)
                    {
                        res += (color[i].r + color[i].g + color[i].b) / 3; 
                    }
                    res /= color.Length;

                    for (int i = 0; i < color.Length; i++)
                    {
                        if((color[i].r+color[i].g+color[i].b)/3<res)
                        {
                            color[i] = Color.black;
                        }
                        else
                        {
                            color[i] = Color.white;
                        }
                    }

                    texTemp.SetPixels(color);
                    texTemp.Apply();
                    tempRaw.texture = texTemp;
                    data = DecodeByStaticPic(texTemp);
                    if (data != null)
                    {
                        {
                            decoding = false;
                            dataText = data.Text;
                            Debug.Log(dataText);
                            e_QRScanFinished(dataText);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
           
        }
        
    }

    public void StartDecode(WebCamTexture te)
    {
        webCamera = te;
        decoding = true;
        codeReader = new BarcodeReader();
        W = webCamera.width;
        H = webCamera.height;
        WxH = W * H;
        targetbyte = new byte[WxH];
        texTemp = new Texture2D(W, H);
        StartCoroutine(Decode());

    }

    public void ReStartDecode()
    {
        decoding = true;
    }

    public void StopDecode()
    {
        decoding = false;
        StopAllCoroutines();
    }

    public  Result DecodeByStaticPic(Texture2D tex)
    {
        Result data =codeReader.Decode(tex.GetPixels32(), tex.width, tex.height);
        return data;
    }

}
