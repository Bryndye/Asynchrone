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

        PosInitials = new Vector3[Portes.Length];
        for (int i = 0; i < Portes.Length; i++)
        {
            PosInitials[i] = Portes[i].position;
        }
    }

    [HideInInspector] public bool Activated;

    public Transform[] Portes;
    [HideInInspector] public Vector3[] PosInitials;

    

    private void Update()
    {
        if (Activated == true)
        {
            return;
        }
        if (interactions[0].Activated && interactions[1].Activated)
        {
            CallEvent();
        }
        ActiveLight();
    }

    private void ActiveLight()
    {
        if (Portes[0].parent.TryGetComponent(out DoubleFeedback doubleFB))
        {
            if (interHm.Activated)
            {
                doubleFB.ActiveHuman(true);
            }
            if (interRbt.Activated)
            {
                doubleFB.ActiveRobot(true);
            }
        }
    }

    public void CallEvent()
    {
        if (!Activated)
        {
            Activated = true;

            for (int i = 0; i < Portes.Length; i++)
            {
                if (Portes[i] != null)
                {
                    Portes[i].gameObject.SetActive(!Portes[i].gameObject.activeSelf);
                    if (cm != null)
                        cm.GetTargetPorte(Portes);

                    if (Portes[i].parent.TryGetComponent(out EncadrementFeedback encafb))
                    {
                        encafb.SetEncadrementColor();
                    }

                }
            }
        }
    }
}
