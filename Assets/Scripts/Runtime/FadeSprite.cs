using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeSprite : MonoBehaviour
{
    public float delay = 0f;
    public float startBlend = 0f;
    public float targetBlend = 1f;
    public float fadeMagnitude = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Material material;

    private void Awake()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.material = this.spriteRenderer.material;
    }

    private void OnEnable()
    {
        this.SetBlend(this.startBlend);
        this.StartCoroutine(this.Fade());
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(this.delay);

        float blend = this.material.GetFloat("_OverlayOpacity");

        while (Mathf.Abs(this.targetBlend - blend) > Mathf.Epsilon)
        {
            blend = Mathf.MoveTowards(blend, this.targetBlend, this.fadeMagnitude);
            this.SetBlend(blend);

            yield return new WaitForSeconds(0.05f);
        }
    }

    private void SetBlend(float blend)
    {
        this.material.SetFloat("_OverlayOpacity", blend);
    }
}
