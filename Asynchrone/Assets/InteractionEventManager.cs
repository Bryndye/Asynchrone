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

    public void Event_Button()
    {
        Debug.Log("Event button called");
    }

    public void Event_Alarme()
    {
        Debug.Log("Event Alarme called");
    }

    public void CheckEvent(Event_int ei)
    {
        if (Event_int.alarme == ei)
        {
            Event_Alarme();
        }
        if (Event_int.bouton == ei)
        {
            Event_Button();
        }
    }
}
