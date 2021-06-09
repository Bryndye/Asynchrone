using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncadrementFeedback : MonoBehaviour
{
    [SerializeField]
    private GameObject porte;
    //private MeshRenderer renderer;
    private bool isOpenned 
    {
        get
        {
            return porte.activeSelf;
        }
    }

    [SerializeField]
    private GameObject closeEncadrement, openEncadrement;
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

    public void SetColorLight()
    {
        //if (isOpenned)
        //{
        //    //Mpb.SetColor("Color", closeColor);
        //    //renderer.materials[1] = openMat;
        //}
        //else
        //{
        //    //Mpb.SetColor("Color", openColor);
        //    //renderer.materials[1] = closeMat;
        //}

        //isOpenned = open;

        openEncadrement.SetActive(!isOpenned);
        closeEncadrement.SetActive(isOpenned);

        //renderer.SetPropertyBlock(Mpb);
    }

}
