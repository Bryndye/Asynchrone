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
    private CameraManager cm;
    private ManagerPlayers mp;

    public bool SetColor = false;
    [SerializeField] private whichPlayer _whichPlayer;
    private PlayerController _playerControlRef;
    private PlayerController _playerControlGet;

    [Space]
    public bool Pince;
    public bool Distributeur;
    [SerializeField] Transform _pointArrive;
    [HideInInspector] public bool ActivePince;

    [SerializeField] public bool activated;
    [SerializeField] private bool cinematic = true;


    public Transform[] Influence;
    [SerializeField] private GameObject _feedBackActivated;




    private void Awake() { 
        cm = CameraManager.Instance;
        mp = ManagerPlayers.Instance;

        if (_feedBackActivated != null)
            _feedBackActivated.SetActive(false);
    }


    private void Update()
    {
        CallPince();
    }











    public void CallEvent()
    {
        //Set which player has the right to use it
        _playerControlRef = _whichPlayer == whichPlayer.Human ? mp.pc1 : mp.pc2; 


        if (!activated && _playerControlRef == _playerControlGet)
        {
            Debug.Log("Event called");
            activated = true;

            for (int i = 0; i < Influence.Length; i++)
            {
                if (Influence[i] != null)
                {
                    Influence[i].gameObject.SetActive(!Influence[i].gameObject.activeSelf);
                    if (cm != null && cinematic)
                        cm.GetTargetPorte(Influence);
                }
            }

            if (_feedBackActivated != null)
                _feedBackActivated.SetActive(true);
        }
    }


    public void CallDistri() 
    {
        activated = true;

        if (mp.Rbt.DivStock <= 0 && !mp.onPlayer1)
        {
            //trigger Anim successfull
            mp.Rbt.DivStock = 1;
        }
        else
        {
            CanvasManager cm = CanvasManager.Instance;
            string[] dia = new string[1];

            if (mp.onPlayer1)
            {
                dia[0] = "Je ne peux pas l'utiliser. Vatrek devrait réussir.";
                cm.StartDiaEffect(dia);
            }
            else
            {
                dia[0] = "Je suis déjà rechargé à bloc!";
                cm.StartDiaEffect(dia);
            }
        }
    }


    public void CallPince()
    {
        //Set which player has the right to use it
        _playerControlRef = _whichPlayer == whichPlayer.Human ? mp.pc1 : mp.pc2;

        if (ActivePince && !activated && _playerControlRef == _playerControlGet)
        {
            Influence[0].position = Vector3.Lerp(Influence[0].transform.position, _pointArrive.position, 0.01f);

            if (Influence[0].position.y + 0.01f > _pointArrive.position.y)
            {
                activated = true;
            }
        }
    }








    public void SetPlayerController(PlayerController pcCalled) => _playerControlGet = pcCalled;



    private void OnDrawGizmos()
    {
        if (SetColor)
        {
            if (_feedBackActivated != null)
            {
                _feedBackActivated.GetComponent<Light>().color = _whichPlayer == whichPlayer.Human ? Color.cyan : Color.red;
            }
            /*
            MeshRenderer _mesh = GetComponent<MeshRenderer>();
            if (_mesh != null)
            {
                Color _color = _whichPlayer == whichPlayer.Human ? Color.cyan : Color.red;
                //Debug.Log(_mesh.material.GetColor("_Color"));
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                if (_mesh.material.HasProperty("_Color"))
                {
                    _mesh.material.SetColor("_Color", _color);
                }
            }*/
            SetColor = false;
        }
    }
}
