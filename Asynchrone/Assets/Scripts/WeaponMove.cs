using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMove : MonoBehaviour
{
    [Header("Anim W")]
    [SerializeField] Transform Arm;
    [SerializeField] float timer;

    private void Update()
    {
        MoveArm();
    }

    void MoveArm()
    {
        float y = 5 * Input.GetAxis("Mouse X");
        float x = -5 * Input.GetAxis("Mouse Y");

        Arm.localEulerAngles = Vector3.Lerp(Arm.localEulerAngles
        , new Vector3(Arm.localEulerAngles.x, Arm.localEulerAngles.y , Arm.localEulerAngles.z)
        , timer);
    }

    /*
    void Radar()
    {
        if (radar_ != null)
            radar_.localEulerAngles = new Vector3(0, 0, transform.eulerAngles.y);
    }*/
}
