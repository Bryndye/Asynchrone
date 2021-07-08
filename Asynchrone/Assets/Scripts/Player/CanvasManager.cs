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

    //[HideInInspector]
    public Animator anim;

    [SerializeField] 
    private GameObject checkpoint_t;

    public GameObject mortZone;

    [Header("Char anim")]
    private bool skip;
    private int index =0;
    [HideInInspector] 
    public string[] Dialogues;
    string[] dialoguesStock;

    private Sprite[] portraits;
    private Sprite[] portraitsStock;
    private string[] noms;
    private string[] nomsStock;
    //private AudioSource audioSource;
    //[HideInInspector] 
    //public AudioClip[] audioc;
    //[HideInInspector] 
    //AudioClip[] acStock;
    private bool isRuntime;

    [Header("Dialogues")]
    [SerializeField]
    private GameObject zoneDialogue;
    public Text dialogueText;
    [SerializeField]
    private Image portraitSprite;
    [SerializeField]
    private Text nomText;
    [SerializeField] private Image PortraitFond, CadreFond;


    #region UI Spell
    [Header("Bts Spells")]
    public Image ProfilPlayer;
    public GameObject UIHuman;
    public GameObject UIRobot;
    [SerializeField]
    private Button btDiversion, btSwichCam, btCrouch;

    [SerializeField]
    private Text textDiversion, textCrouch, textSwitchCamera;
    [Space]

    [SerializeField]
    private Sprite spriteStand, spriteCrouch, spriteDiv, spriteDivSuprr;
    #endregion


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
        //audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (managerPlayers != null)
        {
            btSwichCam.enabled = managerPlayers.PlayerCntrlerRbt;
        }
    }

    void Update()                                       //UPDATE
    {
        if (isRuntime && dialogueText.text == Dialogues[index] && !skip)
        {
            skip = true;
            Invoke(nameof(NextDialogue), 2f);
        }
        NombreDiversion();
        SetInputText();
    }

    #region LoadLevel
    public void ActiveLoadScreen() => StartCoroutine(LoadScene());

    IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("5.LoadLevel");

        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;

        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            //Debug.Log("Pro :" + asyncOperation.progress);

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                //Wait to you press the space key to activate the Scene
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    #endregion


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
            ChangeSpriteDiv(false);
            Destroy(managerPlayers.RobotPlayer.RobotDiv);
        }
    }

    public void ChangeSpriteCrouch(bool crouch)
    {
        if (crouch)
        {
            btCrouch.GetComponent<Image>().sprite = spriteCrouch;
        }
        else
        {
            btCrouch.GetComponent<Image>().sprite = spriteStand;
        }
    }

    public void ChangeSpriteDiv(bool dived)
    {
        if (dived)
        {
            btDiversion.GetComponent<Image>().sprite = spriteDivSuprr;
        }
        else
        {
            btDiversion.GetComponent<Image>().sprite = spriteDiv;
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
                //div destructible
            }
            else
            {
                btDiversion.interactable = managerPlayers.RobotPlayer.HasDiversion;
                //has div and dont
            }
        }
    }

    private void SetInputText()
    {
        if (textDiversion == null)
        {
            return;
        }
        textDiversion.text = managerPlayers.InputDiversion.ToString();
        textCrouch.text = managerPlayers.InputCrouch.ToString();
        textSwitchCamera.text = managerPlayers.InputSwitchCamera.ToString();
    }
    #endregion



    #region Daliogue


    IEnumerator Type()
    {
        zoneDialogue.SetActive(true);
        portraitSprite.sprite = portraits[index];
        nomText.text = noms[index];
        LaunchAudio();

        UpdateDialogueColor(noms[index]);

        foreach (char letter in Dialogues[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(latence);
        }
    }

     private void LaunchAudio()
     {
         //if (audioc != null && index < audioc.Length)
         //{
         //    //cm.AS_dia.clip = audioc[index];
         //    //cm.AS_dia.Play();
         //    //print("audio launch");
         //}
     }

     public void StartDiaEffect(string[] _dialogues, Sprite[] _portraits, string[] _noms)
     {
         if (!isRuntime)
         {
            Dialogues = _dialogues;
            portraits = _portraits;
            noms = _noms;
            isRuntime = true;
            index = 0;
            StartCoroutine(Type());
         }
         else
         {
            dialoguesStock = _dialogues;
            portraitsStock = _portraits;
            nomsStock = _noms;
         }
     }

     public void NextDialogue()
     {
         if (index < Dialogues.Length - 1 && Dialogues != null)
         {
             index++;
             dialogueText.text = null;
             StartCoroutine(Type());
         }
         else
         {
             Dialogues = null;
             dialogueText.text = null;
             isRuntime = false;
             zoneDialogue.SetActive(false);
         }
         //if (acStock != null && acStock.Length > 0)
         //{
         //    audioc = acStock;
         //    acStock = null;
         //}
         if (dialoguesStock != null && dialoguesStock.Length > 0 && Dialogues == null)
         {
             isRuntime = true;
             index = 0;
             Dialogues = dialoguesStock;
             dialoguesStock = null;

            portraits = portraitsStock;
            portraitsStock = null;

            noms = nomsStock;
            nomsStock = null;

            dialogueText.text = null;
             StartCoroutine(Type());
             //Debug.Log("next dialogues");
         }
         skip = false;
     }

    void UpdateDialogueColor(string CharacterName)
    {
        Color ToUpdateWith = GetCharacterColor(CharacterName);
        CadreFond.color = ToUpdateWith;
        PortraitFond.color = ToUpdateWith;
        nomText.color = ToUpdateWith;
    }

    Color GetCharacterColor(string CharacterName)
    {
        if (CharacterName.Contains("Jumes"))
            return new Color(249f, 242f, 0);
        else if (CharacterName.Contains("V4trek"))
            return new Color(0f, 136f, 169f);
        else
            return new Color(206f, 0f, 0f);
    }

     [Space]
     [SerializeField] float latence = 0.1f;

    #endregion

}
