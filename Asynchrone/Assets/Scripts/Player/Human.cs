using System.Collections;
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

    float size;
    float speed;

    [Header("Accroupi")]
    [HideInInspector] public bool isAccroupi;
    #endregion





    private void Awake()                            //AWAKE
    {
        if (Instance != this)
            Destroy(this);

        mP = ManagerPlayers.Instance;
        mP.HumanPlayer = this;
        mP.Player1 = transform;

        nav = GetComponent<NavMeshAgent>();
        speed = nav.speed;
        cc = GetComponent<CapsuleCollider>();
        size = cc.height;
    }





    private void Update()                           //UPDATE
    {
        if (!mP.PlayerCtrlerHm.InCinematic)
        {
            if (mP.onPlayer1)
            {
                InputManager();
            }
        }
    }











    private void InputManager()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            CheckMask();
        }
    }






    #region Accroupi
    public void CheckMask()
    {
        NavMeshHit hit = new NavMeshHit();
        nav.SamplePathPosition(NavMesh.AllAreas, 0.0f, out hit);
        int ll = LayerMask.GetMask("Human");
        //Debug.Log("Agent is currently on " + hit.mask + "  " + ll);
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
}
