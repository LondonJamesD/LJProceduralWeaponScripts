using UnityEngine;

[System.Serializable]
public abstract class ProcAnim : MonoBehaviour
{
    public abstract void PerformUpdate(AnimStack stack);
}
