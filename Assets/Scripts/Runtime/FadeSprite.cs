using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeSprite : MonoBehaviour
{
    public float delay = 0f;
    public float startAlpha = 0f;
    public float targetAlpha = 1f;
    public float fadeMagnitude = 0.1f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        this.SetAlpha(this.startAlpha);
        this.StartCoroutine(this.Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(this.delay);

        float alpha = this.spriteRenderer.color.a;
        float increment = (this.targetAlpha >= alpha ? 1f : -1f) * this.fadeMagnitude;

        while (Mathf.Abs(this.targetAlpha - alpha) > Mathf.Epsilon)
        {
            alpha += increment;
            this.SetAlpha(alpha);

            yield return new WaitForSeconds(0.05f);
        }
    }

    private void SetAlpha(float alpha)
    {
        this.spriteRenderer.color = new Color(
            this.spriteRenderer.color.r,
            this.spriteRenderer.color.g,
            this.spriteRenderer.color.b,
            alpha
        );
    }
}
