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
            if (mp.PlayerRobotTransform != null)
            {
                sm.GetSpawn(mp.PlayerHumanTransform.position, mp.PlayerRobotTransform.position, false);
            }
            else
            {
                sm.GetSpawn(mp.PlayerHumanTransform.position, Vector3.zero, false);
            }
            sm.AiCheck();
            cm.ActiveCheckpointText();
            done = true;
        }
    }
}
