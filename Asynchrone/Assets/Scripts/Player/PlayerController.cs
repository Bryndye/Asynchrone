using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    #region var
    [HideInInspector] 
    public Animator AnimPlayer;
    [HideInInspector] 
    public NavMeshAgent NavPlayer;
    private ManagerPlayers managerPlayers;
    private SpawnMANAGER spawnManager;
    SoundManager SM;
    //[HideInInspector] 
    public bool CanPlay = true;

    [Space]
    [SerializeField] 
    LayerMask ingoreDiv;
    [SerializeField] 
    LayerMask ingorePlayers;
    public GameObject signPlayerControlled;
    Transform targetClickMouse;

    [Space]
    public bool InCinematic;
    public GameObject FeedbackClick;

    float time;
    #endregion




    private void Awake()                                //AWAKE
    {
        managerPlayers = ManagerPlayers.Instance;
        spawnManager = SpawnMANAGER.Instance;
        SM = SoundManager.Instance;
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




    //GetKeyDown pour les Interactions et le GetKey pour deplacement
    private void InputManager()
    {
        if (Input.GetKeyDown(managerPlayers.InputMovement))
        {
            OnClickMouseR();
        }
        if (Input.GetKey(managerPlayers.InputMovement) && canMove)
        {            
            time += Time.deltaTime;

            if (time > 0.4f)
            {
                time = 0;
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
    }





    #region Mouse/Interaction element decor
    private bool canMove = true;
    private void OnClickMouseR()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!managerPlayers.onPlayerHuman && managerPlayers.RobotPlayer.CanDiv && managerPlayers.RobotPlayer.HasDiversion)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ingoreDiv))
            {
                managerPlayers.RobotPlayer.CreateDiversion(hit);
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

                //check s'il y a une interaction pour ensuite appeler la fonction de celle-ci
                if (hit.collider.tag == "Interaction")
                {
                    SetDesination(hit, true);
                }
                else if (NavPlayer.CalculatePath(hit.point, NavPlayer.path))
                {
                    SetDesination(hit, false);
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
            //Debug.Log(targetClickMouse);
        }
        else if(raycastHit.point != Vector3.zero)
        {
            targetClickMouse = null;
            NavPlayer.SetDestination(raycastHit.point);
        }
        else
        {
            targetClickMouse = null;
            NavPlayer.SetDestination(transform.position);
        }
    }


    [Space]
    [SerializeField]
    private float minDistanceInteraction = 1f;
    private void CheckDisInteraction()
    {
        if (targetClickMouse != null)
        {
            NavPlayer.SetDestination(targetClickMouse.position);

            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetClickMouse.position.x, targetClickMouse.position.z)) < minDistanceInteraction)
            {

                if (targetClickMouse.TryGetComponent(out Interaction interaction))
                {
                    if (interaction.activated || interaction.PlayerControlRef != this)
                    {
                        SetDesination(raycastNull());
                        targetClickMouse = null;
                        return;
                    }

                    if (!managerPlayers.onPlayerHuman && interaction.Distributeur)
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
                    if (managerPlayers.onPlayerHuman && ai.Killable())
                    {
                        SetDesination(raycastNull());
                        SetAnim("Attack", false, true);
                        SM.GetASound("Taser", transform);
                        ai.Death();
                    }
                    CanPlay = false;
                    SetDesination(raycastNull());
                }

                else if (targetClickMouse.TryGetComponent(out trap_interaction trapInter))
                {
                    if (managerPlayers.PlayerCntrlerRbt == this)
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
            }
            //Debug.Log(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(finalDestination.x, finalDestination.z)) + " " + transform.name);
        }
    }

    private void CrouchedAnim()
    {
        if (managerPlayers.onPlayerHuman)
        {
            SetAnim("Crouched", managerPlayers.HumanPlayer.isAccroupi, false);
        }
    }
    public void DivAnim()
    {
        SetAnim("Div", true, true);
    }

    [SerializeField]
    private float cooldownInteraction = 1f;
    private void StopPlayerWhenAction()
    {
        if (managerPlayers.onPlayerHuman && managerPlayers.PlayerControllerHm == this)
        {
            CanPlay = true;
        }
        else if (!managerPlayers.onPlayerHuman && managerPlayers.PlayerCntrlerRbt == this)
        {
            CanPlay = true;
        }
        else
        {
            CanPlay = false;   
        }
    }

    #endregion




    public void Death()
    {
        InCinematic = true;
        SetDesination(raycastNull());
        //myAnim.SetTrigger("Mort")

        targetClickMouse = null;
        spawnManager.mySpawnSituation = SpawnSituation.DeathProcess;
        spawnManager.Respawn();
    }
}

