using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncadrementFeedback : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer renderer;
    public bool isOpenned;

    //[Header("Color")]
    [SerializeField]
    private Material closeMat, openMat;
    //[SerializeField]
    //private Color closeColor;
    //[SerializeField]
    //private Color openColor;

    MaterialPropertyBlock _mpb;
    public MaterialPropertyBlock Mpb
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

    private void Start()
    {
        SetColorLight();
    }

    private void SetColorLight()
    {
        if (isOpenned)
        {
            //Mpb.SetColor("Color", closeColor);
            renderer.materials[1] = openMat;
            Debug.Log(renderer.materials[1] + " open");
        }
        else
        {
            //Mpb.SetColor("Color", openColor);
            renderer.materials[1] = closeMat;
            Debug.Log(renderer.materials[1] + " close");
        }

        //renderer.SetPropertyBlock(Mpb);
    }

}
