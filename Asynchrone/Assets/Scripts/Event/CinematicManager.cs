using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    ManagerPlayers mP;
    [SerializeField] float time;
    [SerializeField] Animator anim;
    CanvasManager cm;

    private void Awake() { 
        anim.enabled = false;
        mP = ManagerPlayers.Instance;
        cm = CanvasManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (cm)
            {
                cm.BandeAppear();
            }
            checkInCinematic(true);
            anim.enabled = true;
            Invoke(nameof(EndCinematic), time);
        }
    }

    private void EndCinematic()
    {
        if (cm)
        {
            cm.BandeDisAppear();
        }
        checkInCinematic(false);
        Destroy(gameObject);
    }

    private void checkInCinematic(bool ok)
    {
        if (mP.pc1)
        {
            mP.pc1.InCinematic = ok;
        }
        if (mP.pc2)
        {
            mP.pc2.InCinematic = ok;
        }
    }
}
