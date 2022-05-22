using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCanvasGroup : MonoBehaviour
{
    public float minDelayOnLoad = 0f;
    public float startAlpha = 0f;
    public float targetAlpha = 1f;
    public float fadeMagnitude = 0.1f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        this.canvasGroup = this.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        float loadTime = Time.time;
        float delay = Mathf.Clamp(this.minDelayOnLoad - loadTime, 0f, this.minDelayOnLoad);

        this.SetAlpha(this.startAlpha);
        this.StartCoroutine(this.Fade(delay));
    }

    IEnumerator Fade(float delay)
    {
        yield return new WaitForSeconds(delay);

        float alpha = this.canvasGroup.alpha;
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
        this.canvasGroup.alpha = alpha;
    }
}
