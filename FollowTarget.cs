using UnityEngine;
[ExecuteAlways]

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public float rate;
    public bool position;
    public bool rotation;
    void LateUpdate()
    {
        if(position)
        transform.position = target.position;
        if(rotation)
        transform.rotation = target.rotation;
    }
}
