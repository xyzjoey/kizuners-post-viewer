using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class AssetReferenceInfo
{
    public AssetReference reference = null;
    public Transform parent = null;
    [HideInInspector]
    public bool completed = false;

    public void InstantiateAsync(Action callback)
    {
        var op = Addressables.InstantiateAsync(this.reference, this.parent);

        op.Completed += (op) =>
        {
            this.completed = true;
            callback();
        };
    }
}

public class SceneObjectLoader : MonoBehaviour
{
    public List<AssetReferenceInfo> referenceInfoList;

    private ItemMenu itemMenu;
    private int completedCount = 0;

    private void Awake()
    {
        this.itemMenu = CommonObjects.Get().itemMenu;
    }

    private void Start()
    {
        this.LoadAll();
    }

    private void LoadAll()
    {
        foreach (var referenceInfo in this.referenceInfoList)
        {
            referenceInfo.InstantiateAsync(this.OnSingleLoadCompleted);
        }
    }

    private void OnSingleLoadCompleted()
    {
        if (++this.completedCount >= this.referenceInfoList.Count)
        {
            this.OnAllLoadCompleted();
        }
    }

    private void OnAllLoadCompleted()
    {
        this.itemMenu.ExitState(ItemMenuState.Uninitialized);
    }
}
