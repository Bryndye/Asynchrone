using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public whichPlayer myPlayer;

    //private enum interactionSelect
    //{
    //    interaction,
    //    trap,
    //    ia,
    //    none
    //}
    //private interactionSelect interSelected = interactionSelect.none;

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
            }
            UpdatePosSign();
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
            OnClickMouseR(true);
        }
        if (Input.GetKey(managerPlayers.InputMovement) && canMove)
        {
            OnClickMouseR(false);
        }
    }





    #region Mouse
    private bool canMove = true;
    private void OnClickMouseR(bool stay)
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
                if (stay)
                {
                    // play particle system
                    Instantiate(FeedbackClick, hit.point, transform.rotation);
                }

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




    #region Destination/Distance

    private void SetDesination(RaycastHit raycastHit, bool inter = false)
    {
        if (raycastHit.collider != null && inter && targetClickMouse != raycastHit.collider.gameObject)
        {
            targetClickMouse = raycastHit.collider.transform;
            //Debug.Log(targetClickMouse);
            ActiveSignOnInteraction();
        }
        else if(raycastHit.point != Vector3.zero)
        {
            targetClickMouse = null;
            NavPlayer.SetDestination(raycastHit.point);
            StopSignInteraction();
        }
        else
        {
            targetClickMouse = null;
            NavPlayer.SetDestination(transform.position);
            StopSignInteraction();
        }
    }

    //private void CheckWhichInteraction()
    //{
    //    if (targetClickMouse.TryGetComponent(out Interaction interaction))
    //    {
    //        interSelected = interactionSelect.interaction;
    //    }
    //    else if (targetClickMouse.TryGetComponent(out anAI ia))
    //    {
    //        interSelected = interactionSelect.ia;
    //    }
    //    else if (targetClickMouse.TryGetComponent(out trap_interaction trap))
    //    {
    //        interSelected = interactionSelect.trap;
    //    }
    //    else
    //    {
    //        interSelected = interactionSelect.none;
    //    }
    //}


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
                    switch (interaction.interType)
                    {
                        case Interaction.interactionType.Porte:
                            break;
                        case Interaction.interactionType.Distributeur:
                            break;
                        case Interaction.interactionType.Pince:
                            break;
                        default:
                            break;
                    }
                    if (interaction.Activated || interaction.PlayerControlRef != this)
                    {
                        SetDesination(raycastNull());
                        targetClickMouse = null;
                        return;
                    }

                    if (!managerPlayers.onPlayerHuman && interaction.interType == Interaction.interactionType.Distributeur)
                    {
                        interaction.Event();
                        //interaction.CallDistri();
                        SetAnim("Interaction", false, true);
                    }
                    else if (interaction.interType == Interaction.interactionType.Pince)
                    {
                        interaction.Event();
                        interaction.SetPlayerController(this);
                        //interaction.ActivePince = true;
                        SetAnim("Interaction", false, true);
                    }
                    else
                    {
                        interaction.Event();
                        interaction.SetPlayerController(this);
                        //interaction.CallActivePorte();
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
                StopSignInteraction();
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
    private void SetAnim(string paramName, bool active, bool isTrigger)
    {
        if (AnimPlayer)
        {
            foreach (AnimatorControllerParameter param in AnimPlayer.parameters)
            {
                if (param.name == paramName)
                {
                    if (isTrigger)
                    {
                        AnimPlayer.SetTrigger(paramName);
                    }
                    else
                    {
                        AnimPlayer.SetBool(paramName, active);
                    }
                }
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



    #region Gestion Interaction Sign
    [SerializeField]
    private GameObject signOnClickInteraction;
    private void ActiveSignOnInteraction()
    {
        if (targetClickMouse.TryGetComponent(out Interaction interaction))
        {
            if (interaction.whichPlayer == myPlayer)
            {
                signOnClickInteraction.SetActive(true);
            }
            else
            {
                signOnClickInteraction.SetActive(false);
            }
        }
        else if (myPlayer == whichPlayer.Human && targetClickMouse.TryGetComponent(out anAI ia))
        {
            signOnClickInteraction.SetActive(true);
        }
        else if (myPlayer == whichPlayer.Robot && targetClickMouse.TryGetComponent(out trap_interaction trap))
        {
            signOnClickInteraction.SetActive(true);
        }
        else
        {
            signOnClickInteraction.SetActive(false);
        }

    }

    private void UpdatePosSign()
    {
        if (signOnClickInteraction.activeSelf)
        {
            signOnClickInteraction.transform.position = targetClickMouse.position + new Vector3(0, 0.1f, 0);
        }
    }

    private void StopSignInteraction() => signOnClickInteraction.SetActive(false);


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

