using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour
{
    private CameraControl cameraControl;
    private ItemMenu itemMenu;

    private MouseInfo mouseInfo = new MouseInfo();
    private Item focusedItem = null;

    void Awake()
    {
        this.cameraControl = CommonObjects.Get().cameraControl;
        this.itemMenu = CommonObjects.Get().itemMenu;
    }

    void Update()
    {
        this.mouseInfo.UpdateInfo();

        this.cameraControl.UpdateOnMouse(this.mouseInfo);

        if (this.mouseInfo.isShortClick)
        {
            if (this.mouseInfo.target == null)
            {
                this.UnfocusItem();
            }
            else if (this.mouseInfo.target.layer == (int)Layer.UI)
            {
                if (this.mouseInfo.target.tag == Tag.ItemButton)
                {
                    this.OnItemClick(this.mouseInfo.target.GetComponent<ItemButton>().GetTargetItem());
                }
            }
            else if (this.mouseInfo.target.tag == Tag.Item)
            {
                this.OnItemClick(this.mouseInfo.target.GetComponent<Item>());
            }
            else
            {
                this.UnfocusItem();
            }
        }

        if (this.IsTargetChanged())
        {
            if (this.mouseInfo.state == MouseState.Down)
            {
                this.SetInteractTargetState(this.mouseInfo.target, InteractState.MouseDown);
            }
            else
            {
                this.SetInteractTargetState(this.mouseInfo.target, InteractState.MouseOver);
            }

            this.SetInteractTargetState(this.mouseInfo.prevTarget, InteractState.None);
        }
        else if (this.mouseInfo.IsStateChanged())
        {
            if (this.mouseInfo.state == MouseState.Down)
            {
                this.SetInteractTargetState(this.mouseInfo.target, InteractState.MouseDown);
            }
            else
            {
                this.SetInteractTargetState(this.mouseInfo.target, InteractState.MouseOver);
            }
        }
    }

    private bool IsTargetChanged()
    {
        if (this.mouseInfo.prevTarget == this.mouseInfo.target)
        {
            return false;
        }

        if (this.mouseInfo.prevTarget == null || this.mouseInfo.target == null)
        {
            return true;
        }

        if (this.mouseInfo.prevTarget.layer == (int)Layer.UI && this.mouseInfo.target.layer == (int)Layer.UI)
        {
            return false;
        }

        return true;
    }

    private void SetInteractTargetState(GameObject target, InteractState state)
    {
        if (target == null)
        {
            return;
        }

        if (target.layer == (int)Layer.UI)
        {
            this.itemMenu.SetInteractState(state);
        }
        else if (target.tag == Tag.Item)
        {
            target.GetComponent<Item>().SetInteractState(state);
        }
    }

    public void OnItemClick(Item item)
    {
        this.FocusItem(item);
        this.itemMenu.ScrollTo(item);
        this.itemMenu.EnterState(ItemMenuState.ScrollingBySceneControl);
    }

    public void FocusItem(Item item)
    {
        if (this.focusedItem != null)
        {
            this.focusedItem.SetFocus(false);
        }

        this.cameraControl.Focus(item.gameObject);
        item.SetFocus(true);

        this.focusedItem = item;
    }

    public void UnfocusItem()
    {
        this.itemMenu.EnterState(ItemMenuState.Closed);

        this.cameraControl.Unfocus();

        if (this.focusedItem != null)
        {
            this.focusedItem.SetFocus(false);
            this.focusedItem = null;
        }
    }
}
