﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    #region var
    [HideInInspector] public NavMeshAgent nav;
    ManagerPlayers mP;
    public bool CanPlay;

    Vector3 finalDestination;
    Transform targetInteraction;

    public bool InCinematic;
    #endregion

    private void Awake()
    {
        mP = ManagerPlayers.Instance;
        nav = GetComponent<NavMeshAgent>();

        finalDestination = transform.position;
    }

    void Update()
    {
        if (!InCinematic)
        {
            if (CanPlay)
            {
                InputManager();
                CheckDisInteraction();
            }
            WalkAnim();
        }
    }

    private void InputManager()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            OnClickMouseR();
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            OnClickMouseL();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && !mP.onPlayer1 && mP.Rbt.BackToHuman )
        {
            mP.Rbt.BackToHuman = false;
            OnClickMouseR();
        }
    }

    #region Interaction element decor
    private void OnClickMouseL()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Interaction")
            {
                nav.SetDestination(hit.point);
                finalDestination = hit.point;
                targetInteraction = hit.collider.transform;
            }

            anAI ia = hit.collider.GetComponent<anAI>();
            if (ia != null && !mP.onPlayer1)
            {
                nav.SetDestination(hit.point);
                finalDestination = hit.point;
                targetInteraction = hit.collider.transform;
            }
        }
    }
    private void CheckDisInteraction()
    {
        if (targetInteraction != null)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetInteraction.position.x, targetInteraction.position.z)) < 1)
            {
                Interaction iem = targetInteraction.GetComponent<Interaction>();
                if (iem != null)
                {
                    iem.CallEvent();
                }

                anAI ia = targetInteraction.GetComponent<anAI>();
                if (ia != null && !mP.onPlayer1)
                {
                    Debug.Log("T MORT");
                }

                targetInteraction = null;
            }
        }
        //Debug.Log(targetInteraction);
    }

    #endregion

    private void OnClickMouseR()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer != 10)
            {
                nav.SetDestination(hit.point);
                finalDestination = hit.point;
            }
            else
            {
                nav.SetDestination(transform.position);
                finalDestination = transform.position;
            }
        }
    }


    #region AnimManager
    private void WalkAnim()
    {
        if (finalDestination != null)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(finalDestination.x, finalDestination.z)) > 1)
            {
                //Debug.Log("je marche " + transform.name);
            }
            else
            {
                //Debug.Log("idle " + transform.name);
            }
            //Debug.Log(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(finalDestination.x, finalDestination.z)) + " " + transform.name);
        }
    }
    #endregion
}
