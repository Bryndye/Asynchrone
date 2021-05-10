using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_script : MonoBehaviour
{
    private ManagerPlayers mp;
    private LineRenderer lr;

    [SerializeField] private GameObject trap1;
    [SerializeField] private GameObject trap2;

    [SerializeField] private bool activeVisualLink;



    private void Awake()
    {
        mp = ManagerPlayers.Instance;

        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }



    public void Called(GameObject trapIndex)
    {
        //Debug.Log(trapIndex+" called!");

        if (trapIndex == trap1)
        {
            //Debug.Log(trap1.name + " called!");           
            mp.PlayerCntrlerRbt.nav.Warp(trap2.transform.GetChild(0).position);

        }
        if (trapIndex == trap2)
        {
            //Debug.Log(trap2.name + " called!");
            mp.PlayerCntrlerRbt.nav.Warp(trap1.transform.GetChild(0).position);
        }
    }








    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(trap1.transform.GetChild(0).position, 0.2f);
        Gizmos.DrawSphere(trap2.transform.GetChild(0).position, 0.2f);

        lr = GetComponent<LineRenderer>();

        if (trap1 != null && trap2 != null && activeVisualLink)
        {
            lr.enabled = true;

            lr.SetPosition(0, trap1.transform.position);
            lr.SetPosition(1, trap2.transform.position);
            //Debug.Log("called");
        }
        else
        {
            lr.enabled = false;
        }
    }
}
