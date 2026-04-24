using UnityEngine;
using System.Collections.Generic;

public class HandPoseApplier : MonoBehaviour
{
    [Tooltip("The baked HandPose asset to apply.")]
    public HandPose pose;

    [Tooltip("Apply pose automatically on Start.")]
    public bool applyOnStart = false;

    private Dictionary<string, Transform> boneLookup;

    private void Awake()
    {
        BuildBoneLookup();
    }

    private void Start()
    {
        if (applyOnStart)
        {
            ApplyPose();
        }
    }

    /// <summary>
    /// Builds a lookup table of bone name -> Transform
    /// starting from this GameObject (assumed root bone).
    /// </summary>
    private void BuildBoneLookup()
    {
        boneLookup = new Dictionary<string, Transform>();
        CacheBonesRecursively(transform);
    }

    private void CacheBonesRecursively(Transform current)
    {
        if (!boneLookup.ContainsKey(current.name))
        {
            boneLookup.Add(current.name, current);
        }

        foreach (Transform child in current)
        {
            CacheBonesRecursively(child);
        }
    }

    /// <summary>
    /// Applies the pose stored in the HandPose asset.
    /// </summary>
    [ContextMenu("Apply Pose")]
    public void ApplyPose()
    {
        if (pose == null)
        {
            Debug.LogError("HandPoseApplier: No HandPose assigned.");
            return;
        }

        if (pose.handHierarchy == null || pose.handHierarchy.Count == 0)
        {
            Debug.LogError("HandPoseApplier: HandPose has no baked data.");
            return;
        }

        // Safety: ensure lookup exists
        if (boneLookup == null || boneLookup.Count == 0)
        {
            BuildBoneLookup();
        }

        ApplyBoneRecursively(pose.handHierarchy[0]);
    }

    private void ApplyBoneRecursively(HandPose.BoneData boneData)
    {
        if (!boneLookup.TryGetValue(boneData.boneName, out Transform targetBone))
        {
            Debug.LogWarning($"HandPoseApplier: Bone '{boneData.boneName}' not found in hierarchy.");
            return;
        }

        targetBone.localPosition = boneData.localPosition;
        targetBone.localRotation = boneData.localRotation;

        foreach (var child in boneData.children)
        {
            ApplyBoneRecursively(child);
        }
    }
}
