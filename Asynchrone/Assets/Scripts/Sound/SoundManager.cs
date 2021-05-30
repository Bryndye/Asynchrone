using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private AudioMixer soundMixer;

    [HideInInspector]public List<GameObject> SFX_Pool;
    public int PoolIndex = 0;

    private void Awake()
    {
        if (Instance != this)
            Destroy(this);

        soundMixer.SetFloat("SoundVolume", ConvertedValue(PlayerPrefs.GetFloat("SoundVolume")));
        musicMixer.SetFloat("MusicVolume", ConvertedValue(PlayerPrefs.GetFloat("MusicVolume")));
    }

    private void Start()
    {
        for(int i = 0; i < 50; i++)
        {
            GameObject newAudio = Instantiate(Resources.Load<GameObject>("Audio/aSFX"), transform.position, transform.rotation, transform);
            SFX_Pool.Add(newAudio);
        }

        if (SceneManager.GetActiveScene().buildIndex > 1)
            GetASound("Ascenseur_Ouverture", null, true);
    }

    public void GetASound(string mySoundName, Transform myNewParent, bool isUI = false)
    {
        GameObject toGive = SFX_Pool[PoolIndex];

        toGive.GetComponent<SFX>().NewJob(mySoundName, myNewParent, isUI);

        PoolIndex += 1;
        if (PoolIndex > 49)
            PoolIndex = 0;
    }

    float ConvertedValue(float ValueToGive) { return -80f + ValueToGive * 80f; }
    float GetValue(float MixerValue) { return MixerValue / 80 + 1; }
}
