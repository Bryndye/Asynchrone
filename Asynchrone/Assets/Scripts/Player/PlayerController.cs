using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    #region var
    [HideInInspector] public Animator AnimPlayer;
    [HideInInspector] public NavMeshAgent NavPlayer;
    private ManagerPlayers managerPlayer;
    private SpawnMANAGER spawnManager;
    [HideInInspector] public bool CanPlay = true;

    [Space]
    [SerializeField] LayerMask ingoreDiv;
    [SerializeField] LayerMask ingorePlayers;
    [HideInInspector] Transform targetClickMouse;

    [Space]
    public bool InCinematic;
    public GameObject FeedbackClick;
    #endregion






    private void Awake()                                //AWAKE
    {
        managerPlayer = ManagerPlayers.Instance;
        spawnManager = SpawnMANAGER.Instance;
        NavPlayer = GetComponent<NavMeshAgent>();

        AnimPlayer = GetComponentInChildren<Animator>();
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
                if (NavPlayer.CalculatePath(hit.point, NavPlayer.path))
                {
                    SetDesination(hit, false);
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
        if (!managerPlayer.onPlayerHuman && managerPlayer.RobotPlayer.canDiv && managerPlayer.RobotPlayer.DivStock > 0)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ingoreDiv))
            {
                managerPlayer.RobotPlayer.CreateDiversion(hit);
                canMove = false;
                Invoke(nameof(CanMove),0.6f);
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ingorePlayers))
            {
                // play particle system
                Instantiate(FeedbackClick, hit.point, transform.rotation); 

                if (NavPlayer.CalculatePath(hit.point, NavPlayer.path))
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

    private void SetDesination(RaycastHit raycastHit, bool inter = false)
    {
        if (raycastHit.collider != null && inter)
        {
            targetClickMouse = raycastHit.collider.transform;
            NavPlayer.SetDestination(targetClickMouse.position);
        }
        else if(raycastHit.point != Vector3.zero)
        {
            NavPlayer.SetDestination(raycastHit.point);
        }
        else
        {
            NavPlayer.SetDestination(transform.position);
        }
    }



    private void CheckDisInteraction()
    {
        if (targetClickMouse != null)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetClickMouse.position.x, targetClickMouse.position.z)) < 1.8f)
            {

                if (targetClickMouse.TryGetComponent(out Interaction interaction))
                {
                    bool isHuman = interaction._whichPlayer == whichPlayer.Human;

                    if (interaction.activated)
                    {
                        targetClickMouse = null;
                        return;
                    }

                    if (!managerPlayer.onPlayerHuman && interaction.Distributeur)
                    {
                        interaction.CallDistri();
                        SetAnim("Interaction", false, true);
                    }
                    else if (interaction.Pince)
                    {
                        interaction.SetPlayerController(this);
                        interaction.ActivePince = true;
                        SetAnim("Interaction", false, true);
                    }
                    else
                    {
                        interaction.SetPlayerController(this);
                        interaction.CallActivePorte();
                        SetAnim("Interaction", false, true);
                    }
                    CanPlay = false;
                    SetDesination(raycastNull());
                }

                else if (targetClickMouse.TryGetComponent(out anAI ai))
                {
                    if (managerPlayer.onPlayerHuman && ai.Killable())
                    {
                        SetDesination(raycastNull());
                        SetAnim("Attack", false, true);
                        ai.Death();
                    }
                    CanPlay = false;
                    SetDesination(raycastNull());
                }

                else if (targetClickMouse.TryGetComponent(out trap_interaction trapInter))
                {
                    if (!managerPlayer.onPlayerHuman)
                    {
                        trapInter.Called();
                    }
                }
                Invoke(nameof(StopPlayerWhenAction), cooldownInteraction);
                targetClickMouse = null;
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
        if (AnimPlayer)
        {
            if (trigger)
            {
                AnimPlayer.SetTrigger(var);
            }
            else
            {
                AnimPlayer.SetBool(var, active);
            }
        }
    }
    private void WalkAnim()
    {
        if (NavPlayer.destination != null)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(NavPlayer.destination.x, NavPlayer.destination.z)) > 0.1f)
            {
                SetAnim("Walking", true, false);
            }
            else
            {
                SetAnim("Walking", false, false);
                SetDesination(raycastNull(), false);
            }
            //Debug.Log(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(finalDestination.x, finalDestination.z)) + " " + transform.name);
        }
    }

    private void CrouchedAnim()
    {
        if (managerPlayer.onPlayerHuman)
        {
            SetAnim("Crouched", managerPlayer.HumanPlayer.isAccroupi, false);
        }
    }
    public void DivAnim()
    {
        SetAnim("Div", true, true);
    }

    [Space]
    [SerializeField]
    private float cooldownInteraction = 1f;
    private void StopPlayerWhenAction()
    {
        CanPlay = true;
    }

    #endregion




    public void Death()
    {
        targetClickMouse = null;
        spawnManager.mySpawnSituation = SpawnSituation.DeathProcess;
        spawnManager.Respawn();
    }
}

