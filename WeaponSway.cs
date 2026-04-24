using UnityEngine;

public class WeaponSway : ProcAnim
{
    [Header("Sway Response")]
    public float positionStrength = 0.015f;
    public float rotationStrength = 1.8f;

    [Header("ADS Suppression")]
    [Tooltip("How much sway is removed at 100% ADS. 1.0 = No sway in ADS, 0.0 = Full sway in ADS.")]
    [Range(0f, 1f)] public float adsSuppressionWeight = 0.75f;

    [Header("Spring Settings")]
    public float positionStiffness = 120f;
    public float positionDamping = 20f;

    public float rotationStiffness = 90f;
    public float rotationDamping = 15f;

    private Transform parent;

    // Spring state (OFFSETS ONLY)
    private Vector3 positionOffset;
    private Vector3 positionVelocity;

    private Vector3 rotationOffset;
    private Vector3 rotationVelocity;

    // Global tracking
    private Quaternion lastParentRotation;

    void Start()
    {
        parent = transform.parent;
        if (!parent)
        {
            enabled = false;
            return;
        }

        lastParentRotation = parent.rotation;
    }

    public override void PerformUpdate(AnimStack stack)
    {
        float dt = Time.deltaTime;
        if (dt <= 0f || !parent) return;

        Quaternion deltaRotation =
            parent.rotation * Quaternion.Inverse(lastParentRotation);

        Vector3 rotEuler = NormalizeEuler(deltaRotation.eulerAngles);

        // ---------- ADS SUPPRESSION ----------
        // Calculate a multiplier: 1.0 at hipfire, down to (1.0 - adsSuppressionWeight) at full ADS
        float suppressionFactor = 1f - (stack.adsWeight * adsSuppressionWeight);

        // ---------- TARGET OFFSETS ----------
        // We apply the suppressionFactor here so the 'goal' of the spring is closer to zero during ADS
        Vector3 targetPosOffset = new Vector3(
            -rotEuler.y,
             rotEuler.x,
            0f
        ) * positionStrength * suppressionFactor;

        Vector3 targetRotOffset = new Vector3(
             rotEuler.x,
             rotEuler.y,
            -rotEuler.y
        ) * rotationStrength * suppressionFactor;

        // ---------- SPRING SOLVE ----------
        positionOffset = Spring(
            positionOffset,
            targetPosOffset,
            ref positionVelocity,
            positionStiffness,
            positionDamping,
            dt
        );

        rotationOffset = Spring(
            rotationOffset,
            targetRotOffset,
            ref rotationVelocity,
            rotationStiffness,
            rotationDamping,
            dt
        );

        stack.positionOffset += positionOffset;
        stack.rotationOffset += rotationOffset;

        lastParentRotation = parent.rotation;
    }

    Vector3 Spring(
        Vector3 current,
        Vector3 target,
        ref Vector3 velocity,
        float stiffness,
        float damping,
        float dt
    )
    {
        Vector3 force = (target - current) * stiffness;
        velocity += force * dt;
        velocity *= Mathf.Exp(-damping * dt);
        current += velocity * dt;
        return current;
    }

    Vector3 NormalizeEuler(Vector3 euler)
    {
        euler.x = Mathf.DeltaAngle(0, euler.x);
        euler.y = Mathf.DeltaAngle(0, euler.y);
        euler.z = Mathf.DeltaAngle(0, euler.z);
        return euler;
    }
}