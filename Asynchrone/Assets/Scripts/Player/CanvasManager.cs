using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : Singleton<CanvasManager>
{
    //public static CanvasManager Instance;

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
    private AudioSource audioSource;
    [HideInInspector] public AudioClip[] audioc;
    [HideInInspector] AudioClip[] acStock;
    private bool isRuntime;

    [SerializeField]
    private GameObject zoneDialogue;

    [Header("Bts Spells")]
    public Text QuelPlayer;
    public GameObject UIHuman;
    public GameObject UIRobot;
    [SerializeField] private Button bt_divRbt;






    void Awake()                                        //AWAKE
    {
        if (Instance != this)
            Destroy(this);

        if (ManagerPlayers.Instance != null)
        {
            mp = ManagerPlayers.Instance;
        }
        anim = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();

        anim.SetTrigger("Disappear");
    }

    void Update()                                       //UPDATE
    {
        if (isRuntime && dialogueHere.text == sentences[index] && !skip)
        {
            skip = true;
            Invoke(nameof(NextDialogue), 2f);
        }
        NombreDiversion();
    }


    public void ActiveLoadScreen() => SceneManager.LoadScene("5.LoadLevel");



    #region SetBtn
    public void CallFctSpell(int i)
    {
        switch (i)
        {

            case 0:
                mp.Camera_Manager();
                break;
            case 1:
                if (!mp.onPlayerHuman && !mp.RobotPlayer.robot_div && mp.RobotPlayer.DivStock > 0)
                {
                    mp.RobotPlayer.StartDiv();
                }
                else
                {
                    Destroy(mp.RobotPlayer.robot_div);
                }
                break;

            case 3:
                if (mp.onPlayerHuman)
                {
                    mp.HumanPlayer.CheckMask();
                }
                break;

            default:
                mp.Camera_Manager();
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
        if (mp == null)
        {
            return;
        }
        if (mp.RobotPlayer != null && bt_divRbt != null)
        {
            if (mp.RobotPlayer.DivStock > 0)
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
         zoneDialogue.SetActive(true);
         LaunchAudio();

         foreach (char letter in sentences[index].ToCharArray())
         {
             dialogueHere.text += letter;
             yield return new WaitForSeconds(latence);
         }
     }

     private void LaunchAudio()
     {
         if (audioc != null && index < audioc.Length)
         {
             //cm.AS_dia.clip = audioc[index];
             //cm.AS_dia.Play();
             //print("audio launch");
         }
     }

     public void StartDiaEffect(string[] _dialogues, AudioClip[] _audioClips = null)
     {
         if (!isRuntime)
         {
             sentences = _dialogues;
             audioc = _audioClips;
             isRuntime = true;
             index = 0;
             StartCoroutine(Type());
         }
         else
         {
             sentencesStock = _dialogues;
             acStock = _audioClips;
         }
     }

     public void NextDialogue()
     {
         if (index < sentences.Length - 1 && sentences != null)
         {
             index++;
             dialogueHere.text = null;
             StartCoroutine(Type());
         }
         else
         {
             sentences = null;
             dialogueHere.text = null;
             isRuntime = false;
             zoneDialogue.SetActive(false);
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

     [Space]
     [SerializeField] float latence = 0.1f;

    #endregion




    #region NewDialogue
    [Header("Dialogues")]


    private List<string> dialoguesString;
    private List<AudioClip> dialoguesAudioClips;

    public void GetSentences(string[] _dialogues, AudioClip[] _audioClips = null)
    {
        for (int i = 0; i < _dialogues.Length; i++)
        {
            dialoguesString.Add(_dialogues[i]);
        }
        for (int i = 0; i < _audioClips.Length; i++)
        {
            dialoguesAudioClips.Add(_audioClips[i]);
        }
    }

    private void NewLaunchAudio()
    {
        //Une fois qu'on chope les audios, on lance
        //Quand l'AudioSource ne joue plus alors on supprime i = 0 des Lists
        //on fait jouer new i = 0
        //et on répète jusqu'à qu'il n'y a plus d'audios
    }

    private void NewLaunchDialogue()
    {

    }

    private void NewNextDialogue()
    {

    }
    #endregion

}
