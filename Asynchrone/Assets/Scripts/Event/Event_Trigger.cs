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
    public MyDialogue[] myDialogues;
    private string[] dialogues;
    private Sprite[] myPortraits;
    private string[] myNames;
    //public AudioClip[] audioC;

    [Header("Porte")]
    public GameObject PorteMesh;



    private void Awake()
    {
        SetDialogue();

        for (int i = 0; i < iaToActivate.Length; i++)
        {
            iaToActivate[i].SetActive(false);
        }
    }

    private void SetDialogue()
    {
        dialogues = new string[myDialogues.Length];
        myPortraits = new Sprite[myDialogues.Length];
        myNames = new string[myDialogues.Length];
        for (int i = 0; i < myDialogues.Length; i++)
        {
            dialogues[i] = myDialogues[i].dialogues;
            myPortraits[i] = myDialogues[i].portrait;
            myNames[i] = myDialogues[i].nom.ToString();
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
        if (cm.dialogueText != null && dialogues.Length > 0)
        {
            cm.StartDiaEffect(dialogues, myPortraits, myNames);
        }
    }

    #region Porte
    private void PorteTime()
    {
        PorteMesh.SetActive(false);
        Invoke(nameof(ActivePorte), 2f);
    }
    private void ActivePorte()
    {
        PorteMesh.SetActive(true);
    }
    #endregion


    #region EditMoi


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
        switch (eventToTrigger)
        {
            case typeEvent.Audio:
                gameObject.name = "Evt audio " + nameEvent;
                break;
            case typeEvent.ActiveIA:
                gameObject.name = "Evt ia " + nameEvent;
                break;
            case typeEvent.Porte1side:
                gameObject.name = "Evt Porte " + nameEvent;
                break;
            default:
                break;
        }
    }

    #endregion
}

[System.Serializable]
public class MyDialogue
{
    public enum NomChara
    {
        V4trek,
        Jumes
    }
    public NomChara nom;
    public Sprite portrait;
    [TextArea(5, 30)]
    public string dialogues;
}

