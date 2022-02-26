using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MouseState
{
    Down,
    Up
}

public enum InteractState
{
    None,
    MouseOver,
    MouseDown
}

public class MouseInfo
{
    // config
    public float mouseDownStartTime = -1f;
    public float shortClickDuration = 0.25f;

    // info
    public float normalizedX = 0f;
    public float normalizedY = 0f;
    public bool isShortClick = false;
    public MouseState prevState = MouseState.Up;
    public MouseState state = MouseState.Up;
    public GameObject prevTarget = null;
    public GameObject target = null;
    public float scroll = 0f;

    public void UpdateInfo()
    {
        // position
        float mouseX = Input.mousePosition.x / Screen.width - 0.5f;
        float mouseY = Input.mousePosition.y / Screen.height - 0.5f;
        this.normalizedX = Mathf.Clamp(mouseX, -0.5f, 0.5f);
        this.normalizedY = Mathf.Clamp(mouseY, -0.5f, 0.5f);

        // state
        this.prevState = this.state;

        if (Input.GetMouseButtonDown(0))
        {
            this.state = MouseState.Down;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            this.state = MouseState.Up;
        }

        // click
        this.isShortClick = this.CheckShortClick();

        // target reset
        this.prevTarget = this.target;
        this.target = null;

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
                    this.target = result.gameObject;
                    break;
                }
            }
        }

        // scene target
        if (this.target == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            
            if (hit.collider != null)
            {
                this.target = hit.collider.gameObject;
            }
        }

        // scroll
        this.scroll = Input.GetAxis("Mouse ScrollWheel");
    }

    public bool CheckShortClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.mouseDownStartTime = Time.time;
        }

        if (this.mouseDownStartTime >= 0f && Input.GetMouseButtonUp(0))
        {
            float clickDuration = Time.time - this.mouseDownStartTime;

            this.mouseDownStartTime = -1f;

            if (clickDuration <= this.shortClickDuration)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsStateChanged()
    {
        return this.state != this.prevState;
    }
}