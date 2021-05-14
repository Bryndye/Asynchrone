﻿using System.Collections;
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

    public bool SetColor = false;
    public whichPlayer whichPlayer;
    [SerializeField] 
    public bool activated;
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

    public bool Distributeur;
    [SerializeField]
    private GameObject _feedBackActivated;
    [Header("Sounds")]
    string InteractiveSoundName;
    string InteractionSoundName;


    private void Awake() { 
        cameraManager = CameraManager.Instance;
        managerPlayers = ManagerPlayers.Instance;
        SoundM = SoundManager.Instance;

        if (_feedBackActivated != null)
            _feedBackActivated.SetActive(false);

        SetUI();
    }

    private void SetUI()
    {
        myRenderer = GetComponent<Renderer>();
        if (myRenderer == null)
        {
            myRenderer = GetComponentInChildren<Renderer>();
        }

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
        //Set which player has the right to use it
        PlayerControlRef = whichPlayer == whichPlayer.Human ? managerPlayers.PlayerControllerHm : managerPlayers.PlayerCntrlerRbt;
    }

    public void SetPlayerController(PlayerController pcCalled) => playerControlGet = pcCalled;


    private void Update()
    {
        CallPince();
    }



    public void CallActivePorte()
    {
        if (!activated && PlayerControlRef == playerControlGet)
        {
            activated = true;

            if(whichPlayer == whichPlayer.Human)
                SoundM.GetASound("Button_Clic", transform);
            else if (whichPlayer == whichPlayer.Robot)
                SoundM.GetASound("Hack", transform);

            for (int i = 0; i < Portes.Length; i++)
            {
                if (Portes[i] != null)
                {
                    Portes[i].gameObject.SetActive(!Portes[i].gameObject.activeSelf);
                    if (cameraManager != null)
                        cameraManager.GetTargetPorte(Portes);

                    if (InteractionSoundName != "")
                    {
                        SoundM.GetASound("Porte", Portes[i].transform);
                    }
                }
            }

            if (_feedBackActivated != null)
                _feedBackActivated.SetActive(true);
        }
    }


    public void CallDistri() 
    {
        //activated = true;

        if (!managerPlayers.RobotPlayer.HasDiversion && !managerPlayers.onPlayerHuman)
        {
            //trigger Anim successfull
            managerPlayers.RobotPlayer.HasDiversion = true;
            Debug.Log("Distri ++");
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


    public void CallPince()
    {
        if (ActivePince && !activated && PlayerControlRef == playerControlGet)
        {
            Portes[0].position = Vector3.Lerp(Portes[0].transform.position, pointArrive.position, 0.01f);

            if (Portes[0].position.y + 0.01f > pointArrive.position.y)
            {
                activated = true;
            }
        }
    }



    private void OnDrawGizmos()
    {
        if (SetColor)
        {
            if (_feedBackActivated != null)
            {
                _feedBackActivated.GetComponent<Light>().color = whichPlayer == whichPlayer.Human ? Color.yellow : Color.cyan;
            }
            if (TryGetComponent(out Outline outline))
            {
                outline.OutlineColor = whichPlayer == whichPlayer.Human ? Color.yellow : Color.cyan;
            }
            SetColor = false;
        }
    }
}
