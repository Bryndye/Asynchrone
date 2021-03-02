﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    NavMeshAgent nav;
    ManagerPlayers mP;

    public bool BackToHuman;

    [Header("Diversion")]
    [SerializeField] GameObject range;
    [SerializeField] LayerMask ignoreWall;
    [SerializeField] float rangeDis;
    public bool canDiv;
    [HideInInspector] public GameObject robot_div;
    public int DivStock = 0;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        mP = ManagerPlayers.Instance;
        mP.Rbt = this;
        mP.Player2 = transform;
    }

    void Update()
    {
        if (!mP.pc2.InCinematic)
        {         
            if (BackToHuman)
            {
                Linked();
            }
            if (!mP.onPlayer1)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    mP.RobotBackToHuman();
                }
                if (Input.GetKeyDown(KeyCode.Z) && robot_div == null && DivStock > 0)
                {
                    StartDiv();
                }
                if (Input.GetKeyDown(KeyCode.Z) && robot_div != null)
                {
                    Destroy(robot_div);
                }
            }
            GestionDiv();
        }
    }

    #region Diversion

    public void StartDiv() { canDiv = !canDiv;
        Debug.Log("OUPS");
    }
    //le sprite 1x, 1y pour faire 2.5u
    public void CreateDiversion(RaycastHit hit)
    {
        if (hit.collider.gameObject.layer != 10)
        {
            Vector3 point = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Vector3 dir = (transform.position - point).normalized;

            if (CheckWall(dir, point))
            {
                robot_div = Instantiate(Resources.Load<GameObject>("Player/Fake_Robot"), hit.point, Quaternion.identity);
                StockDivManager();
                canDiv = false;
            }
        }
    }

    private void StockDivManager()
    {
        DivStock--;
        if (DivStock <= 0)
        {
            DivStock = 0;
        }
    }

    private bool CheckWall(Vector3 dir, Vector3 point)
    {
        RaycastHit it;

        if (Physics.Raycast(point, dir, out it, rangeDis))
        {
            //Debug.Log(it.collider.name + "  " + it.collider.gameObject.layer);

            if (it.collider.gameObject.layer == 9 || it.collider.gameObject.layer == 8 || it.collider.gameObject.layer == 12)
            {
                return true;
            }
        }
        return false;
    }

    private void GestionDiv()
    {
        if (canDiv)
        {
            range.transform.localScale = new Vector3(rangeDis / 2.5f, rangeDis / 2.5f, 1);
            //CreateDiversion();
        }
        if (mP.onPlayer1)
        {
            canDiv = false;
        }
        range.SetActive(canDiv);
        //bt_destroy.interactable = robot_div != null;
    }

    #endregion

    #region WhenHumanCallHim
    public void Linked()
    {
        nav.SetDestination(mP.Player1.position);

        if (Vector2.Distance(new Vector2(mP.Player1.position.x, mP.Player1.position.z), new Vector2(transform.position.x, transform.position.z)) < 1)
        {
            BackToHuman = false;
            mP.Hm.RobotIntoMe(true);
            if (!mP.onPlayer1)
            {
                mP.Camera_Manager();
            }
        }
    }

    public void CancelBack()
    {
        if (gameObject.activeSelf)
        {
            nav.SetDestination(transform.position);
        }
    }
    #endregion
}
