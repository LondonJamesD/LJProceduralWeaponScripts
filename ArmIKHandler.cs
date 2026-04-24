using EquipmentPack;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class ArmIKHandler : MonoBehaviour
{
    public EquipmentPack.Equipment equipRef;
    [SerializeField] private GameObject armVisuals;
    [SerializeField] private TwoBoneIKConstraint lArm;
    [SerializeField] private TwoBoneIKConstraint rArm;
    [SerializeField] private HandPoseApplier poseApplierL;
    [SerializeField] private HandPoseApplier poseApplierR;
    private Transform currentLTarget;
    private Transform currentRTarget;
    private Transform IKTargetL;
    private Transform IKTargetR;
    private void Awake()
    {
        IKTargetL = lArm.data.target;
        IKTargetR = rArm.data.target;
    }
    private void OnEnable()
    {
        equipRef.onWeaponChanged += OnEquipWeapon;
    }
    private void OnDisable()
    {
        equipRef.onWeaponChanged -= OnEquipWeapon;
    }
    public void OnEquipWeapon(Weapon wep)
    {
        if (wep == null)
        {
            armVisuals.SetActive(false);
            return;
        }
        IWeaponIK wepIK = wep as IWeaponIK;
        if (wepIK == null)
        {
            armVisuals.SetActive(false);
            return;
        } else armVisuals.SetActive(true);
        currentLTarget = wepIK.GetLeftHandIK();
        currentRTarget = wepIK.GetRightHandIK();
        HandPose leftPose = wepIK.GetLeftHandPose();
        HandPose rightPose = wepIK.GetRightHandPose();
        poseApplierL.pose = leftPose;
        poseApplierL.ApplyPose();
        poseApplierR.pose = rightPose;
        poseApplierR.ApplyPose();
    }
    public void LateUpdate()
    {
        IKTargetL.position = currentLTarget.position;
        IKTargetL.rotation = currentLTarget.rotation;

        IKTargetR.position = currentRTarget.position;
        IKTargetR.rotation = currentRTarget.rotation;
    }
}
