using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleInteraction : MonoBehaviour
{
    private CameraManager cm;
    private Interaction[] interactions;

    [SerializeField]
    private Interaction interRbt, interHm;
    //private DoubleFeedback doubleFB;

    void Awake() { 
        interactions = GetComponentsInChildren<Interaction>();
        cm = CameraManager.Instance;
    }

    bool ok;
    bool activated;

    public Transform[] Portes;


    private void Update()
    {
        if (activated == true)
        {
            return;
        }
        if (interactions[0].activated && interactions[1].activated)
        {
            ok = true;
            CallEvent();
        }
        ActiveLight();
    }

    private void ActiveLight()
    {
        if (Portes[0].parent.TryGetComponent(out DoubleFeedback doubleFB))
        {
            if (interHm.activated)
            {
                doubleFB.ActiveHuman(true);
            }
            if (interRbt.activated)
            {
                doubleFB.ActiveRobot(true);
            }
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

                    if (Portes[i].parent.TryGetComponent(out EncadrementFeedback encafb))
                    {
                        encafb.SetColorLight();
                    }

                }
            }
        }
    }
}
