using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    ManagerPlayers mP;
    NavMeshAgent nav;

    float speed;

    [SerializeField] GameObject robotBeLike;
    public bool intoMe;

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
        mP = ManagerPlayers.Instance;
        nav = GetComponent<NavMeshAgent>();
        speed = nav.speed;
    }

    private void Update()
    {
        if (Input.GetAxisRaw("Accroupir") > 0)
        {
            nav.height = 1;
            nav.speed = speed / 2;
        }
        else
        {
            nav.height = 2;
            nav.speed = speed;
        }
    }
}
