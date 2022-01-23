using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCamera : MonoBehaviour
{
    public float rotationMultiplier = 1f;

    void Update()
    {
        float mouseX = Input.mousePosition.x / Screen.width - 0.5f;
        float mouseY = Input.mousePosition.y / Screen.height - 0.5f;

        this.transform.rotation = Quaternion.Euler(mouseY * rotationMultiplier, -mouseX * rotationMultiplier, 0);
    }
}
