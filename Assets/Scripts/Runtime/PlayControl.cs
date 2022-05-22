using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayControl : MonoBehaviour
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

        if (!(this.mouseInfo.pointerType == PointerType.Touch && this.mouseInfo.pointerState == PointerState.ShortDown))
        {
            this.cameraControl.UpdateOnMouse(this.mouseInfo);
        }

        if (this.mouseInfo.pointerState == PointerState.OnShortUp)
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

        bool isPrevPointerDown = MouseInfo.IsPointerStateWhateverDown(this.mouseInfo.prevPointerState);
        bool isPointerDown = MouseInfo.IsPointerStateWhateverDown(this.mouseInfo.pointerState);
        bool isUpDownChanged = isPrevPointerDown != isPointerDown;

        if (this.IsTargetChanged())
        {
            if (isPointerDown)
            {
                this.SetInteractTargetState(this.mouseInfo.target, InteractState.MouseDown);
            }
            else if (this.mouseInfo.pointerType != PointerType.Touch)
            {
                this.SetInteractTargetState(this.mouseInfo.target, InteractState.MouseOver);
            }

            this.SetInteractTargetState(this.mouseInfo.prevTarget, InteractState.None);
        }
        else if (isUpDownChanged)
        {
            if (isPointerDown)
            {
                this.SetInteractTargetState(this.mouseInfo.target, InteractState.MouseDown);
            }
            else if (this.mouseInfo.pointerType != PointerType.Touch)
            {
                this.SetInteractTargetState(this.mouseInfo.target, InteractState.MouseOver);
            }
            else
            {
                this.SetInteractTargetState(this.mouseInfo.target, InteractState.None);
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
        if (item == null)
        {
            return;
        }

        this.FocusItem(item);
        this.itemMenu.ScrollTo(item);
        this.itemMenu.EnterState(ItemMenuState.ScrollingBySceneControl);
    }

    public void FocusItem(Item item)
    {
        if (item == null)
        {
            return;
        }

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
