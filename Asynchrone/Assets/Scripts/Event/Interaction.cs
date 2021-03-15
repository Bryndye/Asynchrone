﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    private CameraManager cm;
    private ManagerPlayers mp;

    public bool distributeur;
    private bool activated;
    [SerializeField] private bool cinematic = false;


    public Transform[] Influence;





    private void Awake() { 
        cm = CameraManager.Instance;
        mp = ManagerPlayers.Instance;
    }

    public void CallEvent()
    {
        if (!activated)
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
