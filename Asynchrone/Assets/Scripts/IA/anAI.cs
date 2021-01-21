using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public enum myBehaviour { Guard, Patrol };
public enum Situation { None, Interaction, GuardMove, PatrolMove, PatrolWait, Interrogation, Pursuit, Dead };
public enum Classe { Basic, Looper };

public class anAI : MonoBehaviour
{
    [Header("Composants")]
    NavMeshAgent myNavMeshAgent;

    [Header("Global")]
    public Classe myClasse;
    [Space]

    [Header("Champ de vision"), HideInInspector]
    public List<Transform> Vus;
    [HideInInspector]
    public  float TempsInterrogation, TempsRegard;
    [Tooltip("Temps d'attente avant de commencer à poursuivre alors que l'IA a un joueur en vue")]
    public float LatenceInterrogation;
    [Space, Tooltip("Distance du champs de vision")]
    public float ViewRadius;
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
    MeshFilter viewMeshFilter;
    Mesh viewMesh;
    MeshFilter viewMeshFilter2;
    Mesh viewMesh2;

    [Header("Comportement")]
    public myBehaviour Comportement;
    public Situation mySituation;
    GameObject myUI;
    public Vector3 PursuitLastPosition;

    [Header("Garde")]
    public Vector3 BasePosition;
    public Vector3 ForwardRotationBase;

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
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        viewMesh2 = new Mesh();
        viewMesh.name = "View Mesh 2";
        viewMeshFilter2.mesh = viewMesh2;

        BasePosition = transform.position;
        ForwardRotationBase = BasePosition + transform.forward;       

        if(Comportement == myBehaviour.Patrol)
            NextPatrolStep();

        myUI = Instantiate(Resources.Load<GameObject>("UI/aFollowingState"));
        myUI.GetComponent<AIStateUI>().Declaration(this);
    }

    private void Update()
    {
        Vus.Clear();
        FindVisibleTargets();
        if(Comportement == myBehaviour.Patrol)
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
                ForceLook();

            else if(Comportement == myBehaviour.Patrol && mySituation == Situation.Interaction)
            {
                TempsInteraction += Time.deltaTime;
                if(TempsInteraction >= 3)
                {
                    TempsInteraction = 0;
                    NextPatrolStep();
                }
            }

        }
        else if(mySituation == Situation.Interrogation)
        {
            if (Vus.Count > 0)
            {
                LookTo();
                InInterrogation();
            }
            else
            {
                TempsInterrogation -= Time.deltaTime / 10;
                TempsRegard += Time.deltaTime;
                if(TempsRegard >= 3)
                {
                    TempsRegard = 0;
                    PursuitLastPosition = transform.position + GetRandomPositionAround();
                }

                if(TempsInterrogation <= 0)
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
        else if(mySituation == Situation.Pursuit)
        {
            Pursuit();
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

                bool HitWall = Physics.Raycast(transform.position, dirToTarget, dstToTarget, ObstacleMaskWithoutLowWall);
                bool HitLowWall = Physics.Raycast(transform.position, dirToTarget, dstToTarget, ObstackeMaskWithoutWall);

                if (HitLowWall)
                {
                    if(HitLowWall && theTarget.GetComponent<Human>() != null && !theTarget.GetComponent<Human>().isAccroupi)
                        HitLowWall = false;
                    else if(HitLowWall && theTarget.GetComponent<Human>() == null)
                        HitLowWall = false;
                }

                if (!HitWall && !HitLowWall)
                {
                    Vus.Add(theTarget);
                }
            }
        }

        Vus = Vus.OrderBy(w => Vector3.Distance(w.position, transform.position)).ToList();
    }

    ViewCastInfo viewCast(float globalAngle, LayerMask AffectedLayer)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, dir, out hit, ViewRadius, AffectedLayer))
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
            float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = viewCast(angle, ObstacleMask);
            if(i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThrehsold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast, ObstacleMask);
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

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
    }

    #endregion

    #region Patrouille et garde

    #region Editor

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
        Debug.Log(myNavMeshAgent.remainingDistance);
        if (myNavMeshAgent.remainingDistance < 0.1f)
        {
            if (!myNavMeshAgent.isStopped)
                myNavMeshAgent.isStopped = true;
            mySituation = Situation.None;
        }
    }

    void ForceLook()
    {
        Vector3 MyCostumPosition = ForwardRotationBase;
        if (mySituation == Situation.Interaction)
            MyCostumPosition = InteractionTarget;
        else if (mySituation == Situation.Interrogation)
            MyCostumPosition = PursuitLastPosition;
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
        myNavMeshAgent.speed = 3.5f;
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
                Destroy(Vus[0].gameObject);
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
        myNavMeshAgent.speed = 1f;
        PursuitLastPosition = transform.position + Vector3.forward;
    }

    #endregion

    void Death()
    {
        Destroy(myUI.gameObject);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (myNavMeshAgent == null)
            myNavMeshAgent = GetComponent<NavMeshAgent>();

        for (int i = 0; i < EtapesPatrouille.Count; i++)
        {
            if (myClasse == Classe.Basic || i + 1 < EtapesPatrouille.Count)
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
        }

        Gizmos.color = Color.red;

        if(EtapesPatrouilleSecondaire.Count > 0)
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
                if (myClasse == Classe.Basic || i + 1 < EtapesPatrouilleSecondaire.Count)
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
    }
}