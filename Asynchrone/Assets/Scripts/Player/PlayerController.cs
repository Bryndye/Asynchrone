using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    #region var
    [HideInInspector] public Animator anim;
    [HideInInspector] public NavMeshAgent nav;
    private ManagerPlayers mP;
    private SpawnMANAGER sm;
    [HideInInspector] public bool CanPlay;

    [Space]
    [SerializeField] LayerMask ingoreDiv;
    [SerializeField] LayerMask ingorePlayers;
    [HideInInspector] Transform targetInteraction;
    private GameObject targetAI = null;

    [Space]
    public bool InCinematic;
    public GameObject feedbackClick;
    public GameObject fd_faisceau;
    #endregion






    private void Awake()                                //AWAKE
    {
        mP = ManagerPlayers.Instance;
        sm = SpawnMANAGER.Instance;
        nav = GetComponent<NavMeshAgent>();

        anim = GetComponentInChildren<Animator>();
    }

    void Update()                                       //UPDATE
    {
        if (!InCinematic)
        {
            if (CanPlay)
            {
                InputManager();
                /*
                if (Input.GetKeyDown(KeyCode.T))
                {
                    sm.Respawn();
                }*/
            }
            CheckDisInteraction();
            WalkAnim();
            CrouchedAnim();
        }
    }





    private void InputManager()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            OnClickMouseR();
        }
        if (Input.GetKey(KeyCode.Mouse1) && canMove)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ingorePlayers))
            {
                if (nav.CalculatePath(hit.point, nav.path))
                {
                    if (CanReachPosition(hit.point))
                    {
                        SetDesination(hit, false);
                    }
                }
            }
        }
    }





    #region Mouse/Interaction element decor
    [SerializeField] private bool canMove = true;
    private void OnClickMouseR()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!mP.onPlayer1 && mP.Rbt.canDiv && mP.Rbt.DivStock > 0)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ingoreDiv))
            {
                mP.Rbt.CreateDiversion(hit);
                canMove = false;
                Invoke(nameof(CanMove),0.6f);
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ingorePlayers))
            {
                // play particle system
                Instantiate(feedbackClick, hit.point, transform.rotation); 

                if (nav.CalculatePath(hit.point, nav.path))
                {
                    //Debug.Log("New Path");
                    SetDesination(hit, false);
                }

                //check s'il y a une interaction pour ensuite appeler la fonction de celle-ci
                if (hit.collider.tag == "Interaction")
                {
                    SetDesination(hit, true);
                    //Debug.Log("New Interaction Path");
                }
            }
        }
    }

    private void CanMove() => canMove = true;

    #endregion




    #region Position/Destination
    public bool CanReachPosition(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        nav.CalculatePath(position, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }


    private void SetDesination(RaycastHit raycastHit, bool inter = false)
    {
        //Debug.Log("Interaction " + inter);
        if (raycastHit.collider != null && inter)
        {
            anAI ia = raycastHit.collider.GetComponent<anAI>();

            if (ia != null)
            {
                targetAI = raycastHit.collider.gameObject;
            }
            else
            {
                nav.SetDestination(raycastHit.collider.transform.position);
            }

            targetInteraction = raycastHit.collider.transform;
            //Debug.Log("Destination GameObject: "+ raycastHit.collider.gameObject.name);
        }
        else if(raycastHit.point != Vector3.zero)
        {
            nav.SetDestination(raycastHit.point);
            //Debug.Log("Path to the point");
        }
        else
        {
            nav.SetDestination(transform.position);
        }
    }



    private void CheckDisInteraction()
    {
        if (targetAI != null)
        {
            nav.destination = targetAI.transform.position;
        }

        if (targetInteraction != null)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetInteraction.position.x, targetInteraction.position.z)) < 1.8f)
            {
                Interaction iem = targetInteraction.GetComponent<Interaction>();

                if (iem != null)
                {
                    SetAnim("Interaction", false, true);
                    SetDesination(raycastNull());

                    if (!mP.onPlayer1 && iem.Distributeur)
                    {
                        iem.CallDistri();
                    }
                    else if (iem.Pince)
                    {
                        iem.SetPlayerController(this);
                        iem.ActivePince = true;
                    }
                    else
                    {
                        iem.SetPlayerController(this);
                        iem.CallEvent();
                    }
                }

                anAI ia = null;
                if (targetAI != null)
                    ia = targetAI.GetComponent<anAI>();

                if (ia != null && mP.onPlayer1 &&ia.Killable())
                {
                    SetDesination(raycastNull());
                    SetAnim("Attack", false, true);
                    ia.Death();
                    targetAI = null;
                }

                trap_interaction ti = targetInteraction.GetComponent<trap_interaction>();
                if (ti != null && !mP.onPlayer1)
                {
                    ti.Called();
                }
                targetInteraction = null;
            }
        }
    }

    private RaycastHit raycastNull()
    {
        RaycastHit hit = new RaycastHit();
        return hit;
    }
    

    #endregion






    #region AnimManager
    private void SetAnim(string var, bool active, bool trigger)
    {
        if (anim)
        {
            if (trigger)
            {
                anim.SetTrigger(var);
            }
            else
            {
                anim.SetBool(var, active);
            }
        }
    }
    private void WalkAnim()
    {
        if (nav.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.Log("path completed");
        }

        if (nav.destination != null)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(nav.destination.x, nav.destination.z)) > 0.1f)
            {
                SetAnim("Walking", true, false);
                //Debug.Log("je marche " + transform.name);
            }
            else
            {
                SetAnim("Walking", false, false);
                SetDesination(raycastNull(), false);
                //Debug.Log("idle " + transform.name);
            }
            //Debug.Log(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(finalDestination.x, finalDestination.z)) + " " + transform.name);
        }
    }

    private void CrouchedAnim()
    {
        if (mP.onPlayer1)
        {
            if (mP.Hm.isAccroupi)
            {
                SetAnim("Crouched", true, false);
            }
            else
            {
                SetAnim("Crouched", false, false);
            }
        }
    }
    public void DivAnim()
    {
        SetAnim("Div", true, true);
    }
    #endregion





    public void Death()
    {
        targetAI = null;
        sm.Respawn();
    }
}

