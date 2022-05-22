using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

[Serializable]
public class ReferenceInfo
{
    public AssetReference reference;
    [HideInInspector]
    public AsyncOperationHandle operationHandle;

    public bool IsEmpty()
    {
        return !this.reference.RuntimeKeyIsValid();
    }

    public void AddCallback(Action<AsyncOperationHandle> callback)
    {
        if (this.IsEmpty())
        {
            return;
        }

        if (this.operationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            callback(this.operationHandle);
        }
        else
        {
            this.operationHandle.Completed += callback;
        }
    }
}

[Serializable]
public class SceneReferenceInfo : ReferenceInfo
{
    public void Load()
    {
        if (!this.IsEmpty())
        {
            this.operationHandle = Addressables.LoadSceneAsync(this.reference);
        }
    }
}

[Serializable]
public class GameObjectReferenceInfo : ReferenceInfo
{
    public void Load()
    {
        if (!this.IsEmpty())
        {
            this.operationHandle = Addressables.LoadAssetAsync<GameObject>(this.reference);
        }
    }

    public void Instantiate(Transform parent = null)
    {
        if (this.operationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            UnityEngine.Object.Instantiate((GameObject)this.operationHandle.Result, parent);
        }
    }
}

[Serializable]
public class ReferenceInfos
{
    public SceneReferenceInfo scene;
    public GameObjectReferenceInfo mainIllust;
    public List<GameObjectReferenceInfo> items;
}

public class AssetReferenceLoader : MonoBehaviour
{
    public ReferenceInfos referenceInfos;

    private bool isLoaded = false;

    private void Start()
    {
        this.Load();
    }

    private void Load()
    {
        if (!this.isLoaded)
        {
            AssetReferenceLoader.Load(this.referenceInfos);
            this.isLoaded = true;
        }
    }

    static private void Load(ReferenceInfos referenceInfos)
    {
        referenceInfos.scene.Load();
        referenceInfos.mainIllust.Load();

        referenceInfos.mainIllust.AddCallback((mainIllustOp) =>
        {
            referenceInfos.scene.AddCallback((sceneOp) =>
            {
                referenceInfos.mainIllust.Instantiate();
            });

            foreach (var itemReference in referenceInfos.items)
            {
                itemReference.Load();
                itemReference.AddCallback((itemOp) =>
                {
                    referenceInfos.scene.AddCallback((sceneOp) =>
                    {
                        itemReference.Instantiate(CommonObjects.Get().itemGroup.transform);
                    });
                });
            }
        });
    }
}
