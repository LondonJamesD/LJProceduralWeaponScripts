using UnityEngine;

public class WeaponWalkSway : ProcAnim
{
    [Header("Inertia Sway")]
    public float horizontalAmount = 0.015f;
    public float verticalAmount = 0.02f;
    public float velocityMultiplier = 1.0f;
    public float maxVelocity = 4.5f;

    [Header("ADS Suppression")]
    [Tooltip("How much walk bob/sway is removed when aiming (0 = full bob, 1 = no bob)")]
    [Range(0f, 1f)] public float adsSuppression = 0.8f;

    [Header("Spring Settings")]
    public float springStrength = 120f;
    public float damping = 18f;

    [Header("Bob Settings")]
    public float bobRate = 7.5f;
    public float bobAmountX = 0.01f;
    public float bobAmountY = 0.015f;
    public float bobRotation = 1.0f;

    private Vector3 lastParentPos;
    private Vector3 swayOffset;
    private Vector3 swayVelocity;
    private float bobPhase;

    void Start()
    {
        lastParentPos = transform.parent.position;
    }

    public override void PerformUpdate(AnimStack stack)
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        Transform parent = transform.parent;

        // 1. Calculate Velocity
        Vector3 parentPos = parent.position;
        Vector3 worldVelocity = (parentPos - lastParentPos) / dt;
        lastParentPos = parentPos;

        float speed = Mathf.Min(worldVelocity.magnitude, maxVelocity);
        Vector3 localVelocity = parent.InverseTransformDirection(worldVelocity);

        // 2. Calculate Suppression Factor
        // We dampen the bob and sway significantly during ADS to keep sights usable
        float suppression = 1f - (stack.adsWeight * adsSuppression);

        // Also fade out the bob if we aren't moving much
        float movementFade = Mathf.Clamp01(speed / 0.5f);

        // 3. Target Spring Sway
        Vector3 targetOffset = new Vector3(
            localVelocity.x * horizontalAmount,
            -Mathf.Abs(localVelocity.z) * verticalAmount,
            0f
        ) * velocityMultiplier * suppression;

        // 4. Spring Integration
        Vector3 displacement = swayOffset - targetOffset;
        Vector3 springForce = (-springStrength * displacement) - (damping * swayVelocity);

        swayVelocity += springForce * dt;
        swayOffset += swayVelocity * dt;

        // 5. Bob Phase & Calculation
        bobPhase += dt * bobRate * (speed / maxVelocity);
        if (bobPhase > Mathf.PI * 2f) bobPhase -= Mathf.PI * 2f;

        float sin = Mathf.Sin(bobPhase);
        float cos = Mathf.Cos(bobPhase * 2f);

        // Apply suppression and movement fade to the bob
        float finalBobWeight = suppression * movementFade;

        Vector3 bobOffset = new Vector3(
            sin * bobAmountX,
            -Mathf.Abs(cos) * bobAmountY,
            0f
        ) * finalBobWeight;

        Vector3 bobRotationOffset = new Vector3(
            cos * bobRotation,
            sin * bobRotation * 0.5f,
            sin * bobRotation
        ) * finalBobWeight;

        // 6. Write to Stack
        stack.positionOffset += swayOffset + bobOffset;
        stack.rotationOffset += bobRotationOffset;
    }
}