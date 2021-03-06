using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CenterZoomLevel
{
    public float unfocus;
    public float focusIllust;
    public float focusMessage;
}

[System.Serializable]
public class LocalZoomLevel
{
    public float farest;
    [HideInInspector]
    public float initial;
    public float closest;
}

public class CameraControl : MonoBehaviour
{
    public float focusDirectionMultiplier;
    public float rotationMultiplier;
    public float moveMultiplier;
    public float zoomMultiplier;
    [Range(0, 1)]
    public float rotateSmooth;
    [Range(0, 1)]
    public float moveCenterSmooth;
    [Range(0, 1)]
    public float moveLocalSmooth;
    [Range(0, 1)]
    public float zoomCenterSmooth;
    [Range(0, 1)]
    public float zoomLocalSmooth;

    // config
    public CenterZoomLevel centerZoomLevel;
    public LocalZoomLevel localZoomLevel;

    // config taken on start
    private Vector3 defaultCenter;
    private Vector3 defaultWorldPosition;

    // runtime value
    private Vector3 baseRotation;
    private Vector3 targetRotation; // euler
    private Vector3 targetCenter;
    private Vector3 targetLocalPosition;
    private float targetCenterZoom;
    private float targetLocalZoom;

    void Start()
    {
        this.defaultCenter = this.transform.position;
        this.defaultWorldPosition = Camera.main.transform.position;
        this.localZoomLevel.initial = Mathf.Abs(Camera.main.transform.localPosition.z);

        this.SetBaseRotation(Vector3.zero);
        this.SetRotation(Vector3.zero);
        this.SetCenter(Vector3.zero);
        this.SetLocalPosition(Vector3.zero);
        this.SetCenterZoom(this.centerZoomLevel.unfocus);
        this.SetLocalZoom(this.localZoomLevel.initial);
    }

    void Update()
    {
        this.Rotate();
        this.Move();
        this.Zoom();
    }

    public void UpdateOnMouse(MouseInfo mouseInfo)
    {
        this.RotateByMouse(mouseInfo);
        this.SetPositionByMouse(mouseInfo);
        this.SetZoomByMouse(mouseInfo);
    }

    public void RotateByMouse(MouseInfo mouseInfo)
    {
        Vector3 rotation = this.baseRotation;
        rotation.x += mouseInfo.normalizedY * rotationMultiplier;
        rotation.y -= mouseInfo.normalizedX * rotationMultiplier;

        this.SetRotation(rotation);
    }

    public void SetPositionByMouse(MouseInfo mouseInfo)
    {
        var pos = Camera.main.transform.localPosition;
        pos.x = mouseInfo.normalizedX * moveMultiplier;
        pos.y = mouseInfo.normalizedY * moveMultiplier;

        this.SetLocalPosition(pos);
    }

    public void SetZoomByMouse(MouseInfo mouseInfo)
    {
        this.SetLocalZoom(this.targetLocalZoom - mouseInfo.scroll * this.zoomMultiplier);
    }

    public void Focus(GameObject gameObject)
    {
        Item item = gameObject.GetComponent<Item>();

        Vector3 forward = gameObject.transform.forward;
        forward = Vector3.Lerp(Vector3.forward, forward, this.focusDirectionMultiplier);

        this.SetBaseRotation(Quaternion.LookRotation(forward, gameObject.transform.up).eulerAngles);
        this.ResetRotation();
        this.SetCenter(gameObject.transform.position);
        this.SetCenterZoom(item.type == ItemType.Message ? this.centerZoomLevel.focusMessage : this.centerZoomLevel.focusIllust);
        this.SetLocalZoom(this.localZoomLevel.initial);
    }

    public void Unfocus()
    {
        this.SetBaseRotation(Vector3.zero);
        this.ResetRotation();
        this.SetCenter(this.defaultCenter);
        this.SetCenterZoom(this.centerZoomLevel.unfocus);
        this.SetLocalZoom(this.localZoomLevel.initial);
    }

    void SetBaseRotation(Vector3 rotation)
    {
        this.baseRotation = rotation;
    }

    void SetRotation(Vector3 rotation)
    {
        this.targetRotation = rotation;
    }

    void ResetRotation()
    {
        this.SetRotation(this.baseRotation);
    }

    void SetCenter(Vector3 center)
    {
        this.targetCenter = center;
    }

    void SetLocalPosition(Vector3 localPosition)
    {
        this.targetLocalPosition = localPosition;
    }

    void SetCenterZoom(float zoomLevel)
    {
        this.targetCenterZoom = zoomLevel;
    }

    void SetLocalZoom(float zoomLevel)
    {
        this.targetLocalZoom = Mathf.Clamp(zoomLevel, this.localZoomLevel.closest, this.localZoomLevel.farest);
    }

    void Rotate()
    {
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(this.targetRotation), MathHelper.GetTimeBasedLerpFactor(this.rotateSmooth));
    }

    void Move()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, this.targetCenter, MathHelper.GetTimeBasedLerpFactor(this.moveCenterSmooth));
        Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, this.targetLocalPosition, MathHelper.GetTimeBasedLerpFactor(this.moveLocalSmooth));
    }

    void Zoom()
    {
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.one * this.targetCenterZoom, MathHelper.GetTimeBasedLerpFactor(this.zoomCenterSmooth));
        
        var localPos = Camera.main.transform.localPosition;
        localPos.z = Mathf.Lerp(localPos.z, -this.targetLocalZoom, MathHelper.GetTimeBasedLerpFactor(this.zoomLocalSmooth));
        Camera.main.transform.localPosition = localPos;
    }
}
