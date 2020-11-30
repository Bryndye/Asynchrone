using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class anAI : MonoBehaviour
{
    [Header("Composants")]
    NavMeshAgent myNavMeshAgent;

    [Header("TEST: Cible Manuelle")]
    public Transform Target;

    [Header("Champ de vision")]
    public float ViewRadius;
    [Range(0,360)]
    public float ViewAngle;
    [Space]
    public LayerMask CiblesMask;
    public LayerMask ObstacleMask;

    // Start is called before the first frame update
    void Awake()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        myNavMeshAgent.SetDestination(Target.position);
    }

    void FindVisibleTargets()
    {
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, ViewRadius, CiblesMask);

        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            Transform theTarget = targetInViewRadius[i].transform;

            Vector3 dirToTarget = (theTarget.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, dirToTarget) < ViewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, theTarget.position);

                if(!Physics.Raycast(transform.position, dirToTarget, dstToTarget, ObstacleMask))
                {

                }
            }
        }
    }

    public Vector3 DirFromAngle(float AngleInDegrees, bool AngleisGlobal)
    {
        if (!AngleisGlobal)
            AngleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(AngleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(AngleInDegrees * Mathf.Deg2Rad));
    }
}
