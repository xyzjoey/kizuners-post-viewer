using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PointerState
{
    Up,
    OnShortUp,
    ShortDown,
    LongDown,
    Pinch
}

public enum PointerType
{
    Mouse,
    Touch
}

public enum InteractState
{
    None,
    MouseOver,
    MouseDown
}

public class TouchInfo
{
    public int touchCount = 0;
    public float pinchDistanceDelta = 0f;
}

public class MouseInfo
{
    // config
    public float mouseDownStartTime = -1f;
    public float shortClickDuration = 0.25f;
    public float pinchMultipler = 0.06f;

    // runtim info
    public float normalizedX = 0f;
    public float normalizedY = 0f;
    public PointerType pointerType = PointerType.Mouse;
    public PointerState prevPointerState = PointerState.Up;
    public PointerState pointerState = PointerState.Up;
    public GameObject prevTarget = null;
    public GameObject target = null;
    public float scroll = 0f;
    public TouchInfo touchInfo = new TouchInfo();

    public void UpdateInfo()
    {
        // position
        float mouseX = Input.mousePosition.x / Screen.width - 0.5f;
        float mouseY = Input.mousePosition.y / Screen.height - 0.5f;
        this.normalizedX = Mathf.Clamp(mouseX, -0.5f, 0.5f);
        this.normalizedY = Mathf.Clamp(mouseY, -0.5f, 0.5f);

        // state
        this.prevPointerState = this.pointerState;
        this.pointerState = this.CheckState(this.pointerState, this.touchInfo);

        // mouse or touch
        if (this.pointerState == PointerState.ShortDown)
        {
            this.pointerType = Input.touchCount > 0 ? PointerType.Touch : PointerType.Mouse;
        }

        // touch
        this.UpdateTouchInfo(this.prevPointerState, this.pointerState);

        // target
        this.prevTarget = this.target;
        this.target = this.CheckTarget();

        // scroll
        if (this.pointerState == PointerState.Pinch)
        {
            this.scroll = Mathf.Clamp(-this.touchInfo.pinchDistanceDelta, -1f, 1f) * this.pinchMultipler;
        }
        else
        {
            this.scroll = Input.GetAxis("Mouse ScrollWheel");
        }
    }

    private PointerState CheckState(PointerState prevPointerState, TouchInfo prevTouchInfo)
    {
        if (Input.touchCount == 2)
        {
            return PointerState.Pinch;
        }

        bool isPrevAnyKindOfUp = prevPointerState <= PointerState.OnShortUp;
        bool isOnDown = Input.GetMouseButtonDown(0);
        bool isOnUp = Input.GetMouseButtonUp(0) || (prevTouchInfo.touchCount != 0 && Input.touchCount == 0);
        
        if (isPrevAnyKindOfUp)
        {
            if (isOnDown)
            {
                this.mouseDownStartTime = Time.time;
                return PointerState.ShortDown;
            }
            else
            {
                return PointerState.Up;
            }
        }
        else
        {
            bool isShort = (Time.time - this.mouseDownStartTime) <= this.shortClickDuration;

            if (isOnUp)
            {
                return isShort ? PointerState.OnShortUp : PointerState.Up;
            }
            else
            {
                return isShort ? PointerState.ShortDown : PointerState.LongDown;
            }
        }
    }

    private void UpdateTouchInfo(PointerState prevPointerState, PointerState currPointerState)
    {
        bool isPrevPinched = prevPointerState == PointerState.Pinch;
        bool isCurrPinched = currPointerState == PointerState.Pinch;

        if (isPrevPinched && isCurrPinched)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            float prevDistance = Vector2.Distance(touch0.position - touch0.deltaPosition, touch1.position - touch1.deltaPosition);
            float currDistance = Vector2.Distance(touch0.position, touch1.position);

            this.touchInfo.pinchDistanceDelta = currDistance - prevDistance;
        }
        else
        {
            this.touchInfo.pinchDistanceDelta = 0f;
        }

        this.touchInfo.touchCount = Input.touchCount;
    }

    private GameObject CheckTarget()
    {
        // ui target
        if (EventSystem.current != null)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.layer != (int)Layer.IgnoreRaycast)
                {
                    return result.gameObject;
                }
            }
        }

        // scene target
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    static public bool IsPointerStateWhateverUp(PointerState pointerState)
    {
        return pointerState <= PointerState.ShortDown;
    }

    static public bool IsPointerStateWhateverDown(PointerState pointerState)
    {
        return pointerState == PointerState.ShortDown || pointerState == PointerState.LongDown;
    }
}