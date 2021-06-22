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

    [Header("Sounds")]
    string InteractionSoundName;
    SoundManager SoundM;

    bool doneH;
    bool doneR;
    void Awake() { 
        interactions = GetComponentsInChildren<Interaction>();
        cm = CameraManager.Instance;
        SoundM = SoundManager.Instance;

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
            CallDoor();
        }
        //est appele en runtime
        ActiveLight();
    }

    public void ActiveLight()
    {
        if (doneH && doneR)
        {
            return;
        }
        if (Portes[0].parent.TryGetComponent(out DoubleFeedback doubleFB))
        {
            if (interHm.Activated && !doneH)
            {
                doneH = true;
                doubleFB.ActiveHuman(true);
                cm.GetTargetPorte(Portes);
            }
            if (interRbt.Activated && !doneR)
            {
                doneR = true;
                doubleFB.ActiveRobot(true);
                cm.GetTargetPorte(Portes);
            }
        }
    }

    public void CallDoor()
    {
        if (!Activated)
        {
            Activated = true;

            //Invoke(nameof(DesactivatePorte), 1f);


            for (int i = 0; i < Portes.Length; i++)
            {
                if (Portes[i] != null)
                {
                    Portes[i].gameObject.SetActive(!Portes[i].gameObject.activeSelf);

                    if (Portes[i].parent.TryGetComponent(out EncadrementFeedback encafb))
                    {
                        encafb.SetEncadrementColor(!Portes[i].gameObject.activeSelf);
                        encafb.AnimDoor(!Portes[i].gameObject.activeSelf);
                    }
                }
            }

            if (InteractionSoundName != "")
            {
                SoundM.GetASound("Porte", transform);
            }
        }
    }

    private void DesactivatePorte()
    {
        //porte.gameObject.SetActive(!porte.activeSelf);
        for (int i = 0; i < Portes.Length; i++)
        {
            Portes[i].gameObject.SetActive(!Portes[i].gameObject.activeSelf);
            Debug.Log("lol");
        }
    }
}
