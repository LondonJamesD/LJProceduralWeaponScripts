using UnityEngine;

public interface IWeaponIK
{
    public Transform GetLeftHandIK();
    public Transform GetRightHandIK();
    public HandPose GetLeftHandPose();
    public HandPose GetRightHandPose();
}
