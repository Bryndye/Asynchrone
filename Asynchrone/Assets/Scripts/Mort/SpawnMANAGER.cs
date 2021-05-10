using System.Collections;
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
    public Vector3 SpawnPointR;
    public Vector3 SpawnPointH;
    bool done;

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
    //private void 
    public void GetSpawn(Vector3 posH, Vector3 posR, bool die)
    {
        //Debug.Log("get the spawns");
        if(mp.RobotPlayer) SpawnPointR = posR;
        SpawnPointH = posH;
        if (die)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        //Debug.Log("RESPAWN");
        mp.PlayerCtrlerHm.InCinematic = true;
        if (mp.RobotPlayer) mp.PlayerCntrlerRbt.InCinematic = true;
        cm.anim.SetTrigger("dead");

        if (SpawnPointR != null && !done)
        {
            done = true;
            Invoke(nameof(EndAnim), 1f);
        }
    }

    private void EndAnim()
    {
        if (SpawnPointH != null)
        {
            mp.PlayerCtrlerHm.nav.Warp(SpawnPointH);
            mp.PlayerCtrlerHm.anim.SetBool("Walking", false);
        }
        if (SpawnPointR != null && mp.RobotPlayer)
        {
            mp.PlayerCntrlerRbt.nav.Warp(SpawnPointR);
            //mp.pc2.anim.SetBool("Walking", false);
        }
        AiRespawn();
        Invoke(nameof(SetPlayer), 1f);
    }

    private void SetPlayer()
    {
        //Debug.Log("is finished");

        mp.PlayerCtrlerHm.InCinematic = false;
        if (mp.RobotPlayer) mp.PlayerCntrlerRbt.InCinematic = false;
        done = false;

        mySpawnSituation = SpawnSituation.Playing;
    }

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
}
