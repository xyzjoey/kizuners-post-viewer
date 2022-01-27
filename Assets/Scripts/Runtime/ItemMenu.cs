using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMenu : MonoBehaviour
{
    RectTransform rectTransform;
    bool isOpened = false;

    void Start()
    {
        this.rectTransform = this.GetComponent<RectTransform>();

        this.Close();
    }

    public bool IsOpened()
    {
        return this.isOpened;
    }

    public void Open()
    {
        this.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, this.rectTransform.rect.width);

        this.isOpened = true;
    }

    public void Close()
    {
        this.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -this.rectTransform.rect.width, this.rectTransform.rect.width);

        this.isOpened = false;
    }
}
