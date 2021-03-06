﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Human : Singleton<Human>
{
    #region var
    ManagerPlayers managerPlayers;
    CanvasManager canvasManager;
    NavMeshAgent nav;
    CapsuleCollider cc;

    float size;
    float speed;

    [Header("Accroupi")]
    [HideInInspector] public bool isAccroupi;

    #endregion





    private void Awake()                            //AWAKE
    {
        if (Instance != this)
            Destroy(this);

        managerPlayers = ManagerPlayers.Instance;
        managerPlayers.HumanPlayer = this;
        managerPlayers.PlayerHumanTransform = transform;

        canvasManager = CanvasManager.Instance;

        nav = GetComponent<NavMeshAgent>();
        speed = nav.speed;
        cc = GetComponent<CapsuleCollider>();
        size = cc.height;
    }





    private void Update()                           //UPDATE
    {
        if (!managerPlayers.PlayerControllerHm.InCinematic)
        {
            if (managerPlayers.onPlayerHuman)
            {
                if (Input.GetKeyDown(managerPlayers.InputCrouch))
                {
                    CheckMask();
                }
            }
        }
    }








    #region Accroupi
    public void CheckMask()
    {
        NavMeshHit hit = new NavMeshHit();
        nav.SamplePathPosition(NavMesh.AllAreas, 0.0f, out hit);
        int ll = LayerMask.GetMask("Human");
        
        if (hit.mask != ll)
        {
            Accroupi();
        }
    }



    private void Accroupi()
    {
        int h, sp, si;
        float center;
        isAccroupi = !isAccroupi;

        canvasManager.ChangeSpriteCrouch(!isAccroupi);

        if (isAccroupi)
        {
            //1,2,2,-0.5f
            h = 1;
            sp = 2;
            si = 2;
            center = -0.5f;
            nav.areaMask += 1 << NavMesh.GetAreaFromName("Human");
        }
        else
        {
            //2, 1, 1, 0   
            h = 2;
            sp = 1;
            si = 1;
            center = 0;
            nav.areaMask -= 1 << NavMesh.GetAreaFromName("Human");
        }

        nav.height = h;
        nav.speed = speed / sp;
        cc.height = size / si;
        cc.center = new Vector3(cc.center.x, center, cc.center.z);
    }

    #endregion
}
