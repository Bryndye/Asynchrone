using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class P_Controller : MonoBehaviour
{
    [Header("For Camera")]
    [SerializeField] Transform anchor_cam;
    [SerializeField] Transform arms;
    [SerializeField] Transform body_Player;
    [SerializeField] float multiplicateur;
    float rotY;

    [Space]
    Rigidbody rb;
    [Header("Movement")]
    [SerializeField] float speedMove;

    public float StatutMove 
    {
        get 
        {
            if (onGround)
            {
                return 1;
            }
            else
            {
                return 0.5f;
            }
            ;
        }

    }

    [Header("Jump")]
    [SerializeField] float jumpPower;
    bool onGround;
    [SerializeField] Transform[] posRay;

    [Header("Crouch")]
    CapsuleCollider cc;
    float sizeCC
    {
        get
        {
            if (Input.GetAxis("Crounch") != 0)
            {
                //cc.center = new Vector3(0,-0.5f,0);
                return 1;
            }
            else
            {
                //cc.center = Vector3.zero;
                return 2;
            }
            ;
        }
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        //QualitySettings.antiAliasing = 8;
    }

    private void Update()
    {
        CameraMove();
        Movement();
        Jump();
        Crounch();
    }

    private void CameraMove()
    {
        rotY += Input.GetAxis("Mouse Y") * multiplicateur * -1;
        body_Player.localEulerAngles += new Vector3(0, Input.GetAxis("Mouse X") * multiplicateur, 0);

        rotY = Mathf.Clamp(rotY, -80, 80);
        anchor_cam.transform.localRotation = Quaternion.Euler(rotY, 0, 0);
        arms.transform.localRotation = Quaternion.Euler(rotY, 0, 0);
    }

    private void Movement()
    {
        if (onGround)
        {
            rb.velocity = transform.right * Input.GetAxis("Horizontal") * speedMove * StatutMove +
            rb.velocity.y * transform.up +
            transform.forward * Input.GetAxis("Vertical") * speedMove * StatutMove;
        }
    }

    private void Jump()
    {
        //onGround = RayCastOnGround();

        if (RayCastOnGround() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = transform.up * jumpPower;
        }
    }

    private bool RayCastOnGround()
    {
        RaycastHit hitGround;
        for (int i = 0; i < posRay.Length; i++)
        {
            if (Physics.Raycast(posRay[i].position, transform.up * -1, out hitGround, 1.1f))
            {
                return true;
            }
        }
        if (Physics.Raycast(transform.position, transform.up * -1, out hitGround, 1.1f))
        {
            return true;
        }
        return false;
    }

    private void Crounch()
    {
        if (sizeCC != cc.height)
        {
            cc.height = sizeCC;
        }
    }
}
