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

    public bool SetColor = false;
    public whichPlayer _whichPlayer;
    private PlayerController _playerControlRef;
    private PlayerController _playerControlGet;

    [Space]
    public bool Pince;
    public bool Distributeur;
    [SerializeField] Transform _pointArrive;
    [HideInInspector] public bool ActivePince;

    [SerializeField] public bool activated;


    public Transform[] Portes;
    [SerializeField] private GameObject _feedBackActivated;

    [Header("Sound")]
    public string SoundName;


    private void Awake() { 
        cameraManager = CameraManager.Instance;
        managerPlayers = ManagerPlayers.Instance;
        SoundM = SoundManager.Instance;

        if (_feedBackActivated != null)
            _feedBackActivated.SetActive(false);
    }

    public void SetPlayerController(PlayerController pcCalled) => _playerControlGet = pcCalled;


    private void Update()
    {
        CallPince();
    }



    public void CallActivePorte()
    {
        //Set which player has the right to use it
        _playerControlRef = _whichPlayer == whichPlayer.Human ? managerPlayers.PlayerControllerHm : managerPlayers.PlayerCntrlerRbt; 


        if (!activated && _playerControlRef == _playerControlGet)
        {
            //Debug.Log("Event called");
            activated = true;

            if(SoundName != "")
            {
                SoundM.GetASound(SoundName, transform);
            }

            for (int i = 0; i < Portes.Length; i++)
            {
                if (Portes[i] != null)
                {
                    Portes[i].gameObject.SetActive(!Portes[i].gameObject.activeSelf);
                    if (cameraManager != null)
                        cameraManager.GetTargetPorte(Portes);
                }
            }

            if (_feedBackActivated != null)
                _feedBackActivated.SetActive(true);
        }
    }


    public void CallDistri() 
    {
        activated = true;

        if (managerPlayers.RobotPlayer.DivStock <= 0 && !managerPlayers.onPlayerHuman)
        {
            //trigger Anim successfull
            managerPlayers.RobotPlayer.DivStock = 1;
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
        }
    }


    public void CallPince()
    {
        //Set which player has the right to use it
        _playerControlRef = _whichPlayer == whichPlayer.Human ? managerPlayers.PlayerControllerHm : managerPlayers.PlayerCntrlerRbt;

        if (ActivePince && !activated && _playerControlRef == _playerControlGet)
        {
            Portes[0].position = Vector3.Lerp(Portes[0].transform.position, _pointArrive.position, 0.01f);

            if (Portes[0].position.y + 0.01f > _pointArrive.position.y)
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
                _feedBackActivated.GetComponent<Light>().color = _whichPlayer == whichPlayer.Human ? Color.cyan : Color.red;
            }
            SetColor = false;
        }
    }
}
