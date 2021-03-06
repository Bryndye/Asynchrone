﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncadrementFeedback : MonoBehaviour
{
    [SerializeField]
    private GameObject porte;
    [SerializeField]
    private Animator myAnim;
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
        myAnim.gameObject.SetActive(isOpenned);

        SetEncadrementColor(!isOpenned);
    }

    public void SetEncadrementColor(bool open)
    {
        if (openEncadrement != null && closeEncadrement != null)
        {
            openEncadrement.SetActive(open);
            closeEncadrement.SetActive(!open);
        }
    }

    public void AnimDoor(bool open)
    {
        myAnim.gameObject.SetActive(true);
        if (open)
        {
            myAnim.SetTrigger("Open");
            Invoke(nameof(DesactivateDoor), 0.9f);
        }
        else
        {
            myAnim.SetTrigger("Close");
        }
    }

    private void DesactivateDoor()
    {
        myAnim.gameObject.SetActive(false);
    }
}
