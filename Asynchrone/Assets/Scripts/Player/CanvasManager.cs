using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : Singleton<CanvasManager>
{
    //[SerializeField] Button bt_continue;
    CameraManager cm;
    ManagerPlayers mp;
    PlayerController pc;

    [HideInInspector] public Animator anim;
    [SerializeField] private GameObject checkpoint_t;

    [Header("Char anim")]
    public Text dialogueHere;
    private bool skip;
    private int index =0;
    [HideInInspector] public string[] sentences;
    [HideInInspector] string[] sentencesStock;
    [HideInInspector] public AudioClip[] audioc;
    [HideInInspector] AudioClip[] acStock;
    private bool isRuntime;

    [Header("Bts Spells")]
    public Text QuelPlayer;
    public GameObject UIHuman;
    public GameObject UIRobot;
    [SerializeField] private Button bt_divRbt;

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
                if (!mp.onPlayer1 && !mp.Rbt.robot_div)
                {
                    mp.Rbt.StartDiv();
                }
                else
                {
                    Destroy(mp.Rbt.robot_div);
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

    public void ActiveCheckpointText()
    {
        checkpoint_t.SetActive(true);
        Invoke(nameof(DesactiveI),2f);
    }

    private void DesactiveI()
    {
        checkpoint_t.SetActive(false);
    }

    private void NombreDiversion()
    {
        if (bt_divRbt != null)
        {
            if (mp.Rbt.DivStock > 0)
            {
                bt_divRbt.interactable = true;
            }
            else
            {
                bt_divRbt.interactable = false;
            }
        }
    }
    #endregion

    #region Daliogue
    IEnumerator Type()
    {
        LaunchAudio();
        foreach (char letter in sentences[index].ToCharArray())
        {
            dialogueHere.text += letter;
            yield return new WaitForSeconds(latence);
        }
    }
    private void LaunchAudio()
    {
        if (audioc.Length > 0 && index < audioc.Length)
        {
            //cm.AS_dia.clip = audioc[index];
            //cm.AS_dia.Play();
            //print("audio launch");
        }
    }

    public void StartDiaEffect(string[] dia, AudioClip[] ac)
    {
        if (!isRuntime)
        {
            sentences = dia;
            audioc = ac;
            isRuntime = true;
            index = 0;
            StartCoroutine(Type());
        }
        else
        {
            sentencesStock = dia;
            acStock = ac;
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
        if (acStock != null && acStock.Length > 0)
        {
            audioc = acStock;
            acStock = null;
        }
        if (sentencesStock != null && sentencesStock.Length > 0 && sentences == null)
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
        NombreDiversion();
    }
}
