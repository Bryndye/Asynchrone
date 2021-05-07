using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinceSetPos : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private void OnDrawGizmosSelected()
    {
        if (target == null)
        {
            return;
        }
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
    }
}
