using DG.Tweening;
using UnityEngine;

public class PulsatingVisual : BaseVisual
{
    [SerializeField] private float pulsingTime = 1f;
    [SerializeField] private float fadeInTime = 0.25f;
    [SerializeField] private float fadeOutTime = 0.25f;
    [SerializeField] private float lowerFadeAmount = 0.45f;
    [SerializeField] private float upperFadeAmount = 0.85f;

    Tween fadeTween;

    private void Awake()
    {
        selectedVisual = selectedVisual != null ? selectedVisual : transform.Find("Visual").gameObject;
    }

    public override void OnSelect()
    {
        fadeTween.Kill();
        selectedVisual.SetActive(true);

        fadeTween = DOTween.To(() => currentOpacity,
            x => SetAllMaterialsAlpha(x),
            upperFadeAmount, fadeInTime)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                fadeTween = DOTween.To(() => upperFadeAmount,
                    x => SetAllMaterialsAlpha(x),
                    lowerFadeAmount,
                    pulsingTime)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });
    }

    public override void OnDeselect()
    {
        fadeTween.Kill();
        fadeTween = DOTween.To(() => currentOpacity,
            x => SetAllMaterialsAlpha(x),
            0f,
            fadeOutTime)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                SetAllMaterialsAlpha(0f);
                selectedVisual.SetActive(false);
            });
    }


    private void OnDestroy()
    {
        if (fadeTween != null)
            fadeTween.Kill();
    }
}
