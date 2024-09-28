using UnityEngine;
using DG.Tweening;

public class ItemNudge : MonoBehaviour
{
    private bool isAnimating = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAnimating)
        {
            if (gameObject.transform.position.x < collision.transform.position.x)
                RotateAntiClock();
            else
                RotateClock();
        }
    }

    private void RotateAntiClock()
    {
        isAnimating = true;
        Transform target = gameObject.transform.GetChild(0);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(target.DORotate(new Vector3(0, 0, 8), 0.16f).SetEase(Ease.Linear))
                .Append(target.DORotate(new Vector3(0, 0, -10), 0.2f).SetEase(Ease.Linear))
                .Append(target.DORotate(new Vector3(0, 0, 2), 0.04f).SetEase(Ease.Linear))
                .OnComplete(() => isAnimating = false);
    }

    private void RotateClock()
    {
        isAnimating = true;
        Transform target = gameObject.transform.GetChild(0);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(target.DORotate(new Vector3(0, 0, -8), 0.16f).SetEase(Ease.Linear))
                .Append(target.DORotate(new Vector3(0, 0, 10), 0.2f).SetEase(Ease.Linear))
                .Append(target.DORotate(new Vector3(0, 0, -2), 0.04f).SetEase(Ease.Linear))
                .OnComplete(() => isAnimating = false);
    }
}
