using DG.Tweening;
using UnityEngine;

[DefaultExecutionOrder(50)]
public class KitchenObjectView : MonoBehaviour
{
    protected KitchenObjectModel model;
    protected Tween currentTween;

    public void BindModel(KitchenObjectModel model)
    {
        this.model = model;
    }

    private void Start()
    {
        Initialize();
    }

    public void SetTransform(Transform tr)
    {
        currentTween?.Kill();
        //currentTween = DOTween.To(() => transform.position,
        //    x => transform.position = x,
        //    tr.position,
        //    0.2f)
        //    .SetEase(Ease.InOutSine)
        //    .OnComplete(() => transform.position = tr.position);
        //transform.SetParent(tr, true);
        //transform.localRotation = Quaternion.identity;

        transform.SetParent(tr, true);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.InOutSine));
        sequence.Join(transform.DOLocalRotateQuaternion(Quaternion.identity, 0.2f).SetEase(Ease.InOutSine));

        currentTween = sequence;
    }

    private void OnDestroy()
    {
        currentTween?.Kill();
    }

    public virtual void Initialize() { }
}
