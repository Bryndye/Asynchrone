using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleInteraction : MonoBehaviour
{
    private CameraManager cm;
    private Interaction[] interactions;

    void Awake() { 
        interactions = GetComponentsInChildren<Interaction>();
        cm = CameraManager.Instance;
    }

    bool ok;
    bool activated;

    public Transform[] Portes;


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
            activated = true;

            for (int i = 0; i < Portes.Length; i++)
            {
                if (Portes[i] != null)
                {
                    Portes[i].gameObject.SetActive(!Portes[i].gameObject.activeSelf);
                    if (cm != null)
                        cm.GetTargetPorte(Portes);
                }
            }
        }
    }
}
