using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SceneLoader : MonoBehaviour
{
    public AssetReference itemsReference;

    private ItemMenu itemMenu;

    private void Awake()
    {
        this.itemMenu = CommonObjects.Get().itemMenu;
    }

    private void Start()
    {
        // this.LoadItems();
    }

    private void LoadItems()
    {
    //     this.itemMenu.ResetItemsAndButtons();
    }
}
