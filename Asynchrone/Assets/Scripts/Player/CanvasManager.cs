using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : Singleton<CanvasManager>
{
    //[SerializeField] Button bt_continue;
    Animator anim;
    public Text dialogueHere;
    private bool skip;
    private int index =0;
    public string[] sentences;
    private bool isRuntime;

    void Awake()
    {
        if (Instance != this)
            Destroy(this);

        anim = GetComponent<Animator>();
        anim.SetTrigger("Disappear");
    }

    #region visual
    public void BandeAppear()
    {
        anim.SetTrigger("Appear");
    }
    public void BandeDisAppear()
    {
        anim.SetTrigger("Disappear");
    }
    #endregion

    #region Daliogue
    IEnumerator Type()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            dialogueHere.text += letter;
            yield return new WaitForSeconds(latence);
        }
    }

    public void StartDiaEffect(string[] dia)
    {
        sentences = dia;
        isRuntime = true;
        index = 0;
        StartCoroutine(Type());
    }

    public void NextDialogue()
    {
        if (skip)
        {
            if (index < sentences.Length - 1)
            {
                index++;
                dialogueHere.text = null;
                StartCoroutine(Type());
            }
            else
            {
                dialogueHere.text = null;
                isRuntime = false;
            }
            skip = false;
        }
    }

    [SerializeField] float latence = 0.1f;

    #endregion
    void Update()
    {
        if (isRuntime && dialogueHere.text == sentences[index])
        {
            skip = true;
        }
    }
}
