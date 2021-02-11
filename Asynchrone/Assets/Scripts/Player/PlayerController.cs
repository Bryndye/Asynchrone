using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    #region var
    public Animator anim;
    [HideInInspector] public NavMeshAgent nav;
    ManagerPlayers mP;
    SpawnMANAGER sm;
    public bool CanPlay;

    [SerializeField] LayerMask ingoreDiv;
    [SerializeField] LayerMask ingorePlayers;
    [SerializeField] Transform targetInteraction;

    public bool InCinematic;
    GameObject fd_faisceau;
    #endregion

    private void Awake()
    {
        mP = ManagerPlayers.Instance;
        sm = SpawnMANAGER.Instance;
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
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
            if (!mP.onPlayer1 && mP.Rbt.BackToHuman)
            {
                mP.Rbt.BackToHuman = false;
            }
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
                        SetDesination(hit, false, true);
                    }
                }
            }
        }
    }

    #region Mousse/Interaction element decor
    [SerializeField] private bool canMove = true;
    private void OnClickMouseR()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (mP.onPlayer1 && mP.Hm.canDiv && mP.Hm.DivStock > 0)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ingoreDiv))
            {
                mP.Hm.CreateDiversion(hit);
                canMove = false;
                Invoke(nameof(OnClickStay),0.6f);
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ingorePlayers))
            {
                if (nav.CalculatePath(hit.point, nav.path))
                {
                    if (CanReachPosition(hit.point))
                    {
                        SetDesination(hit, false, true);
                    }
                }
                if (hit.collider.tag == "Interaction")
                {
                    SetDesination(hit, true, true);
                }
                /*
                anAI ia = hit.collider.GetComponent<anAI>();
                if (ia != null && !mP.onPlayer1)
                {
                    SetDesination(hit, true, true);
                    Debug.Log("La");
                }*/
            }
        }
    }
    private void OnClickStay()
    {
        if (!canMove)
        {
            canMove = true;
        }
    }

    public bool CanReachPosition(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        nav.CalculatePath(position, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    private void SetDesination(RaycastHit t, bool inter, bool active)
    {
        if (!fd_faisceau)
        {
            fd_faisceau = Instantiate(Resources.Load<GameObject>("Feedback/Player/Particle_loading"));
        }
        fd_faisceau.SetActive(active);
        if (t.point != Vector3.zero)
        {
            nav.SetDestination(t.point);
        }
        else
        {
            nav.SetDestination(transform.position);
        }

        if (inter)
        {
            //print("inter");
            targetInteraction = t.collider.transform;
            fd_faisceau.transform.position = t.collider.transform.position;
        }
        else
        {
            fd_faisceau.transform.position = t.point;
        }
    }
    private void CheckDisInteraction()
    {
        if (targetInteraction != null)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetInteraction.position.x, targetInteraction.position.z)) < 1.5f)
            {
                Interaction iem = targetInteraction.GetComponent<Interaction>();
                if (iem != null)
                {
                    iem.CallEvent();
                    SetDesination(raycastNull(), false, false);
                }

                anAI ia = targetInteraction.GetComponent<anAI>();

                if (ia != null && !mP.onPlayer1)
                {
                    ia.Death();
                    Debug.Log("T MORT");
                }
                targetInteraction = null;
                SetDesination(raycastNull(), false, false);
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
                SetDesination(raycastNull(), false, false);
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
        sm.Respawn();
    }
}

