using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public enum Event_int
{
    bouton,
    PorteCondition
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

    public void Event_Button(Interaction it)
    {
        Transform[] porte = it.Influence;
        //Debug.Log("Event button called");
        for (int i = 0; i < porte.Length; i++)
        {
            if (porte[i] != null)
            {
                porte[i].gameObject.SetActive(!porte[i].gameObject.activeSelf);
            }
        }
    }

    public void CheckEvent(Event_int ei, Interaction it)
    {
        if (Event_int.bouton == ei)
        {
            Event_Button(it);
        }
    }
}
