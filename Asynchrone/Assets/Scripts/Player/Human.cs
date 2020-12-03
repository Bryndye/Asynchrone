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

    [Header("Diversion")]
    [SerializeField] GameObject range;
    [SerializeField] LayerMask ignoreWall;
    [SerializeField] float rangeDis;
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
            CreateDiversion();
        }
        range.transform.localScale = new Vector3(rangeDis * 10, rangeDis * 10, 1);
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

    //le sprite 5x, 5y pour faire 1.u
    public void CreateDiversion()
    {
        if (Input.GetAxisRaw("Diversion") > 0)
        {
            range.SetActive(true);

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 point = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                Vector3 dir = (transform.position - hit.point).normalized;

                if (CheckWall(dir, hit.point))
                {
                   GameObject holo = Instantiate(Resources.Load<GameObject>("Player/Fake_Robot"), hit.point, Quaternion.identity);
                }
            }
        }
        else
        {
            range.SetActive(false);
        }
    }

    private bool CheckWall(Vector3 dir, Vector3 point)
    {
        RaycastHit it;

        if (Physics.Raycast(point, dir, out it, rangeDis))
        {
            Debug.Log(it.collider.name + "  " + it.collider.gameObject.layer);

            if (it.collider.gameObject.layer == 9 || it.collider.gameObject.layer == 8)
            {         
                return true;
            }          
        }
        return false;
    }
}
