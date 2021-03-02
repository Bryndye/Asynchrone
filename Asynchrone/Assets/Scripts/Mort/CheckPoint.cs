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
            sm.GetSpawn(mp.Player1.position, mp.Player2.position, false);
            cm.ActiveCheckpointText();
            done = true;
        }
    }
}
