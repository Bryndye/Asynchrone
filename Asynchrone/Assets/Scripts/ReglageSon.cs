using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReglageSon : MonoBehaviour
{
    [SerializeField]
    private Slider sliderMusique;
    [SerializeField]
    private Text musicVolumeText;

    private void Start()
    {
        Debug.Log(PlayerPrefs.GetFloat("musicVolume"));
        OnChangeMusic(PlayerPrefs.GetFloat("musicVolume"));
    }


    public void OnChangeMusic(float value)
    {
        PlayerPrefs.SetFloat("musicVolume", value);
        musicVolumeText.text = Mathf.RoundToInt(value * 100).ToString();
        sliderMusique.value = value;
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.VolumeMax = value;
        }
    }
}
