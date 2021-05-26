using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    SoundManager SoundM;
    AudioSource myAudiosource;

    [Header("Timer")]
    float PlayingTime;
    float PlayingLatence;
    bool Playing;

    private void Awake()
    {
        SoundM = SoundManager.Instance;
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
