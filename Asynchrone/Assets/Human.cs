using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    ManagerPlayers mP;

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
    }
}
