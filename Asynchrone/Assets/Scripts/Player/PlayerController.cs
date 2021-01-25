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
                nav.SetDestination(hit.point);
                finalDestination = hit.point;
                targetInteraction = hit.collider.transform;
            }

            anAI ia = hit.collider.GetComponent<anAI>();
            if (ia != null && !mP.onPlayer1)
            {
                nav.SetDestination(hit.point);
                finalDestination = hit.point;
                targetInteraction = hit.collider.transform;
            }
        }
    }
    private void CheckDisInteraction()
    {
        if (targetInteraction != null)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetInteraction.position.x, targetInteraction.position.z)) < 1)
            {
                Interaction iem = targetInteraction.GetComponent<Interaction>();
                if (iem != null)
                {
                    iem.CallEvent();
                }

                anAI ia = targetInteraction.GetComponent<anAI>();
                if (ia != null && !mP.onPlayer1)
                {
                    Debug.Log("T MORT");
                }

                targetInteraction = null;
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
                        nav.SetDestination(hit.point);
                        finalDestination = hit.point;
                        Faisceau_(hit.point, true);
                    }
                }
            }
        }
    }
    private void Faisceau_(Vector3 v, bool active)
    {
        if (!fd_faisceau)
        {
            fd_faisceau = Instantiate(Resources.Load<GameObject>("Feedback/Player/Particle_loading"));
        }
        fd_faisceau.SetActive(active);
        fd_faisceau.transform.position = v;
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
                SetAnim("Walking", false, false);
                Faisceau_(Vector3.zero, false);
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
                //anim.SetBool("Crouched", true);
            }
            else
            {
                //anim.SetBool("Crouched", false);
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
