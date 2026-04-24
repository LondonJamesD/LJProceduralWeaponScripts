using UnityEngine;

public class WeaponRecoil : ProcAnim
{
    [Header("Per-Shot Kick")]
    public Vector3 positionKick = new Vector3(0f, 0f, -0.05f);
    public Vector3 rotationKick = new Vector3(-3.5f, 1.5f, 0.5f);

    [Header("Accumulation Limits")]
    public Vector3 maxPositionRecoil = new Vector3(0f, 0f, -0.15f);
    public Vector3 maxRotationRecoil = new Vector3(-15f, 6f, 4f);

    [Header("Target Return")]
    public float recoilReturnSpeed = 10f;

    [Header("Spring Settings")]
    public float positionStiffness = 180f;
    public float positionDamping = 24f;
    public float rotationStiffness = 160f;
    public float rotationDamping = 22f;

    [Header("Recoil Envelope")]
    public float fireDecayRate = 6f;
    public AnimationCurve springResponseCurve =
        AnimationCurve.EaseInOut(0, 0.3f, 1, 1f);

    private float fire;

    private Vector3 positionTarget;
    private Vector3 rotationTarget;

    private Vector3 positionOffset;
    private Vector3 positionVelocity;
    private Vector3 rotationOffset;
    private Vector3 rotationVelocity;

    public override void PerformUpdate(AnimStack stack)
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        fire = Mathf.Max(0f, fire - fireDecayRate * dt);

        float speedMultiplier = Mathf.Lerp(1f, 0.75f, stack.adsWeight); // Return slightly slower in ADS
        float returnAlpha = recoilReturnSpeed * speedMultiplier * dt;

        positionTarget = Vector3.Lerp(positionTarget, Vector3.zero, returnAlpha);
        rotationTarget = Vector3.Lerp(rotationTarget, Vector3.zero, returnAlpha);

        float curveT = 1f - fire;
        float response = springResponseCurve.Evaluate(curveT);

        response = Mathf.Max(response, 0.01f);

        float posStiff = positionStiffness * response;
        float posDamp = positionDamping * response;
        float rotStiff = rotationStiffness * response;
        float rotDamp = rotationDamping * response;

        positionOffset = Spring(positionOffset, positionTarget, ref positionVelocity, posStiff, posDamp, dt);
        rotationOffset = Spring(rotationOffset, rotationTarget, ref rotationVelocity, rotStiff, rotDamp, dt);

        stack.positionOffset += positionOffset;
        stack.rotationOffset += rotationOffset;
    }



    // Call once per shot
    public void Fire(float strength = 1f)
    {
        fire = 1f;

        float yawSign = Random.value < 0.5f ? -1f : 1f;
        float rollSign = Random.value < 0.5f ? -1f : 1f;

        Vector3 randomizedRotationKick = new Vector3(
            rotationKick.x,
            rotationKick.y * yawSign,
            rotationKick.z * rollSign
        );

        positionTarget += positionKick * strength;
        rotationTarget += randomizedRotationKick * strength;

        positionTarget = ClampVector(positionTarget, maxPositionRecoil);
        rotationTarget = ClampVector(rotationTarget, maxRotationRecoil);
    }

    // ---------------- UTIL ----------------

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

    Vector3 ClampVector(Vector3 value, Vector3 max)
    {
        return new Vector3(
            Mathf.Clamp(value.x, -Mathf.Abs(max.x), Mathf.Abs(max.x)),
            Mathf.Clamp(value.y, -Mathf.Abs(max.y), Mathf.Abs(max.y)),
            Mathf.Clamp(value.z, -Mathf.Abs(max.z), Mathf.Abs(max.z))
        );
    }
}
