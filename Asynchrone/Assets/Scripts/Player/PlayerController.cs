using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    #region var
    [SerializeField] Animator anim;
    [HideInInspector] public NavMeshAgent nav;
    ManagerPlayers mP;
    public bool CanPlay;

    [SerializeField] LayerMask ingoreEvent;
    Vector3 finalDestination;
    Transform targetInteraction;

    public bool InCinematic;
    GameObject fd_faisceau;
    #endregion

    private void Awake()
    {
        mP = ManagerPlayers.Instance;
        nav = GetComponent<NavMeshAgent>();

        finalDestination = transform.position;
    }

    void Update()
    {
        if (!InCinematic)
        {
            if (CanPlay)
            {
                InputManager();
                CheckDisInteraction();
            }
            WalkAnim();
            CrouchedAnim();
        }
    }

    private void InputManager()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            OnClickMouseR();
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            OnClickMouseL();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && !mP.onPlayer1 && mP.Rbt.BackToHuman )
        {
            mP.Rbt.BackToHuman = false;
            OnClickMouseR();
        }
    }

    #region Interaction element decor
    private void OnClickMouseL()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Interaction")
            {
                SetDesination(hit, true, true);
            }

            
            anAI ia = hit.collider.GetComponent<anAI>();
            if (ia != null && !mP.onPlayer1)
            {
                SetDesination(hit, true, true);
            }
        }
    }

    private void SetDesination(RaycastHit t, bool inter, bool active)
    {
        if (!fd_faisceau)
        {
            fd_faisceau = Instantiate(Resources.Load<GameObject>("Feedback/Player/Particle_loading"));
        }
        if (t.point != Vector3.zero)
        {
            nav.SetDestination(t.point);
            finalDestination = t.point;
        }
        else
        {
            nav.SetDestination(transform.position);
            finalDestination = transform.position;
        }
        if (inter)
        {
            print("inter");
            targetInteraction = t.collider.transform;
            fd_faisceau.transform.position = t.collider.transform.position;
        }
        else
        {
            fd_faisceau.transform.position = t.point;
        }

        print("mache");
        fd_faisceau.SetActive(active);
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
                    RaycastHit it = new RaycastHit();
                    SetDesination(it,false, false);
                }

                anAI ia = targetInteraction.GetComponent<anAI>();

                if (ia != null && !mP.onPlayer1)
                {
                    Debug.Log("T MORT");
                }
                targetInteraction = null;
                RaycastHit hit = new RaycastHit();
                SetDesination(hit, false, false);
            }
        }
        //Debug.Log(targetInteraction);
    }

    #endregion


    #region MoveR
    private void OnClickMouseR()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (nav.CalculatePath(hit.point, nav.path))
            {
                
                if (hit.collider.gameObject.layer != 10)
                {
                    if (CanReachPosition(hit.point))
                    {
                        SetDesination(hit, false, true);
                    }
                }
            }
        }
    }

    public bool CanReachPosition(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        nav.CalculatePath(position, path);
        return path.status == NavMeshPathStatus.PathComplete;
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

        if (finalDestination != null)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(finalDestination.x, finalDestination.z)) > 0.1f)
            {
                SetAnim("Walking", true, false);
                //Debug.Log("je marche " + transform.name);
            }
            else
            {
                RaycastHit it = new RaycastHit();
                SetAnim("Walking", false, false);
                SetDesination(it, false, false);
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
}

/*
    /*
    private void SetDesination(RaycastHit t, bool inter)
    {
        if (t.point != Vector3.zero)
        {
            nav.SetDestination(t.point);
            finalDestination = t.point;
        }
        else
        {
            nav.SetDestination(transform.position);
            finalDestination = transform.position;
        }

        if (inter)
        {
            targetInteraction = t.collider.transform;
        }
    }

    private void Faisceau_(RaycastHit t, bool active, bool interaction)
    {
        if (!fd_faisceau)
        {
            fd_faisceau = Instantiate(Resources.Load<GameObject>("Feedback/Player/Particle_loading"));
        }
        if (interaction)
        {
            fd_faisceau.transform.position = t.collider.transform.position;
        }
        else
        {
            fd_faisceau.transform.position = t.point;
        }
        fd_faisceau.SetActive(active);
**/
