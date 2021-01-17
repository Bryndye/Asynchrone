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
    }
    public typeEvent eventToTrigger;

    [Header("IA a activer")]
    [ShowIf("ia")] public GameObject[] iaToActivate;

    [Header("Dialogues")]
    [ShowIf("audio")] public string[] dialogues;
    [ShowIf("audio")] public AudioClip audioC;

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

    #region EditMoi
    bool audio;
    bool ia;
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
    }

    #endregion
}
/*
#if UNITY_EDITOR
[CustomEditor(typeof(Event_Trigger))]
public class Event_Trigger_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields

        Event_Trigger script = (Event_Trigger)target;

        // draw checkbox for the bool
        //script.eventToTrigger = EditorGUILayout.Toggle("iaToActivate", script.eventToTrigger);
        if (script.eventToTrigger == Event_Trigger.typeEvent.Audio) // if bool is true, show other fields
        {
            script.audioC = EditorGUILayout.ObjectField("Clip", script.audioC, typeof(AudioClip), true) as AudioClip;
            //script.dialogues2 = EditorGUILayout.ObjectField("IA", script.dialogues2.GetType(), typeof(GameObject[]), true) as GameObject[];
        }
    }
}
#endif*/
