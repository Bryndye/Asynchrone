using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncadrementFeedback : MonoBehaviour
{
    [SerializeField]
    private GameObject porte;
    private bool isOpenned 
    {
        get
        {
            return porte.activeSelf;
        }
    }

    [SerializeField]
    private GameObject closeEncadrement, openEncadrement;

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
        SetEncadrementColor();
    }

    public void SetEncadrementColor()
    {
        openEncadrement.SetActive(!isOpenned);
        closeEncadrement.SetActive(isOpenned);
    }

}
