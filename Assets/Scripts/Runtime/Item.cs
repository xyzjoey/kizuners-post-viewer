using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ItemType
{
    MainIllust, // TODO remove MainIllust
    Message,
    MiniIllust
}

public enum ItemState
{
    Default,
    Focused
    // TODO Read
}

public class Item : MonoBehaviour
{
    public ItemType type;
    public string itemName = "";
    public TMP_FontAsset fontAsset;
    public string imageUrl = "";
    public float floatingSpeed = 5f;
    public float floatingAmplitude = 1f;

    private ItemState state = ItemState.Default;
    private InteractState interactState = InteractState.None;
    private Vector3 defaultPosition;
    private bool enableFloating = false;
    private float floatingShift = 0f;
    private GameObject glowLight = null;

    /*-- Unity event -- */

    private void Awake()
    {
        this.enableFloating = (this.type != ItemType.MainIllust);
        this.floatingShift = Random.Range(0f, 5f);
        this.defaultPosition = this.transform.position;

        if (this.type != ItemType.MainIllust)
        {
            this.glowLight = this.transform.GetChild(0).gameObject;
        }
    }

    private void Update()
    {
        if (this.enableFloating && !this.IsState(ItemState.Focused))
        {
            this.FloatUpAndDown();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetNameFromItemName();
    }
#endif

    /*-- Runtime -- */

    private bool IsState(ItemState state)
    {
        return this.state == state;
    }

    private void SetState(ItemState state)
    {
        if (this.state == state)
        {
            return;
        }

        this.state = state;
    }

    public void SetInteractState(InteractState state)
    {
        if (state == InteractState.MouseOver && this.state != ItemState.Focused)
        {
            this.SetGlow(true);
        }
        else
        {
            this.SetGlow(false);
        }

        this.interactState = state;
    }

    private void FloatUpAndDown()
    {
        float y = this.floatingAmplitude * Mathf.Sin(this.floatingSpeed * (this.floatingShift + Time.time));
        this.transform.position = this.defaultPosition + new Vector3(0, y, 0);
    }

    public void SetFocus(bool focus)
    {
        if (focus)
        {
            this.SetState(ItemState.Focused);
        }
        else
        {
            this.SetState(ItemState.Default);
        }

        this.SetGlow(false);
    }

    private void SetGlow(bool value)
    {
        if (this.glowLight != null)
        {
            this.glowLight.SetActive(value);
        }
    }

    /*-- Editor -- */
#if UNITY_EDITOR
    private void SetNameFromItemName()
    {
        if (this.itemName != "")
        {
            this.name = this.type.ToString() + "_" + this.itemName;
        }
    }

    // public string GetItemNameFromName()
    // {
    //     int seperatorPos = this.name.IndexOf('_');

    //     if (seperatorPos >= 0)
    //     {
    //         return this.name.Substring(seperatorPos + 1);
    //     }
    //     else
    //     {
    //         Debug.LogWarning(this.name + ": Failed to get item name");
            
    //         return this.itemName;
    //     }
    // }

    // public void SetItemNameFromName()
    // {
    //     this.itemName = this.GetItemNameFromName();
    // }


    // public void SetAllMissingItemNameFromName()
    // {
    //     var objects = FindObjectsOfType<Item>();

    //     foreach (var o in objects)
    //     {
    //         var item = o.GetComponent<Item>();

    //         if (item.itemName == "")
    //         {
    //             item.SetItemNameFromName();
    //         }
    //     }
    // }
#endif
}
