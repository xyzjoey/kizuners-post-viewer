using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemButton : MonoBehaviour
{
    private Item targetItem = null;
    private RectControl rectControl;
    private SceneControl sceneControl;

    private void Awake()
    {
        this.InitObjectReference();
    }

    private void InitObjectReference()
    {
        this.rectControl = this.GetComponent<RectControl>();
        this.sceneControl = CommonObjects.Get().sceneControl;
    }

    public void SetTargetItem(Item item)
    {
        if (item == this.targetItem || item == null)
        {
            return;
        }

        this.targetItem = item;

        TMP_Text text = this.GetComponentInChildren<TMP_Text>();
        text.text = item.itemName;
        text.font = item.fontAsset;
    }

    public Item GetTargetItem()
    {
        return this.targetItem;
    }

    public void SetRectOnScroll(float scrollPosition, AnimationCurve xCurve, AnimationCurve yCurve, AnimationCurve scaleCurve)
    {
#if UNITY_EDITOR
        this.InitObjectReference();
#endif

        this.rectControl.MoveTo(new Vector2(xCurve.Evaluate(scrollPosition), yCurve.Evaluate(scrollPosition)));
        this.rectControl.ScaleTo(scaleCurve.Evaluate(scrollPosition));
    }
}
