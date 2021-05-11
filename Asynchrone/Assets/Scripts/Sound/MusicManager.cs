using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IsPressureAllowed { NoPressureAllowed, PressureAllowed };
public enum MusicComposition { TutoMusic, Layer1, Layer2, Layer3 };

public class MusicManager : Singleton<MusicManager>
{
    public IsPressureAllowed myPressureSettings;
    public MusicComposition myComposition;

    [HideInInspector]public int PressureKeepersCount;

    [Header("AudioSources")]
    AudioSource TutoMusic;
    AudioSource Layer1;
    AudioSource Layer2;
    AudioSource Layer3;
    AudioSource Layer4;

    float PressureVolume = 0;
    float SpeedChange = 0.6f;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != this)
            Destroy(this);

        TutoMusic = transform.GetChild(0).GetComponent<AudioSource>();
        Layer1 = transform.GetChild(1).GetComponent<AudioSource>();
        Layer2 = transform.GetChild(2).GetComponent<AudioSource>();
        Layer3 = transform.GetChild(3).GetComponent<AudioSource>();
        Layer4 = transform.GetChild(4).GetComponent<AudioSource>();

        Layer4.volume = 0;

        switch (myComposition)
        {
            case MusicComposition.TutoMusic:
                TutoMusic.Play();
                break;
            case MusicComposition.Layer1:
                Layer1.Play();
                Layer4.Play();
                break;
            case MusicComposition.Layer2:
                Layer1.Play();
                Layer2.Play();
                Layer4.Play();
                break;
            case MusicComposition.Layer3:
                Layer1.Play();
                Layer2.Play();
                Layer3.Play();
                Layer4.Play();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (myPressureSettings == IsPressureAllowed.PressureAllowed)
        {
            if (PressureKeepersCount > 0 && PressureVolume != 1)
            {
                PressureVolume = 1;
                PressureVolume = Mathf.Clamp(PressureVolume, 0f, 1f);
                Layer4.volume = PressureVolume;
            }
            else if (PressureKeepersCount == 0 && PressureVolume != 0)
            {
                PressureVolume -= SpeedChange * Time.deltaTime;
                PressureVolume = Mathf.Clamp(PressureVolume, 0f, 1f);
                Layer4.volume = PressureVolume;
            }
        }
    }
}
