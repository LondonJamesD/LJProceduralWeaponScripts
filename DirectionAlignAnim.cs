using UnityEngine;

[System.Serializable]
public class DirectionAlignAnim : ProcAnim
{
    [Tooltip("The transform whose forward direction we want to match (e.g., the Camera)")]
    public Transform targetSource;

    [Range(0, 1)] public float weight = 1f;

    public override void PerformUpdate(AnimStack stack)
    {
        if (targetSource == null) return;

        Quaternion targetWorldRot = targetSource.rotation;

        Quaternion targetLocalRot;
        if (stack.transform.parent != null)
        {
            targetLocalRot = Quaternion.Inverse(stack.transform.parent.rotation) * targetWorldRot;
        }
        else
        {
            targetLocalRot = targetWorldRot;
        }

        Quaternion requiredOffset = Quaternion.Inverse(stack.baseLocalRotation) * targetLocalRot;

        Vector3 offsetEuler = requiredOffset.eulerAngles;

        offsetEuler.x = NormalizeAngle(offsetEuler.x);
        offsetEuler.y = NormalizeAngle(offsetEuler.y);
        offsetEuler.z = NormalizeAngle(offsetEuler.z);

        stack.rotationOffset += offsetEuler * weight;
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }
}