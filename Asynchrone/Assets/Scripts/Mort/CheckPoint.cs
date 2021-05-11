using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    ManagerPlayers mp;
    SpawnMANAGER sm;
    CanvasManager cm;
    private bool done;

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
            if (mp.PlayerRobot != null)
            {
                sm.GetSpawn(mp.PlayerHuman.position, mp.PlayerRobot.position, false);
            }
            else
            {
                sm.GetSpawn(mp.PlayerHuman.position, Vector3.zero, false);
            }
            sm.AiCheck();
            cm.ActiveCheckpointText();
            done = true;
        }
    }
}
