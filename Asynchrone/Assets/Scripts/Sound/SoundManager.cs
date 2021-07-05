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
    [HideInInspector] public int PoolIndex = 0;

    public AnimationCurve SoundTranslate;

    private void Awake()
    {
        if (Instance != this)
            Destroy(this);
    }

    private void Start()
    {
        soundMixer.SetFloat("SoundVolume", ConvertedValue(PlayerPrefs.GetFloat("SoundVolume")));
        musicMixer.SetFloat("MusicVolume", ConvertedValue(PlayerPrefs.GetFloat("MusicVolume")));

        for (int i = 0; i < 50; i++)
        {
            GameObject newAudio = Instantiate(Resources.Load<GameObject>("Audio/aSFX"), transform.position, transform.rotation, transform);
            SFX_Pool.Add(newAudio);
        }

        if (SceneManager.GetActiveScene().buildIndex > 1 && SceneManager.GetActiveScene().buildIndex != 8)
            GetASound("Ascenseur_Ouverture", null, true);
    }

    public void GetASound(string mySoundName, Transform myNewParent, bool isUI = false)
    {
        GameObject toGive = transform.GetChild(0).gameObject;

        toGive.GetComponent<SFX>().NewJob(mySoundName, myNewParent, isUI);

        /*PoolIndex += 1;
        if (PoolIndex > 49)
            PoolIndex = 0;*/
    }

    float ConvertedValue(float ValueToGive) { return -80f + GetCurveTranslated(ValueToGive) * 80f; }
    float GetValue(float MixerValue) { return MixerValue / 80 + 1; }

    float GetCurveTranslated(float ValueToGive, bool inX = true)
    {
        float toReturn = 0;

        toReturn = SoundTranslate.Evaluate(ValueToGive);

        return toReturn;
    }
}
