using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SmoothStatus { Open, Constant, Close };
public enum IsPressureAllowed { NoPressureAllowed, PressureAllowed };
public enum MusicComposition { TutoMusic, Layer1, Layer2, Layer3, Epilogue };

public class MusicManager : Singleton<MusicManager>
{
    public IsPressureAllowed myPressureSettings;
    public MusicComposition myComposition;
    SmoothStatus mySmoothStatus = SmoothStatus.Open;
    [HideInInspector]public int PressureKeepersCount;

    [Header("AudioSources")]
    AudioSource TutoMusic;
    AudioSource Layer1;
    AudioSource Layer2;
    AudioSource Layer3;
    AudioSource Layer4;
    AudioSource Epilogue;

    [HideInInspector] public List<AudioSource> ConstantPlaying;

    float PressureVolume = 0;
    float SpeedChange = 0.6f;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != this)
            Destroy(this);

        TutoMusic = transform.GetChild(0).GetComponent<AudioSource>(); TutoMusic.volume = 0;
        Layer1 = transform.GetChild(1).GetComponent<AudioSource>(); Layer1.volume = 0;
        Layer2 = transform.GetChild(2).GetComponent<AudioSource>(); Layer2.volume = 0;
        Layer3 = transform.GetChild(3).GetComponent<AudioSource>(); Layer3.volume = 0;
        Layer4 = transform.GetChild(4).GetComponent<AudioSource>(); Layer4.volume = 0;
        Epilogue = transform.GetChild(5).GetComponent<AudioSource>(); Epilogue.volume = 0;

        Layer4.volume = 0;

        switch (myComposition)
        {
            case MusicComposition.TutoMusic:
                TutoMusic.Play();
                ConstantPlaying.Add(TutoMusic);
                break;
            case MusicComposition.Layer1:
                Layer1.Play();
                Layer4.Play();
                ConstantPlaying.Add(Layer1);
                break;
            case MusicComposition.Layer2:
                Layer1.Play();
                Layer2.Play();
                Layer4.Play();
                ConstantPlaying.Add(Layer1);
                ConstantPlaying.Add(Layer2);
                break;
            case MusicComposition.Layer3:
                Layer1.Play();
                Layer2.Play();
                Layer3.Play();
                Layer4.Play();
                ConstantPlaying.Add(Layer1);
                ConstantPlaying.Add(Layer2);
                ConstantPlaying.Add(Layer3);
                break;
            case MusicComposition.Epilogue:
                Epilogue.Play();
                ConstantPlaying.Add(Epilogue);
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

        if(mySmoothStatus == SmoothStatus.Open)
        {
            for(int i = 0; i < ConstantPlaying.Count; i++)
            {
                ConstantPlaying[i].volume += Time.deltaTime / 4;
                ConstantPlaying[i].volume = Mathf.Clamp(ConstantPlaying[i].volume, 0f, 1f);
                if (ConstantPlaying[i].volume == 1)
                {
                    mySmoothStatus = SmoothStatus.Constant;
                }
            }
        }
        else if (mySmoothStatus == SmoothStatus.Close)
        {
            for (int i = 0; i < ConstantPlaying.Count; i++)
            {
                ConstantPlaying[i].volume -= Time.deltaTime;
                ConstantPlaying[i].volume = Mathf.Clamp(ConstantPlaying[i].volume, 0f, 1f);
                if (ConstantPlaying[i].volume == 0)
                {
                    mySmoothStatus = SmoothStatus.Constant;
                }
            }
        }
    }

    public void CloseMusic() => mySmoothStatus = SmoothStatus.Close;
}
