using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof( AudioSource))]
public class Event_Trigger : MonoBehaviour
{
    [SerializeField] string nameEvent;
    bool done = false;

    [SerializeField] private GameObject[] iaToActivate;
    [Space]
    AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < iaToActivate.Length; i++)
        {
            iaToActivate[i].SetActive(false);
        }
    }
    enum typeEvent
    {
        Audio,
        ActiveIA
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
            done = true;
        }
    }

    private void EventAudio()
    {
        audioSource.Play();
        Debug.Log("audio");
    }

    private void EventIA()
    {
        for (int i = 0; i < iaToActivate.Length; i++)
        {
            iaToActivate[i].SetActive(true);
        }
        Debug.Log("ia");
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
