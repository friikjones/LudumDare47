using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 endPosition;
    public float duration;
    public LoopType loopType;
    public Ease easeType;

    private void Start()
    {
        Tweener moveTween = transform.DOMove(endPosition, duration);
        moveTween.SetRelative(true);
        moveTween.SetLoops(-1, loopType);
        moveTween.SetEase(easeType);
    }

}
