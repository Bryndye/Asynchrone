using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    ManagerPlayers mP;
    NavMeshAgent nav;
    CapsuleCollider cc;
    float size;

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
        cc = GetComponent<CapsuleCollider>();
        size = cc.height;
    }

    private void Update()
    {
        if (mP.onPlayer1)
        {
            InputManager();
        }
    }

    private void InputManager()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Accroupi(1,2,2,-0.5f);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Accroupi(2, 1, 1, 0);
        }
    }

    private void Accroupi(int h, int sp, int si, float center)
    {
        nav.height = h;
        nav.speed = speed / sp;
        cc.height = size / si;
        cc.center = new Vector3(cc.center.x, center, cc.center.z);
    }
}
