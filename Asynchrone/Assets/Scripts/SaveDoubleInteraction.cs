using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDoubleInteraction : MonoBehaviour
{
    SpawnManager spawnManager;

    SaveInteraction[] mySaveInteractions;
    DoubleInteraction myDoubleInteraction;

    [Header("Save Data")]
    bool activatedSave;
    bool[] activePortes;

    bool saveDone = false;
    bool loadDone = false;

    private void Awake()
    {
        spawnManager = SpawnManager.Instance;
        myDoubleInteraction = GetComponent<DoubleInteraction>();
        mySaveInteractions = GetComponentsInChildren<SaveInteraction>();

        activePortes = new bool[myDoubleInteraction.Portes.Length];
    }

    private void Update()
    {
        if (spawnManager.mySavingState == SavingState.Running && !saveDone)
        {
            //save data
            saveDone = true;
            //Debug.Log("Save data Doubleinteraction : " + saveDone);

            activatedSave = myDoubleInteraction.Activated;

            for (int i = 0; i < activePortes.Length; i++)
            {
                activePortes[i] = myDoubleInteraction.Portes[i].gameObject.activeSelf;
            }

        }
        else if (spawnManager.mySavingState == SavingState.None)
        {
            saveDone = false;
        }
        if (spawnManager.mySpawnSituation == SpawnSituation.DeathProcess && !loadDone)
        {
            //load save
            loadDone = true;
            //Debug.Log("Load data Doubleinteraction : " + gameObject.name);

            myDoubleInteraction.Activated = activatedSave;

            if (activatedSave == false)
            {
                //myDoubleInteraction.InteractionDone(true);

                for (int i = 0; i < activePortes.Length; i++)
                {
                    myDoubleInteraction.Portes[i].gameObject.SetActive(activePortes[i]);

                    if (myDoubleInteraction.Portes[i].parent.TryGetComponent(out EncadrementFeedback encafb))
                    {
                        encafb.SetEncadrementColor();
                    }
                }

                if (myDoubleInteraction.Portes[0].parent.TryGetComponent(out DoubleFeedback doubleFeedback))
                {
                    doubleFeedback.ActiveHuman(mySaveInteractions[1].ActivatedSave);
                    doubleFeedback.ActiveRobot(mySaveInteractions[0].ActivatedSave);
                }
            }
        }
        else if (spawnManager.mySpawnSituation == SpawnSituation.Playing)
        {
            loadDone = false;
        }
    }
}
