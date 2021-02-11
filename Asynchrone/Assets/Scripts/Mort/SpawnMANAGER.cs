using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnMANAGER : Singleton<SpawnMANAGER>
{
    ManagerPlayers mp;
    CanvasManager cm;
    public Vector3 SpawnPointR;
    public Vector3 SpawnPointH;
    bool done;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        mp = ManagerPlayers.Instance;
        cm = CanvasManager.Instance;
    }
    //private void 
    public void GetSpawn(Vector3 posH, Vector3 posR, bool die)
    {
        Debug.Log("get the spawns");
        SpawnPointR = posR;
        SpawnPointH = posH;
        if (die)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        Debug.Log("RESPAWN");
        mp.pc1.InCinematic = true;
        mp.pc2.InCinematic = true;
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
            mp.pc1.nav.Warp(SpawnPointH);
            mp.pc1.anim.SetBool("Walking", false);
        }
        if (SpawnPointR != null)
        {
            mp.pc2.nav.Warp(SpawnPointR);
            //mp.pc2.anim.SetBool("Walking", false);
        }
        Invoke(nameof(SetPlayer), 1f);
    }

    private void SetPlayer()
    {
        Debug.Log("is finished");

        mp.pc1.InCinematic = false;
        mp.pc2.InCinematic = false;
        done = false;
    }
}
