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
        }
    }

    public void Linked()
    {
        nav.SetDestination(mP.Player1.position);

        if (Vector2.Distance(new Vector2(mP.Player1.position.x, mP.Player1.position.z), new Vector2(transform.position.x, transform.position.z)) < 1)
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
