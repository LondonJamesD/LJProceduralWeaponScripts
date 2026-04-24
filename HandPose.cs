using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewHandPose", menuName = "Hand Pose/New Pose")]
public class HandPose : ScriptableObject
{
    [Tooltip("Assign the root bone of the hand hierarchy here (e.g., Wrist).")]
    public Transform rootBone;

    [Tooltip("The list of saved bone data.")]
    public List<BoneData> handHierarchy = new List<BoneData>();

    [System.Serializable]
    public class BoneData
    {
        public string boneName;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public List<BoneData> children = new List<BoneData>();

        // Constructor for convenience
        public BoneData(string name, Vector3 pos, Quaternion rot)
        {
            boneName = name;
            localPosition = pos;
            localRotation = rot;
            children = new List<BoneData>();
        }
    }

    /// <summary>
    /// Recursive function to traverse the transform hierarchy and save data.
    /// </summary>
    public void BakePose()
    {
        if (rootBone == null)
        {
            Debug.LogError("Root Bone is not assigned! Cannot bake pose.");
            return;
        }

        handHierarchy.Clear();
        BoneData rootData = CaptureBoneRecursively(rootBone);
        handHierarchy.Add(rootData);

        Debug.Log($"<color=green>Success!</color> Baked pose from {rootBone.name} with {CountBones(rootData)} total bones.");
    }

    private BoneData CaptureBoneRecursively(Transform currentTransform)
    {
        // Capture current bone data
        BoneData data = new BoneData(
            currentTransform.name,
            currentTransform.localPosition,
            currentTransform.localRotation
        );

        foreach (Transform child in currentTransform)
        {
            data.children.Add(CaptureBoneRecursively(child));
        }

        return data;
    }

    // Helper to count bones for the debug log
    private int CountBones(BoneData data)
    {
        int count = 1;
        foreach (var child in data.children)
        {
            count += CountBones(child);
        }
        return count;
    }
}

// ----------------------------------------------------------------------------
// CUSTOM EDITOR SCRIPT
// This draws the button in the Inspector.
// ----------------------------------------------------------------------------
#if UNITY_EDITOR
[CustomEditor(typeof(HandPose))]
public class HandPoseContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HandPose script = (HandPose)target;

        script.rootBone = (Transform)EditorGUILayout.ObjectField(
            "Root Bone",
            script.rootBone,
            typeof(Transform),
            true // allows dragging items from the Hierarchy
        );

        SerializedObject so = new SerializedObject(script);
        SerializedProperty hierarchyProp = so.FindProperty("handHierarchy");
        EditorGUILayout.PropertyField(hierarchyProp, true); // true = include children
        so.ApplyModifiedProperties();

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("1. Enter Play Mode.\n2. Pose the hand.\n3. Drag the Root Bone from Hierarchy to the slot above.\n4. Click Bake.", MessageType.Info);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Bake Pose", GUILayout.Height(40)))
        {
            script.BakePose();

            // Force Unity to save the data immediately
            EditorUtility.SetDirty(script);
            AssetDatabase.SaveAssets();
        }
        GUI.backgroundColor = Color.white;
    }
}
#endif