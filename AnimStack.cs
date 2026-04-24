using UnityEngine;
public class AnimStack : MonoBehaviour
{

    [SerializeField] ProcAnim[] anims;
    // Base pose (never modified)
    public Vector3 baseLocalPosition { get; private set; }
    public Quaternion baseLocalRotation { get; private set; }

    // Accumulated offsets (written by anims)
    [HideInInspector] public Vector3 positionOffset;
    [HideInInspector] public Vector3 rotationOffset;

    public float adsWeight;
    public float swaySuppression;
    void Start()
    {
        baseLocalPosition = transform.localPosition;
        baseLocalRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        positionOffset = Vector3.zero;
        rotationOffset = Vector3.zero;
        adsWeight = 0f;
        swaySuppression = 0f;  

        for (int i = 0; i < anims.Length; i++)
        {
            anims[i].PerformUpdate(this);
            
        }

        transform.localPosition = baseLocalPosition + positionOffset;
        transform.localRotation =
            baseLocalRotation * Quaternion.Euler(rotationOffset);

    }
}
