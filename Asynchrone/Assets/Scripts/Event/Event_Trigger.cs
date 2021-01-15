using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Trigger : MonoBehaviour
{
    [SerializeField] string nameEvent;
    private bool done = false;

    [SerializeField] private GameObject[] iaToActivate;
    [Space]
    CanvasManager cm;
    CameraManager camM;

    [Header("Dialogues")]
    public string[] dialogues;
    [SerializeField] AudioClip audioC;

    private void Awake()
    {
        cm = CanvasManager.Instance;
        camM = CameraManager.Instance;
        for (int i = 0; i < iaToActivate.Length; i++)
        {
            iaToActivate[i].SetActive(false);
        }
    }
    enum typeEvent
    {
        Audio,
        ActiveIA,
    }
    [SerializeField] typeEvent eventToTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !done)
        {
            if (eventToTrigger == typeEvent.Audio)
            {
                EventAudio();
            }
            if (eventToTrigger == typeEvent.ActiveIA)
            {
                EventIA();
            }
            EventDialogue();

            done = true;
        }
    }

    private void EventAudio()
    {
        if (audioC)
        {
            //Debug.Log(camM + " Manager Cam  " + audioC);
            camM.LaunchSound(audioC);
        }
    }

    private void EventIA()
    {
        for (int i = 0; i < iaToActivate.Length; i++)
        {
            iaToActivate[i].SetActive(true);
        }
        Debug.Log("ia");
    }

    private void EventDialogue()
    {
        if (cm.dialogueHere && dialogues.Length > 0)
        {
            cm.StartDiaEffect(dialogues);
        }
    }

    private void OnDrawGizmos()
    {
        if (eventToTrigger == typeEvent.Audio)
        {
            gameObject.name = "Evt audio " + nameEvent;
        }
        if (eventToTrigger == typeEvent.ActiveIA)
        {
            gameObject.name = "Evt ia " + nameEvent;
        }
    }
}
