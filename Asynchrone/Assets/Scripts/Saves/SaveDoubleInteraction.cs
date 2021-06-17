using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDoubleInteraction : MonoBehaviour
{
    SpawnMANAGER spawnManager;

    SaveInteraction[] mySaveInteractions;
    DoubleInteraction myDoubleInteraction;

    [Header("Save Data")]
    bool activatedSave;
    bool[] activePortes;


    private void Awake()
    {
        spawnManager = SpawnMANAGER.Instance;
        myDoubleInteraction = GetComponent<DoubleInteraction>();
        mySaveInteractions = GetComponentsInChildren<SaveInteraction>();

        activePortes = new bool[myDoubleInteraction.Portes.Length];
    }

    private void Start()
    {
        AllSavesInteraction.Instance.savesDoubleInteraction.Add(this);
    }

    public void SaveData()
    {
        //Debug.Log("Save data Doubleinteraction : ");

        activatedSave = myDoubleInteraction.Activated;

        for (int i = 0; i < activePortes.Length; i++)
        {
            activePortes[i] = myDoubleInteraction.Portes[i].gameObject.activeSelf;
        }
    }

    public void LoadData()
    {
        //Debug.Log("load data Doubleinteraction : ");

        myDoubleInteraction.Activated = activatedSave;

        if (activatedSave == false)
        {

            for (int i = 0; i < activePortes.Length; i++)
            {
                myDoubleInteraction.Portes[i].gameObject.SetActive(activePortes[i]);

                if (myDoubleInteraction.Portes[i].parent.TryGetComponent(out EncadrementFeedback encafb))
                {
                    encafb.SetEncadrementColor(!activePortes[i]);
                    encafb.AnimDoor(!activePortes[i]);
                }
            }

            if (myDoubleInteraction.Portes[0].parent.TryGetComponent(out DoubleFeedback doubleFeedback))
            {
                doubleFeedback.ActiveHuman(mySaveInteractions[1].ActivatedSave);
                doubleFeedback.ActiveRobot(mySaveInteractions[0].ActivatedSave);
            }
        }
    }
}
