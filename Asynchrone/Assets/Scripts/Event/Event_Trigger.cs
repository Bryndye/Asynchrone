using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;

public class Event_Trigger : MonoBehaviour
{
    CanvasManager cm;
    CameraManager camM;
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
    [ShowIf("ia")] public GameObject[] iaToActivate;

    [Header("Dialogues")]
    [ShowIf("audio")] public string[] dialogues;
    [ShowIf("audio")] public AudioClip[] audioC;

    [Header("Porte")]
    [ShowIf("porte")] public GameObject PorteMesh;

    private void Awake()
    {
        cm = CanvasManager.Instance;
        camM = CameraManager.Instance;
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
            switch (eventToTrigger)
            {
                case typeEvent.Porte1side:
                    PorteTime();
                    break;
                default:
                    break;
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
        Invoke(nameof(ActivePorte),5f);
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
