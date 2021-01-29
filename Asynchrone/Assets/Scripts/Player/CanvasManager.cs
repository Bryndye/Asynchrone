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
    [SerializeField] string[] sentencesStock;
    private bool isRuntime;

    ManagerPlayers mp;
    PlayerController pc;

    [Header("Bts Spells")]
    float time;
    void Awake()
    {
        if (Instance != this)
            Destroy(this);
        mp = ManagerPlayers.Instance;
        anim = GetComponent<Animator>();
        anim.SetTrigger("Disappear");
    }

    #region SetBtn
    public void CallFctSpell(int i)
    {
        switch (i)
        {
            default:
                mp.Camera_Manager();
                break;

            case 0:
                mp.Camera_Manager();
                break;
            case 1:
                if (mp.onPlayer1 && !mp.Hm.robot_div)
                {
                    mp.Hm.StartDiv();
                }
                else
                {
                    Destroy(mp.Hm.robot_div);
                }
                break;
            case 2:
                if (!mp.Hm.intoMe)
                {
                    mp.RobotBackToHuman();
                }
                else
                {
                    mp.Hm.RobotIntoMe(false);
                }
                break;
            case 3:
                if (mp.onPlayer1)
                {
                    mp.Hm.CheckMask();
                }
                break;
        }
    }
    #endregion

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
        if (!isRuntime)
        {
            sentences = dia;
            isRuntime = true;
            index = 0;
            StartCoroutine(Type());
        }
        else
        {
            sentencesStock = dia;
        }
    }

    public void NextDialogue()
    {
        if (index < sentences.Length - 1 && sentences != null)
        {
            index++;
            dialogueHere.text = null;
            StartCoroutine(Type());
            //Debug.Log("Next");
        }
        else
        {
            sentences = null;
            dialogueHere.text = null;
            isRuntime = false;
        }
        if (sentencesStock != null && sentencesStock.Length > 0 && sentences ==  null)
        {
            isRuntime = true;
            index = 0;
            sentences = sentencesStock;
            sentencesStock = null;
            dialogueHere.text = null;
            StartCoroutine(Type());
            //Debug.Log("next dialogues");
        }
        skip = false;
    }

    [SerializeField] float latence = 0.1f;

    #endregion
    void Update()
    {
        if (isRuntime && dialogueHere.text == sentences[index] && !skip)
        {
            skip = true;
            Invoke(nameof(NextDialogue), 2f);
        }
    }
}
