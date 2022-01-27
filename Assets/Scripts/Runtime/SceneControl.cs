using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour
{
    CameraControl cameraControl;
    // ItemMenu itemMenu;
    MouseInfo mouseInfo = new MouseInfo();

    void Start()
    {
        this.cameraControl = GameObject.FindWithTag("CameraControl").GetComponent<CameraControl>();
        // this.itemMenu = GameObject.FindWithTag("ItemMenu").GetComponent<ItemMenu>();
    }

    void Update()
    {
        this.mouseInfo.UpdateInfo();

        this.cameraControl.UpdateOnMouse(this.mouseInfo);

        if (this.mouseInfo.isShortClick)
        {
            // if (this.MouseOnMenu()) ...

            if (this.mouseInfo.MouseOnItem())
            {
                this.FocusItem(this.mouseInfo.target);
            }
            else
            {
                this.UnfocusItem();
            }
        }
    }

    void FocusItem(GameObject gameObject)
    {
        this.cameraControl.Focus(gameObject);
        // this.itemMenu.Open();
    }

    public void UnfocusItem()
    {
        // this.itemMenu.Close();
        this.cameraControl.Unfocus();
    }
}
