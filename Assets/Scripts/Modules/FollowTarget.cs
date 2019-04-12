using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public static bool dynamicCamera = true;

    public Transform target;
    public Vector3 positionOffset = new Vector3(0, 5, -10);
    public Vector3 rotationOffset = new Vector3(30, 0, 0);
    public Vector3 fixedRotation = new Vector3(30, 0, 0);

    void LateUpdate()
    {
        if (dynamicCamera)
        {
            transform.position = target.position + new Vector3(positionOffset.x, positionOffset.y, 0) + target.forward * positionOffset.z;
            transform.rotation = Quaternion.LookRotation(target.position - transform.position) * Quaternion.Euler(rotationOffset);
        }
        else
        {
            transform.position = target.position + positionOffset;
            transform.rotation = Quaternion.Euler(fixedRotation);
        }
    }
}
