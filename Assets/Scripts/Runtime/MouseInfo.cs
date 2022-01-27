using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInfo
{
    // config
    public float mouseDownStartTime = -1f;
    public float shortClickDuration = 0.25f;

    // useful member
    public float normalizedX = 0f;
    public float normalizedY = 0f;
    public bool isShortClick = false;
    public GameObject target = null;

    public void UpdateInfo()
    {
        // position
        float mouseX = Input.mousePosition.x / Screen.width - 0.5f;
        float mouseY = Input.mousePosition.y / Screen.height - 0.5f;
        this.normalizedX = mouseX;
        this.normalizedY = mouseY;

        // click
        this.isShortClick = this.CheckShortClick();

        // click target
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            this.target = hit.collider.gameObject;
        }
        else
        {
            this.target = null;
        }
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

    public bool MouseOnItem()
    {
        return this.target != null && this.target.tag == "Item";
    }
}