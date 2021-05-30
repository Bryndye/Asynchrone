using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : Singleton<Robot>
{
    NavMeshAgent nav;
    ManagerPlayers managerPlayers;
    SoundManager SM;

    [Header("Diversion")]
    [SerializeField] float rangeDis;
    [HideInInspector] public bool CanDiv;
    [HideInInspector] public GameObject RobotDiv;
    MeshFilter viewMeshFilter;
    Mesh viewMesh;
    public bool HasDiversion = false;

    [Header("Valeurs Graphiques")]
    float ShownDistance;
    public float MeshResolution;
    public int EdgeResolveIterations;
    public float EdgeDstThrehsold;
    Vector3 raycastPosition;
    [Space]
    public LayerMask ObstacleMask;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        managerPlayers = ManagerPlayers.Instance;
        SM = SoundManager.Instance;
        managerPlayers.RobotPlayer = this;
        managerPlayers.PlayerRobotTransform = transform;

        viewMeshFilter = transform.GetChild(1).GetComponent<MeshFilter>();

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    void Update()
    {
        if (!managerPlayers.PlayerCntrlerRbt.InCinematic)
        {         

            if (!managerPlayers.onPlayerHuman)
            {

                if (Input.GetKeyDown(managerPlayers.InputDiversion) && RobotDiv == null && HasDiversion)
                {
                    StartDiv();
                }
                
                if (Input.GetKeyDown(managerPlayers.InputDiversion) && RobotDiv != null)
                {
                    Destroy(RobotDiv);
                }
            }
        }
        UpdateDiversionRangeShown();
        DrawFieldOfView();
    }

    #region Diversion

    public void StartDiv() => CanDiv = !CanDiv;

    public void CreateDiversion(RaycastHit hit)
    {
        if (hit.collider.gameObject.layer != 10)
        {
            Vector3 point = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Vector3 dir = (transform.position - point).normalized;

            if (CheckWall(dir, point))
            {
                RobotDiv = Instantiate(Resources.Load<GameObject>("Player/Fake_Robot"), point, Quaternion.identity);
                SM.GetASound("DiversionSet", RobotDiv.transform);
                StockDivManager();
                CanDiv = false;
            }
        }
    }

    private void StockDivManager() => HasDiversion = false;

    private bool CheckWall(Vector3 dir, Vector3 point)
    {
        RaycastHit it;

        if (Physics.Raycast(point, dir, out it, rangeDis))
        {
            //Debug.Log(it.collider.name + "  " + it.collider.gameObject.layer);

            if (it.collider.gameObject.layer == 9 || it.collider.gameObject.layer == 8 || it.collider.gameObject.layer == 12)
            {
                return true;
            }
        }
        return false;
    }


    #region MeshDiversion

    void UpdateDiversionRangeShown()
    {
        if (managerPlayers.onPlayerHuman)
        {
            CanDiv = false;
        }

        if (CanDiv && ShownDistance < rangeDis)
        {
            ShownDistance += Time.deltaTime * 50;
            ShownDistance = Mathf.Clamp(ShownDistance, 0, rangeDis);
        }
        else if (!CanDiv && ShownDistance > 0)
        {
            ShownDistance -= Time.deltaTime * 50;
            ShownDistance = Mathf.Clamp(ShownDistance, 0, rangeDis);
        }
    }

    bool AlreadyClear = true;
    void DrawFieldOfView()
    {
        if(ShownDistance > 0)
        {
            raycastPosition = viewMeshFilter.transform.position;
            int stepCount = Mathf.RoundToInt(360 * MeshResolution);
            float stepAngleSize = 360f / (float)stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo oldViewCast = new ViewCastInfo();

            LayerMask myMask = ObstacleMask;

            for (int i = 0; i < stepCount; i++)
            {
                float angle = transform.eulerAngles.y - 360 / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = viewCast(angle, ObstacleMask);
                if (i > 0)
                {
                    bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > EdgeDstThrehsold;
                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                    {
                        EdgeInfo edge = FindEdge(oldViewCast, newViewCast, ObstacleMask);
                        if (edge.PointA != Vector3.zero)
                            viewPoints.Add(edge.PointA);
                        if (edge.PointB != Vector3.zero)
                            viewPoints.Add(edge.PointB);
                    }
                }
                viewPoints.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }

            int vertCount = viewPoints.Count + 1;
            Vector3[] verticles = new Vector3[vertCount + 1];
            int[] triangles = new int[((vertCount - 2) * 3) + 3];

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

            verticles[verticles.Length - 1] = transform.InverseTransformPoint(viewPoints[0]);

            triangles[triangles.Length - 3] = 0;
            triangles[triangles.Length - 2] = triangles[triangles.Length - 4];
            triangles[triangles.Length - 1] = 1;

            viewMesh.Clear();
            viewMesh.vertices = verticles;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();

            AlreadyClear = false;
        }

        if(ShownDistance == 0 && !AlreadyClear)
        {
            AlreadyClear = true;
            viewMesh.Clear();
        }
    }

    ViewCastInfo viewCast(float globalAngle, LayerMask AffectedLayer)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(raycastPosition, dir, out hit, ShownDistance, AffectedLayer))
        {
            Vector3 HitPointToReturn = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            return new ViewCastInfo(true, HitPointToReturn, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * ShownDistance, rangeDis, globalAngle);
        }
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, LayerMask AffectedLayer)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < EdgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = viewCast(angle, AffectedLayer);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > EdgeDstThrehsold;
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

    public Vector3 DirFromAngle(float AngleInDegrees, bool AngleisGlobal)
    {
        if (!AngleisGlobal)
            AngleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(AngleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(AngleInDegrees * Mathf.Deg2Rad));
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

    #endregion
}
