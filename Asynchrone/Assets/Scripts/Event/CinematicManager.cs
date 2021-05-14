using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    ManagerPlayers mP;
    [SerializeField] float time;
    [SerializeField] Animator anim;
    CanvasManager cm;

    Camera cameraMain;
    Camera cameraCinematic;

    private void Awake() 
    { 
        mP = ManagerPlayers.Instance;
        cm = CanvasManager.Instance;

        //cameraMain = Camera.main;
        //cameraCinematic = transform.GetChild(0).GetComponent<Camera>();

        if (cm)
        {
            cm.BandeAppear();
        }

        checkInCinematic(true);
        Invoke(nameof(EndCinematic), time);
    }

    //private void Update()
    //{
    //    cameraCinematic.transform.position = Vector3.Lerp(cameraCinematic.transform.position, cameraMain.transform.position, time * 100);
    //    cameraCinematic.transform.eulerAngles = Vector3.Lerp(cameraCinematic.transform.eulerAngles, cameraMain.transform.eulerAngles, time * 100);
    //}

    private void EndCinematic()
    {
        if (cm)
        {
            cm.BandeDisAppear();
        }
        checkInCinematic(false);
        Destroy(gameObject);
    }

    private void checkInCinematic(bool inCinematic)
    {
        if (mP.PlayerControllerHm)
        {
            mP.PlayerControllerHm.InCinematic = inCinematic;
        }
        if (mP.PlayerCntrlerRbt)
        {
            mP.PlayerCntrlerRbt.InCinematic = inCinematic;
        }
    }
}
