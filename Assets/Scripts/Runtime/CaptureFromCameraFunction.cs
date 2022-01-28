using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CaptureFromCameraFunction : MonoBehaviour
{
    [SerializeField] private static readonly string CAPUTURED_PICTURE_SAVE_DIRECTORY = "/";

    // こちらのページ参考にしました（ていうかそのまんま）
    // https://www.kemomimi.dev/unity/1161/
    // http://blog.chatlune.jp/2018/02/08/unity-screenshot/

    // キャプチャする画像の縦と横の指定
    [SerializeField] private int width = 1920;
    [SerializeField] private int heght = 1080;

    bool b_capture_flag = false;


    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.C) && !b_capture_flag)
        {
            b_capture_flag = true;
            Capture();
            Debug.Log("画像をキャプチャしました");
        }

        if (!Input.GetKey(KeyCode.C))
        {
            b_capture_flag = false;
        }

    }

    public void Capture()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.Log("camera取得失敗");
        }
        StartCoroutine(CaptureFromCamera(width, heght, cam));
    }

    private IEnumerator CaptureFromCamera(int width, int height, Camera cam)
    {
        yield return new WaitForEndOfFrame();

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cam.targetTexture = renderTexture;

        Debug.Log("画像サイズ : " + width + "*" + height);

        Texture2D texture = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGB24, false);

        texture.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        texture.Apply();

        // 保存する画像のサイズを変えるならResizeTexture()を実行
        texture = ResizeTexture(texture, width, height);

        string datetimeStr = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        string savePath = Application.streamingAssetsPath + CAPUTURED_PICTURE_SAVE_DIRECTORY + datetimeStr + ".png";
        Debug.Log("保存場所：" + savePath);

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(savePath, bytes);

        Destroy(texture);

        if (cam.targetTexture != null)
            cam.targetTexture.Release();

        yield break;
    }

    Texture2D ResizeTexture(Texture2D src, int dst_w, int dst_h)
    {
        Texture2D dst = new Texture2D(dst_w, dst_h, src.format, false);

        float inv_w = 1f / dst_w;
        float inv_h = 1f / dst_h;

        for (int y = 0; y < dst_h; ++y)
        {
            for (int x = 0; x < dst_w; ++x)
            {
                dst.SetPixel(x, y, src.GetPixelBilinear((float)x * inv_w, (float)y * inv_h));
            }
        }
        return dst;
    }
}