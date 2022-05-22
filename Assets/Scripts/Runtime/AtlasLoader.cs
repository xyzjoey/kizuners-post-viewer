using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.U2D;

public class AtlasLoader : MonoBehaviour
{
    private void OnEnable()
    {
        SpriteAtlasManager.atlasRequested -= RequestAtlas;
        SpriteAtlasManager.atlasRequested += RequestAtlas;
    }

    private void OnDisable()
    {
        SpriteAtlasManager.atlasRequested -= RequestAtlas;
    }

    private void RequestAtlas(string tag, Action<SpriteAtlas> callback)
    {
        Addressables.LoadAssetAsync<SpriteAtlas>(tag).Completed += (op) =>
        {
            if (op.Result == null)
            {
                Debug.LogWarning("Failed to load addressable [address=" + tag + "]");
            }
            else
            {
                callback(op.Result);
            }
        };
    }

    // private bool IsAddressableExist(object key) {
    //     foreach (var l in Addressables.ResourceLocators) {
    //         IList<IResourceLocation> locs;
    //         if (l.Locate(key, typeof(SpriteAtlas), out locs))
    //             return true;
    //     }
    //     return false;
    // }
}
