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

        cameraCinematic = transform.GetChild(0).GetComponent<Camera>();
    }

    private void Start()
    {
        if (cm)
        {
            cm.BandeAppear();
        }

        checkInCinematic(true);
        Invoke(nameof(EndCinematic), time);
    }


    private void EndCinematic()
    {
        if (cm)
        {
            cm.BandeDisAppear();
        }
        checkInCinematic(false);
        gameObject.SetActive(false);
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
