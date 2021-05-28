﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SpawnSituation { Playing, DeathProcess }

public class SpawnMANAGER : Singleton<SpawnMANAGER>
{
    [Header("External references")]
    ManagerPlayers mp;
    CanvasManager cm;
   
    [Header("Global")]
    public SpawnSituation mySpawnSituation;

    [Header("Spawn Positions")]
    public Transform SpawnPointR;
    public Transform SpawnPointH;
    bool inCinematic;

    [Header("Playing AIs")]
    public List<anAI> myAIs;
    [HideInInspector]
    public List<anAI> myAIs_Checkpoint;



    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        mp = ManagerPlayers.Instance;
        cm = CanvasManager.Instance;
    }

    private void Start()
    {
        AiCheck();
    }


    private void Update()
    {
        if (inCinematic && Input.GetKeyDown(KeyCode.Space))
        {
            SetPlayer();
        }
    }

    public void GetSpawn(Transform spawnHum, Transform spawnRbt)
    {
        SpawnPointH = spawnHum;
        if (mp.RobotPlayer)
            SpawnPointR = spawnRbt;
    }

    public void Respawn()
    {
        //mp.PlayerControllerHm.InCinematic = true;
        //if (mp.RobotPlayer) mp.PlayerCntrlerRbt.InCinematic = true;
        cm.anim.SetTrigger("dead");

        Invoke(nameof(EndAnim), 1f);
    }

    private void EndAnim()
    {
        inCinematic = true;
        cm.mortZone.SetActive(true);
        if (SpawnPointH != null)
        {
            mp.PlayerControllerHm.NavPlayer.Warp(SpawnPointH.position);
            mp.PlayerHumanTransform.rotation = SpawnPointH.rotation;
            mp.PlayerControllerHm.AnimPlayer.SetBool("Walking", false);
        }
        if (SpawnPointR != null && mp.RobotPlayer)
        {
            mp.PlayerCntrlerRbt.NavPlayer.Warp(SpawnPointR.position);
            mp.PlayerRobotTransform.rotation = SpawnPointH.rotation;
            //mp.pc2.anim.SetBool("Walking", false);
        }
    }

    private void SetPlayer()
    {
        cm.mortZone.SetActive(false);
        cm.anim.SetTrigger("respawn");
        AiRespawn();

        mp.PlayerControllerHm.InCinematic = false;
        if (mp.RobotPlayer) mp.PlayerCntrlerRbt.InCinematic = false;
        inCinematic = false;

        mySpawnSituation = SpawnSituation.Playing;
    }


    #region AI
    public void AiCheck()
    {
        myAIs_Checkpoint = new List<anAI>();

        for (int i = 0; i < myAIs.Count; i++)
        {
            if (myAIs[i].gameObject.activeSelf)
            {
                myAIs_Checkpoint.Add(myAIs[i]);
            }
        }
    }

    void AiRespawn()
    {
        for (int i = 0; i < myAIs_Checkpoint.Count; i++)
        {
            if (!myAIs_Checkpoint[i].gameObject.activeSelf)
            {
                myAIs_Checkpoint[i].gameObject.SetActive(true);
                myAIs_Checkpoint[i].AISpawn();
            }
            myAIs_Checkpoint[i].AIReset();
        }
        myAIs = myAIs_Checkpoint;
    }
    #endregion
}
