using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : Singleton<CanvasManager>
{
    Animator anim;
    public Text dialogueHere;
    private bool isRuntime;
    private string diaTemp;
    private int index;
    private char[] charStock;

    void Awake()
    {
        if (Instance != this)
            Destroy(this);

        anim = GetComponent<Animator>();
        anim.SetTrigger("Disappear");
        index = 0;
    }

    public void BandeAppear()
    {
        anim.SetTrigger("Appear");
    }
    public void BandeDisAppear()
    {
        anim.SetTrigger("Disappear");
    }

    #region Daliogue
    public void StartDiaEffect(string dia)
    {
        isRuntime = false;
        charStock = dia.ToCharArray();
        dialogueHere.text = null;
        diaTemp = dia;
        index = 0;
        isRuntime = true;
    }
    public void EffectDialogue()
    {
        dialogueHere.text += charStock[index];
        index++;

        if (index >= charStock.Length || diaTemp == null)
        {
            charStock = null;
            isRuntime = false;
            Debug.Log("termine");
        }
    }

    public void FinishEffect()
    {
        if (charStock == null)
        {
            dialogueHere.text = null;
        }
        if (isRuntime)
        {
            dialogueHere.text = diaTemp;
            isRuntime = false;
            charStock = null;
        }
    }

    float time;
    [SerializeField] float latence = 0.1f;

    #endregion
    void Update()
    {
        if (isRuntime)
        {
            time += Time.deltaTime;
            if (time >= latence)
            {
                EffectDialogue();
                time = 0;
            }
        }
        else
        {
            time = 0;
        }
    }
}
