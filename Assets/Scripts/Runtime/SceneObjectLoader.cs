using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SceneObjectLoader : MonoBehaviour
{
    public AssetReference mainIllustReference;
    public AssetReference messagesReference;
    public AssetReference miniIllustReference;
    // public AssetReference itemMenuReference;

    // // private ItemMenu itemMenu;

    // // private void Awake()
    // // {
    // //     this.itemMenu = CommonObjects.Get().itemMenu;
    // // }

    private void Start()
    {
        this.LoadAll();
        // StartCoroutine(this.LoadItemsDelayed());
    }

    // // // for debug
    // // private IEnumerator LoadItemsDelayed()
    // // {
    // //     yield return new WaitForSeconds(3);
    // //     this.LoadItems();
    // // }

    private void LoadAll()
    {
        Addressables.InstantiateAsync(this.mainIllustReference);
        Addressables.InstantiateAsync(this.messagesReference, CommonObjects.Get().itemGroup.transform);
        Addressables.InstantiateAsync(this.miniIllustReference, CommonObjects.Get().itemGroup.transform);
    }
}
