using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Event_Trigger : MonoBehaviour
{
    [SerializeField]
    private bool reactivable;
    private BoxCollider bc;

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

    [Space]
    [SerializeField] string nameEvent;
    private bool done = false;

    public enum typeEvent
    {
        Audio,
        ActiveIA,
        Porte1side,
    }
    public typeEvent eventToTrigger;

    [Header("IA a activer")]
    public GameObject[] iaToActivate;

    [Header("Dialogues")]
    [TextArea(5, 30)]
    public string[] dialogues;
    public AudioClip[] audioC;

    [Header("Porte")]
    public GameObject PorteMesh;



    private void Awake()
    {
        for (int i = 0; i < iaToActivate.Length; i++)
        {
            iaToActivate[i].SetActive(false);
        }
    }


    private void Update()
    {
        if (reactivable && SpawnMANAGER.Instance.mySpawnSituation == SpawnSituation.DeathProcess)
        {
            done = false;
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







    private void EventIA()
    {
        for (int i = 0; i < iaToActivate.Length; i++)
        {
            iaToActivate[i].SetActive(true);
        }
    }

    private void EventDialogue()
    {
        if (cm.dialogueHere != null && dialogues.Length > 0)
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

    private void OnDrawGizmos()
    {
        bc = GetComponent<BoxCollider>();
        EditNom();
        if (eventToTrigger == typeEvent.Porte1side)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.position + new Vector3(0, 1, -1), new Vector3(2, 2, 1));
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position, new Vector3(1,1,1)) ;
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
    }

    #endregion
}
