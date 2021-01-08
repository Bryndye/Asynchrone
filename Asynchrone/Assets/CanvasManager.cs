using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : Singleton<CanvasManager>
{
    Animator anim;

    void Awake()
    {
        if (Instance != this)
            Destroy(this);

        anim = GetComponent<Animator>();
        anim.SetTrigger("Disappear");
    }

    public void BandeAppear()
    {
        anim.SetTrigger("Appear");
    }
    public void BandeDisAppear()
    {
        anim.SetTrigger("Disappear");
    }
}
