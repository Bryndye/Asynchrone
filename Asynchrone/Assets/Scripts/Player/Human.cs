using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Human : MonoBehaviour
{
    #region var
    ManagerPlayers mP;
    NavMeshAgent nav;
    CapsuleCollider cc;
    [SerializeField] GameObject meshPrincipal;

    float size;
    float speed;

    [SerializeField] GameObject robotBeLike;
    public bool intoMe;

    [Header("Diversion")]
    [SerializeField] GameObject range;
    [SerializeField] LayerMask ignoreWall;
    [SerializeField] float rangeDis;
    [HideInInspector] public bool canDiv;
    [SerializeField] GameObject acrroupiMesh;
    [HideInInspector] public GameObject robot_div;

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
        if (mP.onPlayer1)
        {
            InputManager();
            GestionDiv();
        }
    }

    private void InputManager()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Accroupi(1,2,2,-0.5f, true);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Accroupi(2, 1, 1, 0, false);
        }
        if (Input.GetKeyDown(KeyCode.Z) && robot_div == null)
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

    private void Accroupi(int h, int sp, int si, float center, bool acr)
    {
        acrroupiMesh.SetActive(acr);
        meshPrincipal.SetActive(!acr);

        nav.height = h;
        nav.speed = speed / sp;
        cc.height = size / si;
        cc.center = new Vector3(cc.center.x, center, cc.center.z);
    }


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

            if (it.collider.gameObject.layer == 9 || it.collider.gameObject.layer == 8)
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
        range.SetActive(canDiv);
        //bt_destroy.interactable = robot_div != null;
    }

    #endregion
}
