using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum whichPlayer
{
    Human,
    Robot
}
public class Interaction : MonoBehaviour
{
    public enum interactionType
    {
        Porte,
        Distributeur,
        Pince
    }
    public interactionType interType;
    [SerializeField]
    private bool reutilisable;

    private CameraManager cameraManager;
    private ManagerPlayers managerPlayers;
    SoundManager SoundM;

    public whichPlayer whichPlayer;
    [SerializeField] 
    public bool Activated;
    [SerializeField]
    private GameObject myUI;
    [SerializeField]
    private Renderer myRenderer;
    [Space]


    [HideInInspector]
    public PlayerController PlayerControlRef;
    private PlayerController playerControlGet;

    [Header("Pince")]
    [SerializeField] Transform pointArrive;
    [HideInInspector] public bool ActivePince;


    public Transform[] Portes;
    [HideInInspector] public Vector3[] PosInitials;

    [SerializeField]
    private GameObject feedBackActivated;

    [Header("Sounds")]
    string InteractionSoundName;


    private void Awake() { 
        cameraManager = CameraManager.Instance;
        managerPlayers = ManagerPlayers.Instance;
        SoundM = SoundManager.Instance;

        if (feedBackActivated != null)
            feedBackActivated.SetActive(true);
    }

    private void SetUI()
    {
        if (whichPlayer == whichPlayer.Human)
        {
            myUI = Instantiate(Resources.Load<GameObject>("UI/Following/Interaction Humain"));
        }
        else
        {
            myUI = Instantiate(Resources.Load<GameObject>("UI/Following/Interaction Robot"));
        }
        myUI.GetComponent<InteractionUI>().Declaration(myRenderer);
    }

    private void Start()
    {
        PlayerControlRef = whichPlayer == whichPlayer.Human ? managerPlayers.PlayerControllerHm : managerPlayers.PlayerCntrlerRbt;
        SetUI();
        SetColorOutline();

        if (interType == interactionType.Pince)
        {
            PosInitials = new Vector3[Portes.Length];
            for (int i = 0; i < Portes.Length; i++)
            {
                PosInitials[i] = Portes[i].position;
            }
        }
    }

    public void SetPlayerController(PlayerController pcCalled) => playerControlGet = pcCalled;


    public void InteractionDone(bool active)
    {
        myUI.SetActive(active);
        if (GetComponentInChildren<Outline>() != null)
        {
            GetComponentInChildren<Outline>().enabled = active;
        }
    }

    private void Update()
    {
        CallPince();
        if (Activated)
        {
            InteractionDone(false);
        }
        else if(myUI.activeSelf)
        {
            InteractionDone(true);
        }
    }

    public void Event()
    {
        switch (interType)
        {
            case interactionType.Porte:
                Invoke(nameof(CallActivePorte), 0.4f);
                break;
            case interactionType.Distributeur:
                Invoke(nameof(CallDistri), 0.2f);
                break;
            case interactionType.Pince:
                Invoke(nameof(SetPince), 0.4f);
                break;
            default:
                break;
        }
    }

    public void CallActivePorte()
    {
        if (!Activated && PlayerControlRef == playerControlGet)
        {
            if (!reutilisable)
            {
                Activated = true;
                InteractionDone(false);
            }

            if (whichPlayer == whichPlayer.Human)
                SoundM.GetASound("Button_Clic", transform);
            else if (whichPlayer == whichPlayer.Robot)
                SoundM.GetASound("Hack", transform);

            for (int i = 0; i < Portes.Length; i++)
            {
                if (Portes[i] != null)
                {
                    Portes[i].gameObject.SetActive(!Portes[i].gameObject.activeSelf);
                    cameraManager.GetTargetPorte(Portes);

                    if (Portes[i].parent.TryGetComponent(out EncadrementFeedback encafb))
                    {
                        encafb.SetEncadrementColor(!Portes[i].gameObject.activeSelf);
                        encafb.AnimDoor(!Portes[i].gameObject.activeSelf);
                    }

                    if (InteractionSoundName != "")
                    {
                        SoundM.GetASound("Porte", transform);
                    }
                }
            }

            if (feedBackActivated != null)
                feedBackActivated.SetActive(false);
        }
    }

    private void DesactivatePorte()
    {
        //porte.gameObject.SetActive(!porte.activeSelf);
        for (int i = 0; i < Portes.Length; i++)
        {
            Portes[i].gameObject.SetActive(!Portes[i].gameObject.activeSelf);
            Debug.Log("lol");
        }
    }



    public void CallDistri() 
    {
        if (!managerPlayers.RobotPlayer.HasDiversion && !managerPlayers.onPlayerHuman)
        {
            //trigger Anim successfull
            managerPlayers.RobotPlayer.HasDiversion = true;
            SoundM.GetASound("DiversionGet", transform);
            Destroy(Instantiate(feedBackActivated, transform.position, transform.rotation), 0.5f );
        }
        else
        {
            CanvasManager canvasManager = CanvasManager.Instance;
      
            //dia[0] = "Je suis déjà rechargé à bloc!";
            //canvasManager.StartDiaEffect(dia);

            SoundM.GetASound("DiversionFull", transform, true);
        }
    }




    bool done = false;
    private void SetPince()
    {
        ActivePince = true;
    }
    public void CallPince()
    {
        if (ActivePince && !Activated && PlayerControlRef == playerControlGet)
        {
            if (!done)
            {
                InteractionDone(false);
                done = true;
                cameraManager.GetTargetPorte(Portes);
            }
            Portes[0].position = Vector3.Lerp(Portes[0].transform.position, pointArrive.position, 0.01f);
            if (Portes[0].position.y > pointArrive.position.y - 0.1f)
            {
                Activated = true;
            }
        }
    }






    private void OnDrawGizmosSelected()
    {
        SetColorOutline();
    }

    private void SetColorOutline()
    {
        if (feedBackActivated != null && interType != interactionType.Distributeur)
        {
            feedBackActivated.GetComponent<Light>().color = whichPlayer == whichPlayer.Human ? Color.yellow : Color.cyan;
        }
        if (TryGetComponent(out Outline outline))
        {
            outline.OutlineColor = whichPlayer == whichPlayer.Human ? Color.yellow : Color.cyan;
        }
    }
}
