using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Event_Trigger : MonoBehaviour
{
    private BoxCollider bc;

    ManagerPlayers mp;
    CanvasManager cm;
    CameraManager camM;
    [SerializeField] string nameEvent;
    private bool done = false;

    public enum typeEvent
    {
        Audio,
        ActiveIA,
        Porte1side,
        FinNiveau
    }
    public typeEvent eventToTrigger;

    [Header("IA a activer")]
    public GameObject[] iaToActivate;

    [Header("Dialogues")]
    public string[] dialogues;
    public AudioClip[] audioC;

    [Header("Porte")]
    public GameObject PorteMesh;

    [Header("Fin de niveau")]
    [SerializeField] private int indexOfNextlevel;
    [SerializeField] private List<GameObject> players;


    private void Awake()
    {
        cm = CanvasManager.Instance;
        camM = CameraManager.Instance;
        mp = ManagerPlayers.Instance;
        for (int i = 0; i < iaToActivate.Length; i++)
        {
            iaToActivate[i].SetActive(false);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !done)
        {
            switch (eventToTrigger)
            {
                case typeEvent.Audio:
                    EventDialogue();
                    break;
                case typeEvent.ActiveIA:
                    EventIA();
                    break;

                default:
                    break;
            }
            done = true;
        }
        if (other.CompareTag("Player"))
        {
            if (eventToTrigger == typeEvent.Porte1side)
            {
                PorteTime();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (eventToTrigger == typeEvent.FinNiveau)
        {
            if (other.CompareTag("Player"))
            {
                if (!players.Contains(other.gameObject))
                {
                    players.Add(other.gameObject);
                }
            }

            if (players.Count >= 2 || mp.Player2 == null && players.Count >= 1)
            {
                SceneManager.LoadScene(indexOfNextlevel);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (eventToTrigger == typeEvent.FinNiveau)
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



    private void EventIA()
    {
        for (int i = 0; i < iaToActivate.Length; i++)
        {
            iaToActivate[i].SetActive(true);
            Debug.Log("ia");
        }
    }

    private void EventDialogue()
    {
        if (cm.dialogueHere && dialogues.Length > 0)
        {
            cm.StartDiaEffect(dialogues, audioC);
        }
    }

    #region Porte
    private void PorteTime()
    {
        PorteMesh.SetActive(false);
        Invoke(nameof(ActivePorte),2f);
    }
    private void ActivePorte()
    {
        PorteMesh.SetActive(true);
    }
    #endregion


    #region EditMoi

    bool audio;
    bool ia;
    bool porte;
    bool end;

    private void OnDrawGizmos()
    {
        bc = GetComponent<BoxCollider>();
        EditNom();
        if (eventToTrigger == typeEvent.Porte1side)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.localPosition + new Vector3(0, 1, -1), new Vector3(2, 2, 1));
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.localPosition, new Vector3(1,1,1)) ;
        }
    }

    private void EditNom()
    {
        if (eventToTrigger == typeEvent.Audio)
        {
            gameObject.name = "Evt audio " + nameEvent;
            audio = true;
        }
        else
        {
            audio = false;
        }
        if (eventToTrigger == typeEvent.ActiveIA)
        {
            gameObject.name = "Evt ia " + nameEvent;
            ia = true;
        }
        else
        {
            ia = false;
        }
        if (eventToTrigger == typeEvent.Porte1side)
        {
            gameObject.name = "Evt Porte " + nameEvent;
            porte = true;

        }
        else
        {
            porte = false;
        }
        if (eventToTrigger == typeEvent.FinNiveau)
        {
            gameObject.name = "Evt End " + nameEvent;
            end = true;
        }
        else
        {
            end = false;
        }
    }

    #endregion
}
