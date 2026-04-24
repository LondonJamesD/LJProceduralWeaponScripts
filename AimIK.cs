using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimIK : MonoBehaviour
{
    [SerializeField] MultiAimConstraint aimConstraint;
    [SerializeField] Transform aimIKPoint;
    Transform currentTarget;
    float timer;
    public bool aiming = false;
    public void StartAim(Transform target, float timer)
    {
        aiming = true;
        this.timer = timer;
        DOTween.To(
            () => aimConstraint.weight, 
            x => aimConstraint.weight = x, 
            1f, 
            timer)
            .SetEase(Ease.InCubic)
            .onComplete += () => aimConstraint.weight = 1f;
        currentTarget = target;
    }
    public void Update()
    {
        if (!aiming) { return; }
        aimIKPoint.position = currentTarget.position;
        aimIKPoint.rotation = currentTarget.rotation;
    }
    public void StopAim()
    {
        aiming = false;
        DOTween.To(
            () => aimConstraint.weight,
            x => aimConstraint.weight = x,
            0f,
            timer)
            .SetEase(Ease.InCubic)
            .onComplete += () => aimConstraint.weight = 0f;
    }
}
