using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEDoubleBTN : MonoBehaviour
{
    private CameraManager cm;
    private Interaction[] interactions;

    void Awake() { 
        interactions = GetComponentsInChildren<Interaction>();
        cm = CameraManager.Instance;
    }

    bool ok;
    bool activated;

    public Transform[] Influence;


    private void Update()
    {
        if (interactions[0].activated && interactions[1].activated)
        {
            ok = true;
            CallEvent();
        }

    }

    public void CallEvent()
    {
        if (!activated)
        {
            //Debug.Log("Event called");
            activated = true;

            for (int i = 0; i < Influence.Length; i++)
            {
                if (Influence[i] != null)
                {
                    Influence[i].gameObject.SetActive(!Influence[i].gameObject.activeSelf);
                    if (cm != null)
                        cm.GetTargetPorte(Influence);
                }
            }
        }
    }
}
