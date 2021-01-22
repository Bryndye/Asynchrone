﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Human : Singleton<Human>
{
    #region var
    ManagerPlayers mP;
    NavMeshAgent nav;
    CapsuleCollider cc;
    [SerializeField] GameObject meshPrincipal;

    float size;
    float speed;

    [Header("Accroupi")]
    [SerializeField] GameObject robotBeLike;
    [HideInInspector] public bool intoMe;
    [HideInInspector] public bool isAccroupi;

    [Header("Diversion")]
    [SerializeField] GameObject range;
    [SerializeField] LayerMask ignoreWall;
    [SerializeField] float rangeDis;
    [HideInInspector] public bool canDiv;
    [SerializeField] GameObject acrroupiMesh;
    [HideInInspector] public GameObject robot_div;
    [SerializeField] float cdDiv;
    bool canSpell = true;

    #endregion

    public void RobotIntoMe(bool intoMoi)
    {
        intoMe = intoMoi;

        robotBeLike.SetActive(intoMe);
        mP.Player2.gameObject.SetActive(!intoMe);

        if (!intoMe)
        {
            mP.Player2.position = transform.position;
        }
    }

    private void Awake()
    {
        if (Instance != this)
            Destroy(this);

        mP = ManagerPlayers.Instance;
        mP.Hm = this;
        mP.Player1 = transform;

        nav = GetComponent<NavMeshAgent>();
        speed = nav.speed;
        cc = GetComponent<CapsuleCollider>();
        size = cc.height;
    }

    private void Update()
    {
        if (!mP.pc1.InCinematic)
        {
            if (mP.onPlayer1)
            {
                InputManager();
            }
            GestionDiv();
        }
    }

    private void InputManager()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            CheckMask();
        }
        
        if (Input.GetKeyDown(KeyCode.Z) && robot_div == null && canSpell)
        {
            canDiv = !canDiv;
        }
        if (Input.GetKeyDown(KeyCode.Z) && robot_div != null)
        {
            Destroy(robot_div);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            mP.RobotBackToHuman();
        }
    }

    #region Accroupi
    private void CheckMask()
    {

        NavMeshHit hit = new NavMeshHit();
        nav.SamplePathPosition(NavMesh.AllAreas, 0.0f, out hit);
        int ll = LayerMask.GetMask("Human");
        Debug.Log("Agent is currently on " + hit.mask + "  " + ll);
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
        //acrroupiMesh.SetActive(isAccroupi);
        //meshPrincipal.SetActive(!isAccroupi);

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

    #region Diversion
    //le sprite 1x, 1y pour faire 2.5u
    public void CreateDiversion()
    {
        range.transform.localScale = new Vector3(rangeDis / 2.5f, rangeDis / 2.5f, 1);

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (hit.collider.gameObject.layer != 10)
            {
                Vector3 point = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                Vector3 dir = (transform.position - point).normalized;

                if (CheckWall(dir, point))
                {
                    robot_div = Instantiate(Resources.Load<GameObject>("Player/Fake_Robot"), hit.point, Quaternion.identity);
                    canDiv = false;
                    canSpell = false;
                    Invoke(nameof(ItsTime), cdDiv);
                    mP.pc1.DivAnim();
                }
            }
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
            CreateDiversion();
        }
        if (!mP.onPlayer1)
        {
            canDiv = false;
        }
        range.SetActive(canDiv);
        //bt_destroy.interactable = robot_div != null;
    }

    private void ItsTime()
    {
        canSpell = true;
    }

    #endregion
}
