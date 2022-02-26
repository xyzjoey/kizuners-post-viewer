using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pivot
{
    Center,
    TopLeft
}

public class RectControl : MonoBehaviour
{
    public Pivot pivot;

    private RectTransform rectTransform;

    void Awake()
    {
        this.rectTransform = this.GetComponent<RectTransform>();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        this.rectTransform = this.GetComponent<RectTransform>();
    }
#endif

    public void MoveTo(Vector2 normalizedPosition)
    {
#if UNITY_EDITOR
        this.rectTransform = this.GetComponent<RectTransform>();
#endif

        var anchorMin = this.rectTransform.anchorMin;
        var anchorMax = this.rectTransform.anchorMax;

        Vector2 size = new Vector2(anchorMax.x - anchorMin.x, anchorMax.y - anchorMin.y);
        Vector2 sizeHalf = size / 2;

        if (this.pivot == Pivot.Center)
        {
            this.rectTransform.anchorMin = normalizedPosition - sizeHalf;
            this.rectTransform.anchorMax = normalizedPosition + sizeHalf;
        }
        else if (this.pivot == Pivot.TopLeft)
        {
            this.rectTransform.anchorMin = normalizedPosition;
            this.rectTransform.anchorMax = normalizedPosition + size;
        }
    }

    public void ScaleTo(float scale)
    {
#if UNITY_EDITOR
        this.rectTransform = this.GetComponent<RectTransform>();
#endif

        this.rectTransform.localScale = Vector3.one * scale;
    }

    public Vector2 GetPosition()
    {
        if (this.pivot == Pivot.Center)
        {
            return (this.rectTransform.anchorMin + this.rectTransform.anchorMax) / 2;
        }
        else
        {
            return this.rectTransform.anchorMin;
        }
    }
}
