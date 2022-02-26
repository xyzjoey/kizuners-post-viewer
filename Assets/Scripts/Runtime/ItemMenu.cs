using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemMenuState
{
    Closed,
    Opened,
    ScrollingByScrollRect,
    ScrollingBySceneControl
}

public class ScrollRectInfo
{
    public float position = 0f; // [0f, 1f]
    public float velocity = 0f;
    public float positionDelta = 0f;
}

public class ScrollInfo
{
    public float contentInterval;
    public float screenInterval;

    public float contentPosition = 0f; // [0f, 1f] // top to down // position of screenDefaultCenter in all content
    public float screenDefaultCenter = 0.5f;
    public float screenPosition = 0.5f; // [0f, 1f] // down to top // position of selectedItem

    public int selectedItemIndex = 0;
    public int prevSelectedItemIndex = 0;
}

public class ItemMenu : MonoBehaviour
{
    public int visibleItemButtonNum = 13; // expect odd
    public int tailingItemButtonNum = 3;

    public AnimationCurve yCurve = AnimationCurve.Linear(-1, -1, 2, 2);
    public AnimationCurve xCurve = new AnimationCurve(new Keyframe(-1f, 1.5f), new Keyframe(0.5f, 0.5f), new Keyframe(2f, 1.5f));
    public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe(-1f, 0.5f), new Keyframe(0.5f, 1f), new Keyframe(2f, 0.5f));

    public float slowDownVelocity = 100f;
    public float stopVelocity = 5f;
    public float slowDownArea = 0.3f;
    public float scrollSmooth = 0.1f;
    public float transitSmooth = 0.1f;

    private ItemMenuState state = ItemMenuState.Closed;
    private InteractState interactState = InteractState.None;
    private ScrollInfo scrollInfo = new ScrollInfo();
    private ScrollRectInfo scrollRectInfo = new ScrollRectInfo();
    private List<Item> items = new List<Item>();

    private IEnumerator scrollCoroutine = null;
    private IEnumerator transitCoroutine = null;

    private SceneControl sceneControl;
    private GameObject itemButtonContainer;
    private GameObject itemButtonPrefab;
    private ScrollRect scrollRect;
    private RectControl scrollViewRectControl;

    /*-- Unity event -- */

    private void Awake()
    {
        this.sceneControl = CommonObjects.Get().sceneControl;
        this.itemButtonContainer = CommonObjects.Get().itemButtonContainer;
        this.itemButtonPrefab = CommonObjects.Get().itemButtonPrefab;
        this.scrollRect = this.GetComponentInChildren<ScrollRect>();
        this.scrollViewRectControl = this.scrollRect.GetComponent<RectControl>();
    }

    // private void Start()
    // {
    //     this.ResetItemsAndButtons();
    // }

    /*-- Callback -- */

    public void OnScrollRectScrolled(Vector2 normalizedPosition)
    {
        this.UpdateScrollRectInfo();

        this.EnterState(ItemMenuState.ScrollingByScrollRect);

        if (this.state != ItemMenuState.ScrollingByScrollRect)
        {
            return;
        }

        this.Scroll(this.scrollInfo.contentPosition + this.ScreenToContentPosition(this.scrollRectInfo.positionDelta));

        // this.LoopScrollRect(); // FIXME

        if (this.interactState != InteractState.MouseDown)
        {
            float velocityAbs = Mathf.Abs(this.scrollRectInfo.velocity);
            bool withinSlowDownArea = Mathf.Abs(this.scrollInfo.screenPosition - this.scrollInfo.screenDefaultCenter) < this.slowDownArea * this.scrollInfo.screenInterval;
            bool shouldStop = (velocityAbs <= this.stopVelocity) ||
                              (velocityAbs <= this.slowDownVelocity && withinSlowDownArea);

            if (shouldStop)
            {
                this.ExitState(ItemMenuState.ScrollingByScrollRect);
                this.sceneControl.FocusItem(this.items[this.scrollInfo.selectedItemIndex]);
                this.ScrollToNearest();
            }
        }
    }

    /*-- state -- */

    public void SetInteractState(InteractState state)
    {
        if (this.interactState == state)
        {
            return;
        }

        this.interactState = state;
    }

    private void SetState(ItemMenuState state)
    {
        if (this.state == ItemMenuState.ScrollingBySceneControl)
        {
            if (this.scrollCoroutine != null)
            {
                this.StopCoroutine(this.scrollCoroutine);
                this.scrollCoroutine = null;
            }
        }

        if (this.state == ItemMenuState.Closed && state == ItemMenuState.Opened)
        {
            this.Show();
        }
        else if (state == ItemMenuState.Closed)
        {
            this.Hide();
        }

        this.state = state;
    }

    public void EnterState(ItemMenuState state)
    {
        if (this.state == state)
        {
            return;
        }

        if (state == ItemMenuState.ScrollingByScrollRect)
        {
            if (this.interactState == InteractState.MouseDown)
            {
                this.SetState(state);
            }
        }
        else if (state == ItemMenuState.ScrollingBySceneControl)
        {
            if (this.state == ItemMenuState.Closed)
            {
                this.SetState(ItemMenuState.Opened);
            }

            this.SetState(state);
        }
        else
        {
            this.SetState(state);
        }
    }

    public void ExitState(ItemMenuState state)
    {
        if (this.state != state)
        {
            return;
        }

        switch (state)
        {
        case ItemMenuState.Closed:
            this.SetState(ItemMenuState.Opened);
            break;
        case ItemMenuState.Opened:
            this.SetState(ItemMenuState.Closed);
            break;
        default:
            this.SetState(ItemMenuState.Opened);
            break;
        }
    }

    public bool IsState(ItemMenuState state)
    {
        return this.state == state;
    }

    /*-- reset -- */

    public void ResetItemsAndButtons()
    {
        this.RetrieveItems();
        this.ResetScrollInfo();
        this.UpdateItemButtonsOnScroll();
    }

    public void ResetScrollInfo()
    {
        this.scrollInfo = new ScrollInfo();
        this.scrollInfo.contentInterval = 1f / this.items.Count;
        this.scrollInfo.screenInterval = 1f / this.visibleItemButtonNum;
    }

    private void RetrieveItems()
    {
        this.items.Clear();

        foreach (var itemObject in GameObject.FindGameObjectsWithTag(Tag.Item))
        {
            Item item = itemObject.GetComponent<Item>();

            if (item.type != ItemType.MainIllust)
            {
#if UNITY_EDITOR
                if (item.itemName == "")
                {
                    Debug.LogWarning(item.name + ": Item name is empty");
                }
#endif

                this.items.Add(item);
            }
        }
    }

    /*-- helper -- */

    private void StartCoroutine(ref IEnumerator currCoroutine, IEnumerator newCoroutine)
    {
        if (currCoroutine != null)
        {
            this.StopCoroutine(currCoroutine);
        }

        currCoroutine = newCoroutine;
        this.StartCoroutine(currCoroutine);
    }

    private int GetItemButtonNum()
    {
        return this.visibleItemButtonNum + 2 * this.tailingItemButtonNum;
    }

    private ItemButton GetItemButton(int index)
    {
        return this.itemButtonContainer.transform.GetChild(index).GetComponent<ItemButton>();
    }

    private int LoopItemIndex(int index)
    {
        int modIndex = index % this.items.Count;

        if (modIndex < 0)
        {
            modIndex += this.items.Count;
        }

        return modIndex;
    }

    private float LoopValue(float value, float max)
    {
        float result = value - max * (int)(value / max);

        if (result < 0)
        {
            result += max;
        }

        return result;
    }

    /*-- transition -- */

    private void Show()
    {
        this.StartCoroutine(ref this.transitCoroutine, this.TransitScrollViewTo(new Vector2(0, 0)));
    }

    private void Hide()
    {
        this.StartCoroutine(ref this.transitCoroutine, this.TransitScrollViewTo(new Vector2(1, 0)));
    }

    private IEnumerator TransitScrollViewTo(Vector2 position)
    {
        Vector2 currPosition = this.scrollViewRectControl.GetPosition();

        while ((position - currPosition).magnitude > Mathf.Epsilon)
        {
            currPosition = MathHelper.TimeBasedLerp(currPosition, position, this.transitSmooth);
            this.scrollViewRectControl.MoveTo(currPosition);

            yield return null;
        }
    }

    /*-- scroll -- */

    // void LoopScrollRect()
    // {
    //     // ScrollRect scrollRect = this.GetComponentInChildren<ScrollRect>();

    //     // float y = scrollRect.normalizedPosition.y;

    //     // if (y >= 0 && y <= 1)
    //     // {
    //     //     return;
    //     // }

    //     // y -= Mathf.Floor(y);

    //     // Vector2 velocity = scrollRect.velocity;
    //     // scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, y);
    //     // scrollRect.velocity = velocity;
    // }

    private void UpdateScrollRectInfo()
    {
        float newPosition = this.scrollRect.normalizedPosition.y;
        this.scrollRectInfo.positionDelta = newPosition - this.scrollRectInfo.position;

        this.scrollRectInfo.position = newPosition;
        this.scrollRectInfo.velocity = this.scrollRect.velocity.y;
    }

    private float ContentToScreenPosition(float contentPosition)
    {
        return - contentPosition * this.scrollInfo.screenInterval / this.scrollInfo.contentInterval;
    }

    private float ScreenToContentPosition(float screenPosition)
    {
        return - screenPosition * this.scrollInfo.contentInterval / this.scrollInfo.screenInterval;
    }

    public void ScrollTo(Item item)
    {
        int itemIndex = this.items.FindIndex(x => (x == item));
        this.ScrollTo(itemIndex);
    }

    private void ScrollTo(int itemIndex)
    {
        if (itemIndex < 0)
        {
            return;
        }

        int shortestIndexDelta = int.MaxValue;

        for (int shift = -1; shift <= 1; ++shift)
        {
            int indexDelta = (itemIndex + shift * this.items.Count) - this.scrollInfo.selectedItemIndex;
            
            if (Mathf.Abs(indexDelta) < Mathf.Abs(shortestIndexDelta))
            {
                shortestIndexDelta = indexDelta;
            }
        }

        float targetContentPosition = (this.scrollInfo.selectedItemIndex + shortestIndexDelta) * this.scrollInfo.contentInterval;

        this.StartCoroutine(ref this.scrollCoroutine, this.ScrollToContentPosition(targetContentPosition));
    }

    private void ScrollToNearest()
    {
        float targetContentPosition = this.scrollInfo.selectedItemIndex * this.scrollInfo.contentInterval;

        this.StartCoroutine(ref this.scrollCoroutine, this.ScrollToContentPosition(targetContentPosition));
    }

    private IEnumerator ScrollToContentPosition(float targetContentPosition)
    {
        this.EnterState(ItemMenuState.ScrollingBySceneControl);

        float totalContentPositionDelta = MathHelper.ShortestLoopDelta(this.scrollInfo.contentPosition, targetContentPosition, 1f);

        while(Mathf.Abs(totalContentPositionDelta) > 0.0000001f)
        {
            float nextContentPositionDelta = MathHelper.TimeBasedLerp(0, totalContentPositionDelta, this.scrollSmooth);
            this.Scroll(this.scrollInfo.contentPosition + nextContentPositionDelta);

            totalContentPositionDelta -= nextContentPositionDelta;

            yield return null;
        }
    }

    // core scroll method
    void Scroll(float newContentPosition)
    {
        this.UpdateScrollInfo(newContentPosition);
        this.UpdateItemButtonsOnScroll();
    }

    void UpdateScrollInfo(float newContentPosition)
    {
        int nearestItemIndex = (int) Mathf.Round(newContentPosition / this.scrollInfo.contentInterval);
        float nearestItemContentPosition = nearestItemIndex * this.scrollInfo.contentInterval;

        this.scrollInfo.contentPosition = this.LoopValue(newContentPosition, 1f);
        this.scrollInfo.screenPosition = this.ContentToScreenPosition(newContentPosition - nearestItemContentPosition) + this.scrollInfo.screenDefaultCenter;

        this.scrollInfo.prevSelectedItemIndex = this.scrollInfo.selectedItemIndex;
        this.scrollInfo.selectedItemIndex = this.LoopItemIndex(nearestItemIndex);
    }

    void UpdateItemButtonsOnScroll()
    {
        int itemButtonNum = this.GetItemButtonNum();
        int middleIndex = itemButtonNum / 2;

        for (int i = 0; i < itemButtonNum; ++i)
        {
            ItemButton itemButton = this.GetItemButton(i);
            int itemIndex = this.LoopItemIndex(this.scrollInfo.selectedItemIndex + i - middleIndex);
            float position = this.scrollInfo.screenPosition + (i - middleIndex) * this.scrollInfo.screenInterval;

            itemButton.SetTargetItem(this.items[itemIndex]);
            this.SetItemButtonPosition(itemButton, position);
        }
    }

    void SetItemButtonPosition(ItemButton itemButton, float position)
    {
        itemButton.SetRectOnScroll(position, this.xCurve, this.yCurve, this.scaleCurve);
    }

    /*-- Editor -- */

    public void ClearItemButtons()
    {
        while (this.itemButtonContainer.transform.childCount != 0)
        {
            GameObject.DestroyImmediate(this.itemButtonContainer.transform.GetChild(0).gameObject);
        }
    }

    private void CreateItemButtons()
    {
        for (int i = 0; i < this.GetItemButtonNum(); ++i)
        {
            Object.Instantiate(this.itemButtonPrefab, this.itemButtonContainer.transform);
        }

        this.ResetItemsAndButtons();
    }

    public void RecreateItemButtons()
    {
        this.ClearItemButtons();
        this.CreateItemButtons();
    }
}
