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
    public bool Pince;
    [SerializeField] Transform pointArrive;
    [HideInInspector] public bool ActivePince;


    public Transform[] Portes;
    [HideInInspector] public Vector3[] PosInitials;

    public bool Distributeur;
    [SerializeField]
    private GameObject feedBackActivated;

    [Header("Sounds")]
    string InteractionSoundName;


    private void Awake() { 
        cameraManager = CameraManager.Instance;
        managerPlayers = ManagerPlayers.Instance;
        SoundM = SoundManager.Instance;

        if (feedBackActivated != null)
            feedBackActivated.SetActive(false);
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

        if (Pince)
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



    public void CallActivePorte()
    {
        if (!Activated && PlayerControlRef == playerControlGet)
        {
            Activated = true;

            if(whichPlayer == whichPlayer.Human)
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
                        encafb.SetEncadrementColor();
                    }

                    if (InteractionSoundName != "")
                    {
                        SoundM.GetASound("Porte", transform);
                    }
                }
            }

            if (feedBackActivated != null)
                feedBackActivated.SetActive(true);
        }
    }


    public void CallDistri() 
    {
        if (!managerPlayers.RobotPlayer.HasDiversion && !managerPlayers.onPlayerHuman)
        {
            //trigger Anim successfull
            managerPlayers.RobotPlayer.HasDiversion = true;
            SoundM.GetASound("DiversionGet", transform);
        }
        else
        {
            CanvasManager canvasManager = CanvasManager.Instance;
            string[] dia = new string[1];

            if (managerPlayers.onPlayerHuman)
            {
                dia[0] = "Je ne peux pas l'utiliser. Vatrek devrait réussir.";
                canvasManager.StartDiaEffect(dia);
            }
            else
            {
                dia[0] = "Je suis déjà rechargé à bloc!";
                canvasManager.StartDiaEffect(dia);
            }

            SoundM.GetASound("DiversionFull", transform, true);
        }
    }

    bool done = false;
    public void CallPince()
    {
        if (ActivePince && !Activated && PlayerControlRef == playerControlGet)
        {
            if (!done)
            {
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
        if (feedBackActivated != null)
        {
            feedBackActivated.GetComponent<Light>().color = whichPlayer == whichPlayer.Human ? Color.yellow : Color.cyan;
        }
        if (TryGetComponent(out Outline outline))
        {
            outline.OutlineColor = whichPlayer == whichPlayer.Human ? Color.yellow : Color.cyan;
        }
    }
}
