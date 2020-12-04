using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    NavMeshAgent nav;
    ManagerPlayers mP;

    public bool BackToHuman;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        mP = ManagerPlayers.Instance;
    }

    void Update()
    {
        if (BackToHuman)
        {
            Linked();
        }
    }

    public void Linked()
    {
        nav.SetDestination(mP.Player1.position);

        if (Vector3.Distance(mP.Player1.position, transform.position) < 1.1f)
        {
            BackToHuman = false;
            mP.Hm.RobotIntoMe(true);
            if (!mP.onPlayer1)
            {
                mP.CameraManager();
            }
        }
    }

    public void CancelBack()
    {
        nav.SetDestination(transform.position);
    }
}
