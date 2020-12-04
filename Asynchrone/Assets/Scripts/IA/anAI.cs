﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum myBehaviour { Guard, Patrol };

public class anAI : MonoBehaviour
{
    [Header("Composants")]
    NavMeshAgent myNavMeshAgent;

    [Header("Champ de vision")]
    public float ViewRadius;
    [Range(0,360)]
    public float ViewAngle;
    [Space]
    public LayerMask CiblesMask;
    public LayerMask ObstacleMask;
    [Space]
    public float MeshResolution;
    public int edgeResolveIterations;
    public float edgeDstThrehsold;

    MeshFilter viewMeshFilter;
    Mesh viewMesh;

    [Header("Comportement")]
    public myBehaviour Comportement;
    public List<Vector3> EtapesPatrouille;
    int StepPatrolIndex;
    public float Latence_InterEtapes;

    // Start is called before the first frame update
    void Awake()
    {
        viewMeshFilter = transform.GetChild(0).GetComponent<MeshFilter>();
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        if(Comportement == myBehaviour.Patrol)
            NextPatrolStep();
    }

    private void Update()
    {
        FindVisibleTargets();
        if(Comportement == myBehaviour.Patrol)
        {
            PatrolRoutine();
        }
       
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    #region Vision

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

    ViewCastInfo viewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, dir, out hit, ViewRadius, ObstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * ViewRadius, ViewRadius, globalAngle);
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(ViewAngle * MeshResolution);
        float stepAngleSize = ViewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - ViewAngle/2 + stepAngleSize * i;
            ViewCastInfo newViewCast = viewCast(angle);
            if(i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThrehsold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.PointA != Vector3.zero)
                        viewPoints.Add(edge.PointA);
                    if (edge.PointB != Vector3.zero)
                        viewPoints.Add(edge.PointB);
                }
            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertCount = viewPoints.Count + 1;
        Vector3[] verticles = new Vector3[vertCount];
        int[] triangles = new int[(vertCount - 2) * 3];

        verticles[0] = Vector3.zero;
        for (int i = 0; i < vertCount - 1; i++)
        {
            verticles[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = verticles;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    public Vector3 DirFromAngle(float AngleInDegrees, bool AngleisGlobal)
    {
        if (!AngleisGlobal)
            AngleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(AngleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(AngleInDegrees * Mathf.Deg2Rad));
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = viewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThrehsold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 PointA;
        public Vector3 PointB;

        public EdgeInfo(Vector3 _PointA, Vector3 _PointB)
        {
            PointA = _PointA;
            PointB = _PointB;
        }
    }

    #endregion

    #region Patrouille

    #region Editor

    public void AddPatrolPoint()
    {
        EtapesPatrouille.Add(transform.position);
    }

    public void ResetPositionToFirstPatrolPoint()
    {
        if(EtapesPatrouille.Count > 0)
        {
            transform.position = EtapesPatrouille[0];
        }
    }

    public void ResetPath()
    {
        EtapesPatrouille.Clear();
    }

    #endregion

    void NextPatrolStep()
    {
        StepPatrolIndex += 1;
        if(StepPatrolIndex >= EtapesPatrouille.Count)
        {
            StepPatrolIndex = 0;
        }

        myNavMeshAgent.SetDestination(EtapesPatrouille[StepPatrolIndex]);
    }

    void PatrolRoutine()
    {
        if(myNavMeshAgent.remainingDistance < 0.1f)
        {
            NextPatrolStep();
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (myNavMeshAgent == null)
            myNavMeshAgent = GetComponent<NavMeshAgent>();

        for (int i = 0; i < EtapesPatrouille.Count; i++)
        {
            Vector3 BasePosition = EtapesPatrouille[i];
            Vector3 TargetPosition = Vector3.zero;

            if (i + 1 < EtapesPatrouille.Count)
                TargetPosition = EtapesPatrouille[i + 1];
            else
                TargetPosition = EtapesPatrouille[0];

            NavMeshPath myNavMeshPath = new NavMeshPath();
            NavMesh.CalculatePath(BasePosition, TargetPosition, NavMesh.AllAreas, myNavMeshPath);

            if(myNavMeshPath.corners.Length < 2)
            {
                Gizmos.DrawLine(BasePosition, TargetPosition);
            }
            else
            {
                for (int a = 0; a + 1 < myNavMeshPath.corners.Length; a++)
                {
                    Gizmos.DrawLine(myNavMeshPath.corners[a], myNavMeshPath.corners[a + 1]);
                }
            }
        }
    }
}