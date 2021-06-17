using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInteraction : MonoBehaviour
{
    SpawnManager spawnManager;
    Interaction myInteraction;

    [Header("Save data Interaction")]
    public bool ActivatedSave;
    bool activePinceSave;
    bool[] activePortes;


    private void Awake()
    {
        spawnManager = SpawnManager.Instance;
        myInteraction = GetComponent<Interaction>();

        activePortes = new bool[myInteraction.Portes.Length];
    }

    private void Start()
    {
        AllSavesInteraction.Instance.savesInteraction.Add(this);
    }


    public void SaveData()
    {
        //Debug.Log("Save data inter : " + gameObject.name);

        ActivatedSave = myInteraction.Activated;

        if (myInteraction.interType == Interaction.interactionType.Distributeur)
        {
            activePinceSave = myInteraction.ActivePince;
        }
        else
        {
            for (int i = 0; i < activePortes.Length; i++)
            {
                activePortes[i] = myInteraction.Portes[i].gameObject.activeSelf;
            }
        }
    }

    public void LoadData()
    {
        myInteraction.Activated = ActivatedSave;
        myInteraction.ActivePince = activePinceSave;

        if (ActivatedSave == false)
        {
            myInteraction.InteractionDone(true);

            if (myInteraction.interType == Interaction.interactionType.Pince)
            {
                for (int i = 0; i < myInteraction.Portes.Length; i++)
                {
                    myInteraction.Portes[i].position = myInteraction.PosInitials[i];
                }
            }
            else
            {
                for (int i = 0; i < activePortes.Length; i++)
                {
                    myInteraction.Portes[i].gameObject.SetActive(activePortes[i]);

                    if (myInteraction.Portes[i].parent.TryGetComponent(out EncadrementFeedback encafb))
                    {
                        encafb.SetEncadrementColor(!activePortes[i]);
                        encafb.AnimDoor(!activePortes[i]);
                    }
                }
            }
        }
    }
}
