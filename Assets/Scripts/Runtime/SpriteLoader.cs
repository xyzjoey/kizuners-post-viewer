using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.Networking;

public class SpriteLoader : MonoBehaviour
{
    public AssetReference spriteReference;
    // public bool correctAlphaOnLoad = true;

    private SpriteRenderer spriteRenderer;

    /*-- Unity event -- */

    private void Awake()
    {
        this.InitObjectReference();
    }

    private void Start()
    {
        this.LoadSpriteReference();
    }

#if UNITY_EDITOR
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        LoadAllSpriteReference();
    }
#endif

    /*-- Common -- */

    private void InitObjectReference()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    public void LoadSpriteReference()
    {
        if (this.spriteReference == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.IsPlaying(this.gameObject)) // edit mode
        {
            this.LoadSpriteReferenceInEditor();
            return;
        }
#endif

        this.LoadSpriteReferenceInRuntime();
    }

    private void LoadSpriteReferenceInRuntime()
    {
        Debug.Log("Loading Asset " + this.spriteReference.GetType());
        // Debug.Log("Loading Asset " + this.spriteReference.GetType() + " " + (this.spriteReference.RuntimeKey).GetType());

        var asyncOperation = Addressables.LoadAssetAsync<Sprite>(this.spriteReference);
        
        asyncOperation.Completed += (operation) =>
        {
            this.spriteRenderer.sprite = operation.Result;

            Addressables.Release(operation);

            Debug.Log("Load Asset Complete " + this.spriteRenderer.sprite);
        };
    }

#if UNITY_EDITOR
    private void LoadSpriteReferenceInEditor()
    {
        this.InitObjectReference();

        var asyncOperation = Addressables.LoadResourceLocationsAsync(this.spriteReference);

        asyncOperation.Completed += (operation) =>
        {
            var locations = operation.Result;

            if (locations.Count > 0)
            {
                this.spriteRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(locations[0].InternalId);
            }

            Addressables.Release(operation);
        };
    }
#endif

    /*-- Editor -- */

#if UNITY_EDITOR
    public static void LoadAllSpriteReference()
    {
        var spriteLoaders = Object.FindObjectsOfType<SpriteLoader>();

        foreach (var spriteLoader in spriteLoaders)
        {
            spriteLoader.LoadSpriteReference();
        }
    }

    public static void UnloadAllSprites()
    {
        var spriteLoaders = Object.FindObjectsOfType<SpriteLoader>();

        foreach (var spriteLoader in spriteLoaders)
        {
            spriteLoader.spriteRenderer.sprite = null;
        }
    }

    public IEnumerator LoadSpriteFromUrl(string url)
    {
        Debug.Log(this.name + ": Loading image from " + url);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(this.name + ": " + www.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            // unused
            // if (this.correctAlphaOnLoad)
            // {
            //     this.CorrectTextureAlpha(texture);
            // }

            this.GetComponent<SpriteRenderer>().sprite = this.SaveTexture(texture);

            Debug.Log(this.name + ": Image loaded");
        }
    }

    public Sprite SaveTexture(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Images/Generated/" + this.name + ".png");

        Debug.Log(this.name + ": writing texture to " + path);

        File.WriteAllBytes(path, bytes);

        AssetDatabase.Refresh();

        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        EditorUtility.SetDirty(textureImporter);
        textureImporter.SaveAndReimport();

        Debug.Log(this.name + ": successfully write texture");

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    // void CorrectTextureAlpha(Texture2D texture)
    // {
    //     float gamma = 2.2f;

    //     for (int y = 0; y < texture.height; ++y)
    //     {
    //         for (int x = 0; x < texture.width; ++x)
    //         {
    //             var color = texture.GetPixel(x, y);

    //             var newColor = color / Mathf.Pow(color.a, 1 / gamma);
    //             newColor.r = Mathf.Clamp(newColor.r, 0f, 1f);
    //             newColor.g = Mathf.Clamp(newColor.g, 0f, 1f);
    //             newColor.b = Mathf.Clamp(newColor.b, 0f, 1f);
    //             newColor.a = Mathf.Pow(color.a, gamma);

    //             texture.SetPixel(x, y, newColor);
    //         }
    //     }

    //     texture.Apply();
    // }
#endif
}
