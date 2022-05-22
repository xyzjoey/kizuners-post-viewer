using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullscreenCircleSprite : MonoBehaviour
{
    public float radius;

    private SpriteRenderer spriteRenderer;
    private Camera backgroundCamera;

    private void Awake()
    {
        this.spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        this.backgroundCamera = CommonObjects.Get().backgroundCamera;
    }

    private void Update()
    {
        this.ScaleToFitScreen();
    }

    private void ScaleToFitScreen()
    {
        if (this.radius == 0)
        {
            return;
        }

        float cameraHeight = this.backgroundCamera.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(this.backgroundCamera.aspect * cameraHeight, cameraHeight);

        float targetRadius = Mathf.Sqrt(cameraSize.x *cameraSize.x + cameraSize.y * cameraSize.y);

        float scale = targetRadius / this.radius;

        this.transform.localScale = Vector3.one * scale;
    }
}
