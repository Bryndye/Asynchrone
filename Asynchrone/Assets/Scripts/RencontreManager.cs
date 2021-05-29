using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RencontreManager : MonoBehaviour
{
    public Animator animRbt;
    public Animator animHm;
    public int indexOfNextlevel;

    CanvasManager cm
    {
        get
        {
            return CanvasManager.Instance;
        }
    }

    SoundManager SM
    {
        get
        {
            return SoundManager.Instance;
        }
    }
    MusicManager MM
    {
        get
        {
            return MusicManager.Instance;
        }
    }

    private void OnEnable()
    {
        animHm.SetTrigger("load");
        animRbt.SetTrigger("load");

        Invoke(nameof(LoadLevel), 2f);
    }

    private void LoadLevel()
    {
        PlayerPrefs.SetInt("indexLevel", indexOfNextlevel);

        cm.anim.SetTrigger("Transition");
        SM.GetASound("Ascenseur_Fermeture", transform);
        MM.CloseMusic();

        SceneManager.LoadScene(7);
    }

}
