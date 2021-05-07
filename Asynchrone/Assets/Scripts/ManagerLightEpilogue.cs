using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerLightEpilogue : MonoBehaviour
{
    [SerializeField]
    private Light[] lightGroup;
    [SerializeField]
    private Color32 colorAlert;
    [SerializeField]
    private Color32 colorBase;

    [SerializeField]
    private Renderer[] screenGroup;
    [SerializeField]
    private Color colorAlertScreen;
    [SerializeField]
    private Color colorBaseScreen;

    MaterialPropertyBlock _mpb;
    public MaterialPropertyBlock MPB
    {
        get
        {
            if (_mpb == null)
            {
                _mpb = new MaterialPropertyBlock();
            }
            return _mpb;
        }
    }

    private void Awake()
    {
        colorBase = lightGroup[0].color;
        colorBaseScreen = screenGroup[0].materials[1].GetColor("_EmissionColor");

        //ChangeColorLigh();
        //ChangeColorScreen();
    }

    public void ChangeColorLigh()
    {
        for (int i = 0; i < lightGroup.Length; i++)
        {
            lightGroup[i].color = colorAlert;
        }
    }

    public void ChangeColorScreen()
    {
        MPB.SetColor("_EmissionColor", colorAlertScreen);
        for (int i = 0; i < screenGroup.Length; i++)
        {
            screenGroup[i].SetPropertyBlock(MPB);
        }
    }
}
