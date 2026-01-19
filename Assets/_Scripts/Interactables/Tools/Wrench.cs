using DG.Tweening;
using UnityEngine;

public class Wrench : MonoBehaviour, IHoldableItem, IInteractable
{
    private IObjectParent Parent;
    private Tween currentTween;
    [SerializeField] private Collider sphere;

    public void ClearParent()
    {
        Parent?.SetChild(null);
        Parent = null;
    }

    public IObjectParent GetParent() => Parent;

    public AudioSO GetPickUpAudio()
    {
        return null;
    }

    public AudioSO GetPlaceAudio()
    {
        return null;
    }

    public void Hold()
    {
        //nothing
    }

    public bool IsTwoHanded()
    {
        return false;
    }



    public void SetParent(IObjectParent parent)
    {
        Parent = parent;

        if (parent != null)
        {
            parent.SetChild(this);
            SetTransform(parent.GetTransform());
        }
    }

    public void SwapParent(IObjectChild swap)
    {
        var parentA = GetParent();
        var parentB = swap.GetParent();

        if (parentA == parentB)
            return;

        swap.ClearParent();
        this.ClearParent();

        this.SetParent(parentB);
        swap.SetParent(parentA);
    }

    public void SetTransform(Transform tr)
    {
        currentTween?.Kill();
        
        transform.SetParent(tr, true);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.InOutSine));
        sequence.Join(transform.DOLocalRotateQuaternion(Quaternion.identity, 0.2f).SetEase(Ease.InOutSine));

        currentTween = sequence;
    }

    public InteractionResult TryInteract(PlayerController context)
    {
        if(context.CanPickUpItem(this))
        {
            SetParent(context);
            sphere.enabled = false;
        }
        return InteractionResult.Ok("");
    }

    public bool CanInteract(PlayerController context)
    {
        return true;
    }
}
