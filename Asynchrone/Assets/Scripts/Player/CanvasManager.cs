using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : Singleton<CanvasManager>
{
    CameraManager cameraManager;
    ManagerPlayers managerPlayers;
    PlayerController playerController;

    [HideInInspector] 
    public Animator anim;
    [SerializeField] 
    private GameObject checkpoint_t;

    [Header("Char anim")]
    public Text dialogueHere;
    private bool skip;
    private int index =0;
    [HideInInspector] 
    public string[] sentences;
    [HideInInspector] 
    string[] sentencesStock;
    private AudioSource audioSource;
    [HideInInspector] 
    public AudioClip[] audioc;
    [HideInInspector] 
    AudioClip[] acStock;
    private bool isRuntime;

    [SerializeField]
    private GameObject zoneDialogue;

    [Header("Bts Spells")]
    public Image ProfilPlayer;
    public GameObject UIHuman;
    public GameObject UIRobot;
    [SerializeField] 
    private Button btDiversion;

    [SerializeField]
    private Text textDiversion, textCrouch, textSwitchCamera;

    #region UIInteraction
    [Header("Interaction UI")]
    public Transform accrocheInteraction;
    public Transform accrocheAIstate;
    #endregion


    void Awake()                                        //AWAKE
    {
        if (Instance != this)
            Destroy(this);

        if (ManagerPlayers.Instance != null)
        {
            managerPlayers = ManagerPlayers.Instance;
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
        SetInputText();
    }


    public void ActiveLoadScreen() => SceneManager.LoadScene("5.LoadLevel");



    #region ButtonSpell
    public void SwitchCamera() => managerPlayers.Camera_Manager();

    public void Crouch()
    {
        if (managerPlayers.onPlayerHuman)
        {
            managerPlayers.HumanPlayer.CheckMask();
        }
    }

    public void Diversion()
    {
        if (!managerPlayers.onPlayerHuman && !managerPlayers.RobotPlayer.RobotDiv && managerPlayers.RobotPlayer.HasDiversion)
        {
            managerPlayers.RobotPlayer.StartDiv();
        }
        else
        {
            Destroy(managerPlayers.RobotPlayer.RobotDiv);
        }
    }
    #endregion



    #region visual
    public void BandeAppear() => anim.SetTrigger("Appear");

    public void BandeDisAppear() => anim.SetTrigger("Disappear");

    private void DesactiveI() => checkpoint_t.SetActive(false);

    public void ActiveCheckpointText()
    {
        checkpoint_t.SetActive(true);
        Invoke(nameof(DesactiveI),2f);
    }


    private void NombreDiversion()
    {
        if (managerPlayers == null)
        {
            return;
        }
        else if (managerPlayers.RobotPlayer != null && btDiversion != null)
        {
            if (managerPlayers.RobotPlayer.RobotDiv != null)
            {
                btDiversion.interactable = true;
            }
            else
            {
                btDiversion.interactable = managerPlayers.RobotPlayer.HasDiversion;
            }
        }
    }

    private void SetInputText()
    {
        textDiversion.text = managerPlayers.InputDiversion.ToString();
        textCrouch.text = managerPlayers.InputCrouch.ToString();
        textSwitchCamera.text = managerPlayers.InputSwitchCamera.ToString();
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
