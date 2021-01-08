using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public Event_int Event_;
    bool activated;

    [SerializeField] Transform[] Influence;

    InteractionEventManager iem;

    public void CallEvent()
    {
        if (!activated)
        {
            //Debug.Log("Event called");
            activated = true;

            iem.CheckEvent(Event_, Influence);
        }
    }

    private void Awake()
    {
        iem = InteractionEventManager.Instance;
    }
}
