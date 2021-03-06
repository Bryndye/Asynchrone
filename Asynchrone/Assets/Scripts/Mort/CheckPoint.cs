﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    ManagerPlayers mp;
    SpawnMANAGER sm;
    CanvasManager cm;
    private bool done;

    [SerializeField]
    private Transform spawnHm, spawnRbt;

    private void Awake()
    {
        sm = SpawnMANAGER.Instance;
        mp = ManagerPlayers.Instance;
        cm = CanvasManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !done)
        {
            if (mp.PlayerRobotTransform != null)
            {
                sm.GetSpawn(spawnHm, spawnRbt);
            }
            else
            {
                sm.GetSpawn(spawnHm, spawnRbt);
            }
            AllSavesInteraction.Instance.SavesData();
            sm.AiCheck();
            cm.ActiveCheckpointText();
            done = true;
        }
    }
}
