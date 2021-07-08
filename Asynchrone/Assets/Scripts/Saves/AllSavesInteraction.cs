using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllSavesInteraction : MonoBehaviour
{
    public static AllSavesInteraction Instance;

    public List<SaveInteraction> savesInteraction;
    public List<SaveDoubleInteraction> savesDoubleInteraction;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("All saves destroyed");
            Destroy(this);
        }
        Instance = this;
    }

    public void SavesData()
    {
        for (int i = 0; i < savesInteraction.Count; i++)
        {
            savesInteraction[i].SaveData();
        }
        for (int i = 0; i < savesDoubleInteraction.Count; i++)
        {
            savesDoubleInteraction[i].SaveData();
        }
    }

    public void LoadSaves()
    {
        for (int i = 0; i < savesInteraction.Count; i++)
        {
            savesInteraction[i].LoadData();
        }
        for (int i = 0; i < savesDoubleInteraction.Count; i++)
        {
            savesDoubleInteraction[i].LoadData();
        }
    }
}
