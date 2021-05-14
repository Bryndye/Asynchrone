using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    ManagerPlayers mp
    {
        get
        {
            return ManagerPlayers.Instance;
        }
    }
    CanvasManager cm
    {
        get
        {
            return CanvasManager.Instance;
        }
    }
    CameraManager camM
    {
        get
        {
            return CameraManager.Instance;
        }
    }
    SoundManager SM
    {
        get
        {
            return SoundManager.Instance;
        }
    }
    MusicManager MM
    {
        get
        {
            return MusicManager.Instance;
        }
    }

    [SerializeField] private int indexOfNextlevel;
    [SerializeField] private string nameOfNextlevel;
    [SerializeField] private List<GameObject> players;

    bool done = false;

    private void Awake()
    {
        PlayerPrefs.SetInt("indexLevel", indexOfNextlevel);
        PlayerPrefs.SetString("nameLevel", nameOfNextlevel);
    }

    private void OnTriggerStay(Collider other)
    {
        if (done)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            if (!players.Contains(other.gameObject))
            {
                players.Add(other.gameObject);
            }
        }
        if (mp != null)
        {
            if (players.Count >= 2 || mp.PlayerRobotTransform == null && players.Count >= 1)
            {
                mp.PlayerControllerHm.InCinematic = true;
                if(mp.PlayerRobotTransform != null)
                    mp.PlayerCntrlerRbt.InCinematic = true;
                done = true;
                cm.anim.SetTrigger("Transition");
                SM.GetASound("Ascenseur_Fermeture", transform);
                MM.CloseMusic();
            }
        }
        else if (players.Count >= 1)
        {
            done = true;
            cm.ActiveLoadScreen();
            //SM.GetASound("Ascenseur_Fermeture", transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (players.Contains(other.gameObject))
            {
                players.Remove(other.gameObject);
            }
        }
    }
}
