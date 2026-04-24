using UnityEngine;

public class WeaponADS : ProcAnim
{
    [Header("Alignment Targets")]
    public Transform aimTarget;
    public Transform cameraTarget;

    [Header("Transition Settings")]
    [Tooltip("How fast the ADS weight reaches the target (0 to 1)")]
    public float adsTransitionSpeed = 10f;
    private float targetAdsWeight = 0f;

    [Header("Spring Settings")]
    public float positionStiffness = 200f;
    public float positionDamping = 28f;
    public float rotationStiffness = 240f;
    public float rotationDamping = 32f;

    [Range(0f, 1f)]
    public float adsWeight;

    private Vector3 posOffset;
    private Vector3 posVelocity;
    private Vector3 rotOffset;
    private Vector3 rotVelocity;

    // --- Public API ---

    public void StartADS() => targetAdsWeight = 1f;
    public void StopADS() => targetAdsWeight = 0f;
    public void ToggleADS() => targetAdsWeight = targetAdsWeight > 0.5f ? 0f : 1f;

    // Use this if you want to set a specific percentage (e.g., for variable zoom)
    public void SetADSTarget(float value) => targetAdsWeight = Mathf.Clamp01(value);

    // -------------------

    public override void PerformUpdate(AnimStack stack)
    {
        if (aimTarget == null || cameraTarget == null) return;

        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        adsWeight = Mathf.Lerp(adsWeight, targetAdsWeight, dt * adsTransitionSpeed);

        Quaternion weaponToSightRotation = Quaternion.Inverse(stack.transform.rotation) * aimTarget.rotation;
        Quaternion targetWorldRot = cameraTarget.rotation * Quaternion.Inverse(weaponToSightRotation);

        Quaternion targetLocalRot = Quaternion.Inverse(stack.transform.parent.rotation) * targetWorldRot;

        Vector3 localSightPos = stack.transform.InverseTransformPoint(aimTarget.position);

        Vector3 targetWorldPos = cameraTarget.position - (targetWorldRot * localSightPos);

        Vector3 targetLocalPos = stack.transform.parent.InverseTransformPoint(targetWorldPos);

        Vector3 desiredPosOffset = targetLocalPos - stack.baseLocalPosition;

        Quaternion relRot = Quaternion.Inverse(stack.baseLocalRotation) * targetLocalRot;
        Vector3 desiredRotOffset = NormalizeEuler(relRot.eulerAngles);

        posOffset = SpringVector(posOffset, desiredPosOffset * adsWeight, ref posVelocity, positionStiffness, positionDamping, dt);
        rotOffset = SpringVector(rotOffset, desiredRotOffset * adsWeight, ref rotVelocity, rotationStiffness, rotationDamping, dt);

        stack.positionOffset += posOffset;
        stack.rotationOffset += rotOffset;
        stack.adsWeight = adsWeight;
    }

    private static Vector3 SpringVector(Vector3 current, Vector3 target, ref Vector3 velocity, float stiffness, float damping, float dt)
    {
        Vector3 force = (target - current) * stiffness;
        velocity += force * dt;
        velocity *= Mathf.Exp(-damping * dt);
        current += velocity * dt;
        return current;
    }

    private static Vector3 NormalizeEuler(Vector3 e)
    {
        return new Vector3(
            Mathf.DeltaAngle(0f, e.x),
            Mathf.DeltaAngle(0f, e.y),
            Mathf.DeltaAngle(0f, e.z)
        );
    }
}