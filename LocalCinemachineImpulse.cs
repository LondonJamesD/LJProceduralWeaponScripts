using UnityEngine;
using Unity.Cinemachine;
public class LocalCinemachineImpulse : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource source;
    [SerializeField] private Transform relativeTo;
    [SerializeField] private Vector2 range;
    public void GenerateWithForce(float force)
    {
        Vector3 recoilLocal = new Vector3(
            Random.Range(-range.x, range.x),
            Random.Range(-range.y, range.y),
         -1f
        );

        Vector3 recoilWorld = relativeTo.TransformDirection(recoilLocal);

        source.GenerateImpulseAtPositionWithVelocity(relativeTo.position, force * recoilWorld);
    }
}
