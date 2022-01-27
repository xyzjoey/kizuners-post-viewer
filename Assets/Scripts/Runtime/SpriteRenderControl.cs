using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpriteRenderControl : MonoBehaviour
{
    public string imageUrl = "";

    void Start()
    {
        this.LoadImage();
    }

    public void LoadImage()
    {
        if (this.imageUrl == "")
        {
            Debug.LogWarning(this.name + ": Image url is empty.");
            return;
        }

        StartCoroutine(this.LoadImageCoroutine());
    }

    IEnumerator LoadImageCoroutine()
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log(this.name + ": Loading image from " + this.imageUrl);
        }
        else
        {
            Debug.Log(this.name + ": Loading image");
        }

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(this.imageUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(this.name + ": " + www.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            this.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        }
    }
}
