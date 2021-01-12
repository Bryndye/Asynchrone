using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Event_int
{
    bouton,
    alarme
}

public class InteractionEventManager : Singleton<InteractionEventManager>
{
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(this);
        }
    }

    public void Event_Button(Transform[] porte)
    {
        //Debug.Log("Event button called");
        for (int i = 0; i < porte.Length; i++)
        {
            if (porte[i] != null)
            {
                porte[i].gameObject.SetActive(!porte[i].gameObject.activeSelf);
            }
        }
    }

    public void Event_Alarme(Transform[] al)
    {
        //Debug.Log("Event Alarme called");
    }

    public void CheckEvent(Event_int ei, Transform[] inf)
    {
        if (Event_int.alarme == ei)
        {
            Event_Alarme(inf);
        }
        if (Event_int.bouton == ei)
        {
            Event_Button(inf);
        }
    }
}
