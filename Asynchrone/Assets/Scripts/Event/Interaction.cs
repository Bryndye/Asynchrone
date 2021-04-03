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

    [SerializeField] private whichPlayer _whichPlayer;
    private PlayerController _pc;

    public bool distributeur;
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




    public void CallEvent(PlayerController pcCalled)
    {
        //Set which player has the right to use it
        _pc = _whichPlayer == whichPlayer.Human ? mp.pc1 : mp.pc2; 


        if (!activated && _pc == pcCalled)
        {
            //Debug.Log("Event called");
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

        if (mp.Rbt.DivStock <= 0)
        {
            //trigger Anim successfull
            mp.Rbt.DivStock = 1;
        }
        else
        {
            CanvasManager cm = CanvasManager.Instance;
            string[] dia = new string[1];
            dia[0] = "Je suis déjà rechagé à bloc!";
            cm.StartDiaEffect(dia, null);
            //trigger anim cancel = texte ta mere
        }
    }
}
