using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    SpawnMANAGER SM;

    SoundManager SoundM;
    AudioSource myAudiosource;

    [Header("Timer")]
    float PlayingTime;
    float PlayingLatence;
    bool Playing;

    bool MustLower => SM.mySpawnSituation == SpawnSituation.DeathProcess;

    private void Awake()
    {
        SoundM = SoundManager.Instance;
        SM = SpawnMANAGER.Instance;
        myAudiosource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Playing)
        {
            PlayingTime += Time.deltaTime;
            if(PlayingTime >= PlayingLatence)
            {
                PlayingTime = 0;
                Playing = false;
                EndJob();
            }
        }

        if (myAudiosource.volume != 0 && MustLower)
        {
            myAudiosource.volume -= Time.deltaTime;
            myAudiosource.volume = Mathf.Clamp(myAudiosource.volume, 0f, 1f);
        }
        else if (myAudiosource.volume != 1 && !MustLower)
        {
            myAudiosource.volume = Mathf.Clamp(myAudiosource.volume, 0f, 1f);
            myAudiosource.volume += Time.deltaTime;
        }
    }

    public void NewJob(string mySoundName, Transform myNewParent, bool isUI)
    {
        myAudiosource.clip = Resources.Load<AudioClip>("Audio/SFXClips/" + mySoundName);
        PlayingLatence = myAudiosource.clip.length;

        if (isUI && CameraManager.Instance != null)
        {
            transform.parent = CameraManager.Instance.transform;
            myAudiosource.spatialBlend = 0;
        }
        else
        {
            transform.parent = myNewParent;
            myAudiosource.spatialBlend = 1;
        }
        transform.localPosition = Vector3.zero;

        myAudiosource.Play();
        Playing = true;
    }

    void EndJob()
    {
        transform.parent = SoundM.transform;
        transform.localPosition = Vector3.zero;
    }
}
