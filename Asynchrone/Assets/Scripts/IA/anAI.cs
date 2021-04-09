using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public enum myBehaviour { Guard, Patrol };
public enum Situation { None, Interaction, GuardMove, PatrolMove, PatrolWait, Interrogation, Pursuit, Dead, Loop };
public enum Classe { Basic, Drone };

public class anAI : MonoBehaviour
{
    [Header("Externe")]
    SpawnMANAGER SM;

    [Header("Composants")]
    NavMeshAgent myNavMeshAgent;
    public GameObject SkinRobot;
    public GameObject SkinDrone;

    [Header("Global")]
    public Classe myClasse;
    bool EnnemiTué;
    [Space]

    [Header("Speeds")]
    public float NormalSpeed;
    public float PursuitSpeed;

    [Header("Champ de vision"), HideInInspector]
    public List<Transform> Vus;
    [HideInInspector]
    public  float TempsInterrogation, TempsRegard; 
    [Tooltip("Temps d'attente avant de commencer à poursuivre alors que l'IA a un joueur en vue")]
    public float LatenceInterrogation;

    [Space]
    [Header("Champs de vision")]
    [Space, Tooltip("Distance du champs de vision")]
    public float ViewRadius;
    public float HearsRadius;
    [Range(0,360), Tooltip("Angle du champs de vision en degrés")]
    public float ViewAngle;
    [Space, Tooltip("Englobe les layers considérées comme des cibles à voir et poursuivre")]
    public LayerMask CiblesMask;
    [Tooltip("Englobe les layers considérées comme des obstacles à la vision")]
    public LayerMask ObstacleMask;
    public LayerMask ObstackeMaskWithoutWall;
    public LayerMask ObstacleMaskWithoutLowWall;
    [Space, Tooltip("Résolution du cône de vision. Plus elle est grande, plus le cône sera précis mais coûteux en ressources")]
    public float MeshResolution;
    public int edgeResolveIterations;
    public float edgeDstThrehsold;

    [Header("Meshs")]
    Vector3 RaycastPosition;
    MeshFilter viewMeshFilter;
    Mesh viewMesh;
    MeshFilter viewMeshFilter2;
    Mesh viewMesh2;
    MeshFilter viewMeshFilter3;
    Mesh viewMesh3;

    [Header("Comportement")]
    public myBehaviour Comportement;
    public Situation mySituation;
    GameObject myUI;
    public bool PositionChecked;
    public Vector3 PursuitLastPosition;

    [Header("Garde")]
    public Vector3 BasePosition;
    public Quaternion BaseRotation;
    public List<Vector3> EtapesRotation;
    int StepRotationIndex;
    float Temps_InterRotations;
    public float Latence_InterRotations;

    [Header("Patrouille")]
    [SerializeField]
    public List<Vector3> EtapesPatrouille;
    [SerializeField]
    public int IndexStartPatrouilleSecondaire;
    [SerializeField]
    public List<Vector3> EtapesPatrouilleSecondaire;
    int StepPatrolIndex;
    bool UseSecondaryPatrol;
    public float Latence_InterEtapes;
    [HideInInspector]
    float Temps_InterEtapes;

    [Header("Interaction")]
    Vector3 InteractionTarget;
    float TempsInteraction;
    bool AlreadyInteracted;

    // Start is called before the first frame update
    void Awake()
    {
        viewMeshFilter = transform.GetChild(0).GetComponent<MeshFilter>();
        viewMeshFilter2 = transform.GetChild(1).GetComponent<MeshFilter>();
        viewMeshFilter3 = transform.GetChild(2).GetComponent<MeshFilter>();
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        SM = SpawnMANAGER.Instance;

        if (!SM.myAIs.Contains(this))
            SM.myAIs.Add(this);
    }

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        viewMesh2 = new Mesh();
        viewMesh2.name = "View Mesh 2";
        viewMeshFilter2.mesh = viewMesh2;

        viewMesh3 = new Mesh();
        viewMesh3.name = "View Mesh 3";
        viewMeshFilter3.mesh = viewMesh3;

        BasePosition = transform.position;
        BaseRotation = transform.rotation;
        if (Comportement == myBehaviour.Guard && EtapesRotation.Count == 0)
            EtapesRotation.Add(BasePosition + transform.forward);

        if (Comportement == myBehaviour.Patrol)
            NextPatrolStep();

        myUI = Instantiate(Resources.Load<GameObject>("UI/aFollowingState"));
        myUI.GetComponent<AIStateUI>().Declaration(this);
    }

    private void Update()
    {
        Vus.Clear();
        if (SM.mySpawnSituation == SpawnSituation.Playing)
        {
            FindVisibleTargets();
            if (Comportement == myBehaviour.Patrol)
            {
                PatrolRoutine();
            }
            if (mySituation != Situation.Interrogation && mySituation != Situation.Pursuit)
            {
                if (Vus.Count > 0)
                    StopPatrol();
                else if (Comportement == myBehaviour.Guard && mySituation == Situation.GuardMove)
                {
                    GuardVerifyToBase();
                }
                else if (Comportement == myBehaviour.Guard && mySituation == Situation.None)
                {
                    ForceLook();
                    Temps_InterRotations += Time.deltaTime;
                    if (Temps_InterRotations >= Latence_InterRotations)
                    {
                        Temps_InterRotations = 0;
                        StepRotationIndex += 1;
                        if (StepRotationIndex >= EtapesRotation.Count)
                        {
                            StepRotationIndex = 0;
                        }
                    }
                }
                else if (Comportement == myBehaviour.Patrol && mySituation == Situation.Interaction)
                {
                    TempsInteraction += Time.deltaTime;
                    if (TempsInteraction >= 3)
                    {
                        TempsInteraction = 0;
                        NextPatrolStep();
                    }
                }
            }
            else if (mySituation == Situation.Interrogation)
            {
                if (Vus.Count > 0)
                {
                    LookTo();
                    InInterrogation();
                    PositionChecked = false;
                }
                else if (!PositionChecked)
                {
                    Vector3 Direction = (new Vector3(PursuitLastPosition.x, transform.position.y, PursuitLastPosition.z) - transform.position);
                    if (Vector3.Angle(Direction, (transform.forward)) > 10)
                    {
                        ForceLook();
                    }
                    else
                        PositionChecked = true;
                }
                else
                {
                    TempsInterrogation -= Time.deltaTime / 10;
                    TempsRegard += Time.deltaTime;
                    if (TempsRegard >= 3)
                    {
                        TempsRegard = 0;
                        if(!EnnemiTué)
                            PursuitLastPosition = transform.position + GetRandomPositionAround();
                    }


                    if (TempsInterrogation <= 0)
                    {
                        TempsInterrogation = Mathf.Clamp(TempsInterrogation, 0f, LatenceInterrogation);

                        if (Comportement == myBehaviour.Patrol)
                            NextPatrolStep();
                        else if (Comportement == myBehaviour.Guard)
                            GuardReturnToBase();
                    }
                    else
                    {
                        ForceLook();
                    }
                }
            }
            else if (mySituation == Situation.Pursuit)
            {
                Pursuit();
            }
        }
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    public void ChangeClass()
    {
        if (!myNavMeshAgent)
            myNavMeshAgent = GetComponent<NavMeshAgent>();

        if (myClasse == Classe.Basic)
        {
            myClasse = Classe.Drone;
            myNavMeshAgent.areaMask = 8;
            transform.position = new Vector3(transform.position.x, 4.17f, transform.position.z);

            for (int i = 0; i < EtapesPatrouille.Count; i++)
            {
                EtapesPatrouille[i] = new Vector3(EtapesPatrouille[i].x, 4.17f, EtapesPatrouille[i].z);
            }
            for (int i = 0; i < EtapesPatrouilleSecondaire.Count; i++)
            {
                EtapesPatrouille[i] = new Vector3(EtapesPatrouilleSecondaire[i].x, 4.17f, EtapesPatrouilleSecondaire[i].z);
            }

            SkinDrone.SetActive(true);
            SkinRobot.SetActive(false);

            transform.GetChild(0).position -= new Vector3(0, 3, 0);
            transform.GetChild(1).position -= new Vector3(0, 3, 0);
            transform.GetChild(2).position -= new Vector3(0, 3, 0);

            SetObjectDirty(this);
        }
        else
        {
            myClasse = Classe.Basic;
            myNavMeshAgent.areaMask = 3;
            transform.position = new Vector3(transform.position.x, 1.1f, transform.position.z);

            for (int i = 0; i < EtapesPatrouille.Count; i++)
            {
                EtapesPatrouille[i] = new Vector3(EtapesPatrouille[i].x, 1.1f, EtapesPatrouille[i].z);
            }
            for (int i = 0; i < EtapesPatrouilleSecondaire.Count; i++)
            {
                EtapesPatrouille[i] = new Vector3(EtapesPatrouilleSecondaire[i].x, 1.1f, EtapesPatrouilleSecondaire[i].z);
            }

            transform.GetChild(0).position += new Vector3(0, 3, 0);
            transform.GetChild(1).position += new Vector3(0, 3, 0);
            transform.GetChild(2).position += new Vector3(0, 3, 0);

            SkinDrone.SetActive(false);
            SkinRobot.SetActive(true);

            SetObjectDirty(this);
        }
    }

    #region Vision

    void FindVisibleTargets()
    {
        RaycastPosition = viewMeshFilter.transform.position;
        if(myClasse == Classe.Basic)
        {
            Collider[] targetInViewRadius = Physics.OverlapSphere(RaycastPosition, ViewRadius, CiblesMask);

            for (int i = 0; i < targetInViewRadius.Length; i++)
            {
                Transform theTarget = targetInViewRadius[i].transform;
                Vector3 theTargetRelocalised = new Vector3(theTarget.position.x, RaycastPosition.y, theTarget.position.z);

                Vector3 dirToTarget = (theTargetRelocalised - RaycastPosition).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < ViewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, theTarget.position);

                    bool HitWall = Physics.Raycast(RaycastPosition, dirToTarget, dstToTarget, ObstacleMaskWithoutLowWall);
                    bool HitLowWall = Physics.Raycast(RaycastPosition, dirToTarget, dstToTarget, ObstackeMaskWithoutWall);

                    if (HitLowWall)
                    {
                        if (HitLowWall && theTarget.GetComponent<Human>() != null && !theTarget.GetComponent<Human>().isAccroupi)
                            HitLowWall = false;
                    }

                    if (!HitWall && !HitLowWall)
                    {
                        Vus.Add(theTarget);
                    }
                }
            }
        }
        else if(myClasse == Classe.Drone)
        {
            Collider[] targetInViewRadius = Physics.OverlapSphere(RaycastPosition, ViewRadius, CiblesMask);

            for (int i = 0; i < targetInViewRadius.Length; i++)
            {
                Transform theTarget = targetInViewRadius[i].transform;
                Vector3 theTargetRelocalised = new Vector3(theTarget.position.x, RaycastPosition.y, theTarget.position.z);

                Vector3 dirToTarget = (theTargetRelocalised - RaycastPosition).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < ViewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, theTarget.position);

                    bool HitWall = Physics.Raycast(RaycastPosition, dirToTarget, dstToTarget, ObstacleMaskWithoutLowWall);

                    if (!HitWall)
                    {
                        Vus.Add(theTarget);
                    }
                }
            }
        }

        Collider[] targetInHearsRadius = Physics.OverlapSphere(RaycastPosition, HearsRadius, CiblesMask);

        for (int i = 0; i < targetInHearsRadius.Length; i++)
        {
            Transform theTarget = targetInHearsRadius[i].transform;
            Vector3 theTargetRelocalised = new Vector3(theTarget.position.x, RaycastPosition.y, theTarget.position.z);

            Vector3 dirToTarget = (theTargetRelocalised - RaycastPosition).normalized;
            float dstToTarget = Vector3.Distance(transform.position, theTarget.position);

            bool HitWall = Physics.Raycast(RaycastPosition, dirToTarget, dstToTarget, ObstacleMaskWithoutLowWall);

            if (!HitWall)
            {
                if ((!theTarget.GetComponent<NavMeshAgent>() || theTarget.GetComponent<NavMeshAgent>().speed > 0.5f) && (!theTarget.GetComponent<Human>() || !theTarget.GetComponent<Human>().isAccroupi))
                    Vus.Add(theTarget);
            }
        }

        Vus = Vus.OrderBy(w => Vector3.Distance(w.position, transform.position)).ToList();
    }

    ViewCastInfo viewCast(float globalAngle, LayerMask AffectedLayer, bool UseViewRadius = true)
    {
        float newViewRadius = ViewRadius;
        if (!UseViewRadius) newViewRadius = HearsRadius;
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(RaycastPosition, dir, out hit, newViewRadius, AffectedLayer))
        {
            Vector3 HitPointToReturn = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            return new ViewCastInfo(true, HitPointToReturn, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * newViewRadius, newViewRadius, globalAngle);
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(ViewAngle * MeshResolution);
        float stepAngleSize = ViewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        LayerMask myMask = ObstacleMask;
        if (myClasse == Classe.Drone)
            myMask = ObstacleMaskWithoutLowWall;

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = viewCast(angle, myMask);
            if(i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThrehsold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast, myMask);
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

        // 2eme Mesh

        if(myClasse != Classe.Drone)
        {
            viewPoints = new List<Vector3>();
            oldViewCast = new ViewCastInfo();

            for (int i = 0; i < stepCount; i++)
            {
                float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = viewCast(angle, ObstacleMaskWithoutLowWall);
                if (i > 0)
                {
                    bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThrehsold;
                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                    {
                        EdgeInfo edge = FindEdge(oldViewCast, newViewCast, ObstacleMaskWithoutLowWall);
                        if (edge.PointA != Vector3.zero)
                            viewPoints.Add(edge.PointA);
                        if (edge.PointB != Vector3.zero)
                            viewPoints.Add(edge.PointB);
                    }
                }
                viewPoints.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }

            vertCount = viewPoints.Count + 1;
            verticles = new Vector3[vertCount];
            triangles = new int[(vertCount - 2) * 3];

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

            viewMesh2.Clear();
            viewMesh2.vertices = verticles;
            viewMesh2.triangles = triangles;
            viewMesh2.RecalculateNormals();
        }

        // 3eme Mesh

        viewPoints = new List<Vector3>();
        oldViewCast = new ViewCastInfo();

        int stepCount2 = Mathf.RoundToInt(360 * MeshResolution);

        for (int i = 0; i < stepCount2; i++)
        {
            float angle = transform.eulerAngles.y - 360 / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = viewCast(angle, ObstacleMaskWithoutLowWall, false);
            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThrehsold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast, ObstacleMaskWithoutLowWall);
                    if (edge.PointA != Vector3.zero)
                        viewPoints.Add(edge.PointA);
                    if (edge.PointB != Vector3.zero)
                        viewPoints.Add(edge.PointB);
                }
            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        vertCount = viewPoints.Count + 1;
        verticles = new Vector3[vertCount];
        triangles = new int[((vertCount - 2) * 3) + 3];

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

        viewMesh3.Clear();
        viewMesh3.vertices = verticles;
        viewMesh3.triangles = triangles;
        viewMesh3.RecalculateNormals();
    }

    public Vector3 DirFromAngle(float AngleInDegrees, bool AngleisGlobal)
    {
        if (!AngleisGlobal)
            AngleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(AngleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(AngleInDegrees * Mathf.Deg2Rad));
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, LayerMask AffectedLayer)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = viewCast(angle, AffectedLayer);

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

    //

    void LookTo()
    {
        Transform target = Vus[0];
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        PursuitLastPosition = target.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
    }

    #endregion

    #region Patrouille et garde

    #region Editor_Patrouille

    public void AddPatrolPoint()
    {
#if UNITY_EDITOR
        EtapesPatrouille.Add(transform.position);
        SetObjectDirty(this);
#endif
    }

    public void AddSecondaryPatrolPoint()
    {
#if UNITY_EDITOR
        if (EtapesPatrouilleSecondaire.Count == 0)
            IndexStartPatrouilleSecondaire = EtapesPatrouille.Count;

        EtapesPatrouilleSecondaire.Add(transform.position);

        SetObjectDirty(this);
#endif
    }

    public void ResetPositionToFirstPatrolPoint()
    {
#if UNITY_EDITOR
        if (EtapesPatrouille.Count > 0)
        {
            transform.position = EtapesPatrouille[0];
        }

        SetObjectDirty(this);
#endif
    }

    public void ResetPath()
    {
#if UNITY_EDITOR
        EtapesPatrouille.Clear();
        EtapesPatrouille = new List<Vector3>();
        EtapesPatrouilleSecondaire.Clear();
        EtapesPatrouille = new List<Vector3>();
        IndexStartPatrouilleSecondaire = 0;

        SetObjectDirty(this);
#endif
    }

    public static void SetObjectDirty(Component comp)
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
            return;

        HandlePrefabInstance(comp.gameObject);
        UnityEditor.EditorUtility.SetDirty(comp);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(comp.gameObject.scene);
#endif
    }

    private static void HandlePrefabInstance(GameObject gameObject)
    {
#if UNITY_EDITOR
        var myPrefabType = UnityEditor.PrefabUtility.GetPrefabAssetType(gameObject);
        if (myPrefabType != UnityEditor.PrefabAssetType.NotAPrefab)
        {
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);
        }
#endif
    }

    #endregion

    #region Editor_Rotation

    public void AddRotationPoint()
    {
#if UNITY_EDITOR
        EtapesRotation.Add(transform.position + transform.forward);
        SetObjectDirty(this);
#endif
    }

    public void ResetRotations()
    {
#if UNITY_EDITOR
        EtapesRotation.Clear();
        EtapesRotation = new List<Vector3>();
        SetObjectDirty(this);
#endif
    }

    #endregion

    void NextPatrolStep()
    {
        if(EtapesPatrouille.Count > 0)
        {
            bool InteractionValid = VerifyInteraction();
            if (!InteractionValid || (InteractionValid && AlreadyInteracted))
            {
                if (myNavMeshAgent.isStopped)
                    myNavMeshAgent.isStopped = false;

                StepPatrolIndex += 1;
                if (AlreadyInteracted)
                {
                    UseSecondaryPatrol = true;
                    StepPatrolIndex = 0;
                }
                if (StepPatrolIndex >= LogicPatrol().Count)
                {
                    UseSecondaryPatrol = false;
                    StepPatrolIndex = 0;
                }

                myNavMeshAgent.SetDestination(LogicPatrol()[StepPatrolIndex]);
                mySituation = Situation.PatrolMove;
                AlreadyInteracted = false;
            }
            else
            {
                mySituation = Situation.Interaction;
                AlreadyInteracted = true;
            }
        }
    }

    void PatrolRoutine()
    {
        if( mySituation == Situation.PatrolMove && myNavMeshAgent.remainingDistance < 0.1f)
        {
            mySituation = Situation.PatrolWait;
        }
        else if(mySituation == Situation.PatrolWait)
        {
            Temps_InterEtapes += Time.deltaTime;
            if(Temps_InterEtapes >= Latence_InterEtapes)
            {
                Temps_InterEtapes = 0;
                NextPatrolStep();
            }
        }
    }

    List<Vector3> LogicPatrol()
    {
        if (UseSecondaryPatrol)
            return EtapesPatrouilleSecondaire;
        else
            return EtapesPatrouille;
    }

    void StopPatrol()
    {
        myNavMeshAgent.isStopped = true;
        mySituation = Situation.Interrogation;

        StepPatrolIndex -= 1;
        if (StepPatrolIndex < 0)
            StepPatrolIndex = EtapesPatrouille.Count - 1;
    }

    bool VerifyInteraction()
    {
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, 2, ObstacleMaskWithoutLowWall);
        bool HasInteraction = false;

        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            if(targetInViewRadius[i].gameObject.tag == "InteractionTarget")
            {
                InteractionTarget = new Vector3(targetInViewRadius[i].transform.position.x, transform.position.y, targetInViewRadius[i].transform.position.z); 
                HasInteraction = true;
                break;
            }
        }

        return HasInteraction;
    }

    void GuardReturnToBase()
    {
        if (myNavMeshAgent.isStopped)
            myNavMeshAgent.isStopped = false;

        myNavMeshAgent.SetDestination(BasePosition);
        mySituation = Situation.GuardMove;
    }

    void GuardVerifyToBase()
    {
        //Debug.Log(myNavMeshAgent.remainingDistance);
        if (myNavMeshAgent.remainingDistance < 0.1f)
        {
            if (!myNavMeshAgent.isStopped)
                myNavMeshAgent.isStopped = true;
            mySituation = Situation.None;
        }
    }

    void ForceLook()
    {
        Vector3 MyCostumPosition = transform.position + transform.forward;
        if (mySituation == Situation.Interaction)
            MyCostumPosition = InteractionTarget;
        else if (mySituation == Situation.Interrogation)
            MyCostumPosition = PursuitLastPosition;
        else
            MyCostumPosition = EtapesRotation[StepRotationIndex];
        Quaternion targetRotation = Quaternion.LookRotation(MyCostumPosition - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
    }

    Vector3 GetRandomPositionAround()
    {
        Vector3 toReturn = Vector3.forward;
        int rnd = Random.Range(0, 4);
        switch (rnd)
        {
            case 0:
                toReturn = Vector3.forward;
                break;
            case 1:
                toReturn = Vector3.right;
                break;
            case 2:
                toReturn = Vector3.left;
                break;
            case 3:
                toReturn = Vector3.back;
                break;
        }
        return toReturn;
    }

    #endregion

    #region Interrogation

    void InInterrogation()
    {
        if (TempsInteraction == 0)
            PositionChecked = false;
        TempsInterrogation += Time.deltaTime;
        if(TempsInterrogation >= LatenceInterrogation)
        {
            TempsInterrogation = Mathf.Clamp(TempsInterrogation, 0f, LatenceInterrogation);
            StartPursuit();
        }
    }

    #endregion

    #region Pursuit

    void StartPursuit()
    {
        mySituation = Situation.Pursuit;
        myNavMeshAgent.isStopped = false;
        myNavMeshAgent.speed = PursuitSpeed;
        myNavMeshAgent.destination = Vus[0].position;
    }

    void Pursuit()
    {
        if(Vus.Count > 0)
        {
            myNavMeshAgent.SetDestination(Vus[0].position);           
            Vector3 ScaledPosition = new Vector3(Vus[0].position.x, transform.position.y, Vus[0].position.z);
            PursuitLastPosition = ScaledPosition;
            if (Vus[0].gameObject.name == "Fake_Robot(Clone)" && Vector3.Distance(ScaledPosition, transform.position) < 1.5f)
            {
                EnnemiTué = true;
                Destroy(Vus[0].gameObject);
                StopPursuit();
            }
            else if (Vus[0].GetComponent<PlayerController>() && Vector3.Distance(ScaledPosition, transform.position) < 1.5f)
            {
                Kill();
            }
        }
        else if(Vector3.Distance(PursuitLastPosition, transform.position) > 1)
        {
            myNavMeshAgent.SetDestination(PursuitLastPosition);
        }
        else
        {
            StopPursuit();
        }
    }

    void StopPursuit()
    {
        mySituation = Situation.Interrogation;
        myNavMeshAgent.isStopped = true;
        myNavMeshAgent.speed = NormalSpeed;
        PursuitLastPosition = transform.position + transform.forward;
    }

    void Kill()
    {
        Vus[0].GetComponent<PlayerController>().Death();
        SM.mySpawnSituation = SpawnSituation.DeathProcess;
    }

    #endregion

    void ResetVisionAndInterrogation()
    {
        if (Comportement == myBehaviour.Guard)
        {
            mySituation = Situation.GuardMove;
        }
        else
        {
            mySituation = Situation.PatrolMove;
        }
        myNavMeshAgent.isStopped = false;
        myNavMeshAgent.speed = NormalSpeed;

        TempsInterrogation = 0;
        TempsRegard = 0;
    }

    #region Reset and Respawn

    public void AIReset()
    {
        transform.position = BasePosition;
        transform.rotation = BaseRotation;
        myNavMeshAgent.Warp(transform.position);

        StepPatrolIndex = 0;
        StepRotationIndex = 0;
        Vus = new List<Transform>();

        if (Comportement == myBehaviour.Guard && EtapesRotation.Count == 0)
            EtapesRotation.Add(BasePosition + transform.forward);

        if (Comportement == myBehaviour.Patrol)
            NextPatrolStep();

        if (!myUI)
        {
            myUI = Instantiate(Resources.Load<GameObject>("UI/aFollowingState"));
            myUI.GetComponent<AIStateUI>().Declaration(this);
        }
        else if (!myUI.gameObject.activeSelf)
            myUI.SetActive(true);

        ResetVisionAndInterrogation();
    }

    #endregion

    public bool Killable()
    {
        if (mySituation != Situation.Pursuit && mySituation != Situation.Interrogation && myClasse != Classe.Drone)
            return true;
        else
            return false;
    }

    public void Death()
    {
        myUI.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if(Comportement == myBehaviour.Patrol)
        {
            Gizmos.color = Color.yellow;

            if (myNavMeshAgent == null)
                myNavMeshAgent = GetComponent<NavMeshAgent>();

            for (int i = 0; i < EtapesPatrouille.Count; i++)
            {
                Gizmos.DrawSphere(EtapesPatrouille[i], 0.1f);

                Vector3 BasePosition = EtapesPatrouille[i];
                Vector3 TargetPosition = Vector3.zero;

                if (i + 1 < EtapesPatrouille.Count)
                    TargetPosition = EtapesPatrouille[i + 1];
                else
                    TargetPosition = EtapesPatrouille[0];

                NavMeshPath myNavMeshPath = new NavMeshPath();
                NavMesh.CalculatePath(BasePosition, TargetPosition, NavMesh.AllAreas, myNavMeshPath);

                if (myNavMeshPath.corners.Length < 2)
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

            Gizmos.color = Color.red;

            if (EtapesPatrouilleSecondaire.Count > 0)
            {
                Vector3 FirstBasePosition = EtapesPatrouille[IndexStartPatrouilleSecondaire - 1];
                Vector3 FirstTargetPosition = EtapesPatrouilleSecondaire[0];

                NavMeshPath myFirstNavMeshPath = new NavMeshPath();
                NavMesh.CalculatePath(FirstBasePosition, FirstTargetPosition, NavMesh.AllAreas, myFirstNavMeshPath);

                if (myFirstNavMeshPath.corners.Length < 2)
                {
                    Gizmos.DrawLine(FirstBasePosition, FirstTargetPosition);
                }
                else
                {
                    for (int a = 0; a + 1 < myFirstNavMeshPath.corners.Length; a++)
                    {
                        Gizmos.DrawLine(myFirstNavMeshPath.corners[a], myFirstNavMeshPath.corners[a + 1]);
                    }
                }

                for (int i = 0; i < EtapesPatrouilleSecondaire.Count; i++)
                {
                    Gizmos.DrawSphere(EtapesPatrouilleSecondaire[i], 0.1f);

                    Vector3 BasePosition = EtapesPatrouilleSecondaire[i];
                    Vector3 TargetPosition = Vector3.zero;

                    if (i + 1 < EtapesPatrouilleSecondaire.Count)
                        TargetPosition = EtapesPatrouilleSecondaire[i + 1];
                    else
                        TargetPosition = EtapesPatrouille[0];

                    NavMeshPath myNavMeshPath = new NavMeshPath();
                    NavMesh.CalculatePath(BasePosition, TargetPosition, NavMesh.AllAreas, myNavMeshPath);

                    if (myNavMeshPath.corners.Length < 2)
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

        if(Comportement == myBehaviour.Guard)
        {
            Gizmos.color = Color.blue;

            for (int i = 0; i < EtapesRotation.Count; i++)
            {
                Vector3 Direction = EtapesRotation[i] - transform.position;
                Vector3 DirectionEtape = transform.position + Direction * ViewRadius;
                Gizmos.DrawLine(transform.position, DirectionEtape);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player") && mySituation != Situation.Interrogation && mySituation != Situation.Pursuit)
        {
            PositionChecked = false;
            mySituation = Situation.Interrogation;
            PursuitLastPosition = collision.gameObject.transform.position;
        }
    }
}