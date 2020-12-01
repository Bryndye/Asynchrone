using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent nav;
    ManagerPlayers mP;
    public bool CanPlay;

    Vector3 finalDestination;

    private void Awake()
    {
        mP = ManagerPlayers.Instance;
        nav = GetComponent<NavMeshAgent>();

        finalDestination = transform.position;
    }

    void Update()
    {
        if (CanPlay)
        {
            InputManager();
        }
        WalkAnim();
    }
    
    private void InputManager()
    {
        if (Input.GetAxisRaw("Aim") > 0)
        {
            OnClickMouse();
        }
    }

    private void OnClickMouse()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            nav.SetDestination(hit.point);
            finalDestination = hit.point;
        }
    }

    private void WalkAnim()
    {
        if (finalDestination != null)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(finalDestination.x, finalDestination.z)) > 1.1f)
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
}
