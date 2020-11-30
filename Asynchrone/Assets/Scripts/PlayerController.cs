using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent nav;
    public ManagerPlayers mP;
    public bool CanPlay;
    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (CanPlay)
        {
            InputManager();
        }
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
            //Instantiate(test, hit.point, Quaternion.identity);
            nav.SetDestination(hit.point);
        }
    }
}
