﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    ManagerPlayers mP;
    [SerializeField] float time;
    [SerializeField] Animator anim;
    CanvasManager cm;

    Camera cameraMain;

    private void Awake() 
    { 
        anim.enabled = false;
        mP = ManagerPlayers.Instance;
        cm = CanvasManager.Instance;

        cameraMain = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //if (cm)
            //{
            //    cm.BandeAppear();
            //}
            //anim.enabled = true;

            checkInCinematic(true);
            Invoke(nameof(EndCinematic), time);
        }
    }

    private void Update()
    {
        
    }

    private void EndCinematic()
    {
        if (cm)
        {
            cm.BandeDisAppear();
        }
        checkInCinematic(false);
        Destroy(gameObject);
    }

    private void checkInCinematic(bool inCinematic)
    {
        if (mP.PlayerControllerHm)
        {
            mP.PlayerControllerHm.InCinematic = inCinematic;
        }
        if (mP.PlayerCntrlerRbt)
        {
            mP.PlayerCntrlerRbt.InCinematic = inCinematic;
        }
    }
}
