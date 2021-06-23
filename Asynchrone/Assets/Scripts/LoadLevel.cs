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
    DoubleFeedback doubleFB;

    [SerializeField] private bool aCinematic;
    [SerializeField] private int indexOfNextlevel;
    [SerializeField] private List<GameObject> players;



    bool done = false;

    private void Awake()
    {
        doubleFB = GetComponent<DoubleFeedback>();
    }

    private void Start()
    {
        if (aCinematic)
        {
            PlayerPrefs.SetInt("indexLevel", indexOfNextlevel);
        }
    }

    private void OnTriggerEnter(Collider other)
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
                if (other.gameObject.TryGetComponent(out PlayerController pc) && mp.PlayerRobotTransform != null)
                {
                    if (pc.myPlayer == whichPlayer.Human)
                    {
                        doubleFB.ActiveHuman(true);
                    }
                    else
                    {
                        doubleFB.ActiveRobot(true);
                    }
                }
            }
        }
        if (mp != null)
        {
            if (players.Count >= 2 || mp.PlayerRobotTransform == null && players.Count >= 1)
            {
                done = true;

                mp.PlayerControllerHm.InCinematic = true;
                if(mp.PlayerRobotTransform != null)
                    mp.PlayerCntrlerRbt.InCinematic = true;

                PlayerPrefs.SetInt("indexLevel", indexOfNextlevel);

                cm.anim.SetTrigger("Transition");
                SM.GetASound("Ascenseur_Fermeture", transform);
                MM.CloseMusic();
            }
        }
        else if (players.Count >= 1)
        {
            done = true;

            PlayerPrefs.SetInt("indexLevel", indexOfNextlevel);
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
            if (other.gameObject.TryGetComponent(out PlayerController pc))
            {
                if (pc.myPlayer == whichPlayer.Human)
                {
                    doubleFB.ActiveHuman(false);
                }
                else
                {
                    doubleFB.ActiveRobot(false);
                }
            }
        }
    }
}
