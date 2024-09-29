using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class ObscuringItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void FadeOut(float targetAlpha = -1f, float duration = -1f)
    {
        if (targetAlpha < 0f)
            targetAlpha = Settings.targetAlpha;

        if (duration < 0f)
            duration = Settings.fadeOutSeconds;

        if (gameObject == null) return;
        spriteRenderer.DOFade(targetAlpha, duration);
    }

    public void FadeIn(float duration = -1f)
    {
        if (duration < 0f)
            duration = Settings.fadeInSeconds;

        if (gameObject == null) return;
        spriteRenderer.DOFade(1f, duration);
    }
}
