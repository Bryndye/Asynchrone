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

    bool saveDone = false;
    bool loadDone = false;

    private void Awake()
    {
        spawnManager = SpawnManager.Instance;
        myInteraction = GetComponent<Interaction>();

        activePortes = new bool[myInteraction.Portes.Length];
    }

    private void FixedUpdate()
    {
        if (spawnManager.mySavingState == SavingState.Running && !saveDone)
        {
            //save data
            saveDone = true;
            //Debug.Log("Save data interaction : " + gameObject.name);

            ActivatedSave = myInteraction.Activated;

            if (myInteraction.Pince)
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
        else
        {
            saveDone = false;
        }
        if (spawnManager.mySpawnSituation == SpawnSituation.DeathProcess && !loadDone)
        {
            //load save
            loadDone = true;
            //Debug.Log("Load data interaction : " + gameObject.name);

            myInteraction.Activated = ActivatedSave;
            myInteraction.ActivePince = activePinceSave;

            if (ActivatedSave == false)
            {
                myInteraction.InteractionDone(true);

                if (myInteraction.Pince)
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
                            encafb.SetEncadrementColor();
                        }
                    }
                }
            }
        }
        else
        {
            loadDone = false;
        }
    }
}
