using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ReglageSon : MonoBehaviour
{
    [Header("Music Elements")]
    float musicVolume;
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private Slider sliderMusique;
    [SerializeField] private Text musicVolumeText;
    [Space]
    [Header("Sound Elements")]
    float soundVolume;
    [SerializeField] private AudioMixer soundMixer;
    [SerializeField] private Slider sliderSound;
    [SerializeField] private Text soundVolumeText;

    private void Start()
    {
        LoadMusicValues();
        LoadSoundValues();
    }

    void LoadMusicValues()
    {
        musicMixer.GetFloat("MusicVolume", out musicVolume);
        musicVolume = GetValue(musicVolume);
        sliderMusique.value = musicVolume;
        musicVolumeText.text = Mathf.RoundToInt(musicVolume * 100) + "%";
    }

    public void OnSliderMusicChange()
    {
        musicVolume = sliderMusique.value;
        musicVolumeText.text = Mathf.RoundToInt(musicVolume * 100) + "%";
        musicMixer.SetFloat("MusicVolume", ConvertedValue(musicVolume));
    }

    void LoadSoundValues()
    {
        soundMixer.GetFloat("SoundVolume", out soundVolume);
        soundVolume = GetValue(soundVolume);
        sliderSound.value = soundVolume;
        soundVolumeText.text = Mathf.RoundToInt(soundVolume * 100) + "%";
    }

    public void OnSliderSoundChange()
    {
        soundVolume = sliderSound.value;
        soundVolumeText.text = Mathf.RoundToInt(soundVolume * 100) + "%";
        soundMixer.SetFloat("SoundVolume", ConvertedValue(soundVolume));
    }

    float ConvertedValue(float ValueToGive) { return -80f + ValueToGive * 80f; }
    float GetValue(float MixerValue) { return MixerValue / 80 + 1; }
}
