using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public List<GameObject> SFX_Pool;
    int PoolIndex = 0;

    private void Awake()
    {
        if (Instance != this)
            Destroy(this);
    }

    private void Start()
    {
        for(int i = 0; i < 50; i++)
        {
            GameObject newAudio = Instantiate(Resources.Load<GameObject>("Audio/aSFX"), transform.position, transform.rotation, transform);
            SFX_Pool.Add(newAudio);
        }
    }

    public void GetASound(string mySoundName, Transform myNewParent, bool isUI = false)
    {
        GameObject toGive = SFX_Pool[PoolIndex];

        toGive.GetComponent<SFX>().NewJob(mySoundName, myNewParent, isUI);

        PoolIndex += 1;
        if (PoolIndex > 49)
            PoolIndex = 0;
    }
}
