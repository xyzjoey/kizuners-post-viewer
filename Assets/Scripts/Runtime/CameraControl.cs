using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZoomLevel
{
    Default = -43,
    Focus = -5
}

public class CameraControl : MonoBehaviour
{
    public float cameraRotationFactor;

    Vector3 defaultPosition;

    void Start()
    {
        this.defaultPosition = this.transform.position;
    }

    public void UpdateOnMouse(MouseInfo mouseInfo)
    {
        this.RotateByMouse(mouseInfo);
    }

    public void RotateByMouse(MouseInfo mouseInfo)
    {
        this.transform.rotation = Quaternion.Euler(mouseInfo.normalizedY * cameraRotationFactor, -mouseInfo.normalizedX * cameraRotationFactor, 0);
    }

    public void Focus(GameObject gameObject)
    {
        this.transform.position = gameObject.transform.position;
        this.Zoom(ZoomLevel.Focus);
    }

    public void Unfocus()
    {
        if (this.transform.position == this.defaultPosition)
        {
            return;
        }

        this.transform.position = this.defaultPosition;
        this.Zoom(ZoomLevel.Default);
    }

    void Zoom(ZoomLevel zoomLevel)
    {
        Camera.main.transform.localPosition = Vector3.forward * (float)zoomLevel;
    }
}
