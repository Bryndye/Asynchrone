using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSounds : MonoBehaviour
{
    SoundManager SM;

    private void Awake()
    {
        SM = SoundManager.Instance;
    }

    public void MakeHumanRunSound()
    {
        int rnd = Random.Range(0, 4) + 1;
        SM.GetASound("RunPack/Run_" + rnd, transform);
    }
}
