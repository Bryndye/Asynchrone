using UnityEngine;

public class ManagerPlayers : Singleton<ManagerPlayers>
{
    //public static ManagerPlayers Instance;

    [Header("Human")]
    public Transform PlayerHumanTransform;
    [HideInInspector] public Human HumanPlayer;
    [HideInInspector] public PlayerController PlayerControllerHm;

    [Header("Robot")]
    public Transform PlayerRobotTransform;
    [HideInInspector] public Robot RobotPlayer;
    [HideInInspector] public PlayerController PlayerCntrlerRbt;

    CameraManager cameraManager;
    CanvasManager canvasManager;
    public bool onPlayerHuman;


    public KeyCode InputSwitchCamera;


    private void Awake()
    {
        if (!enabled)
        {
            return;
        }
        Time.timeScale = 1;

        if (Instance != this)
            Destroy(this);
        
        if (CameraManager.Instance != null)
        {
            cameraManager = CameraManager.Instance;
        }
        if (CanvasManager.Instance != null)
        {
            canvasManager = CanvasManager.Instance;
        }


        if (PlayerHumanTransform != null)
            PlayerControllerHm = PlayerHumanTransform.GetComponent<PlayerController>();

        if (PlayerRobotTransform != null)
            PlayerCntrlerRbt = PlayerRobotTransform.GetComponent<PlayerController>();

        if (PlayerHumanTransform != null && PlayerRobotTransform != null)
        {
            Camera_Manager();
        }
        else
        {
            cameraManager.Target = PlayerControllerHm.transform;
            PlayerControllerHm.CanPlay = true;
            onPlayerHuman = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(InputSwitchCamera))
        {
            Camera_Manager();
        }
        CursorStyle();
    }

    public void Camera_Manager()
    {
        if (PlayerRobotTransform != null)
        {
            onPlayerHuman = !onPlayerHuman;

            if (onPlayerHuman)
            {
                //if (canvasManager.ProfilPlayer != null)
                //    canvasManager.ProfilPlayer.sprite = null;

                cameraManager.Target = PlayerHumanTransform;
            }
            else
            {
                //if (!canvasManager.ProfilPlayer)
                //    canvasManager.ProfilPlayer.sprite = null;

                cameraManager.Target = PlayerRobotTransform;
            }

            PlayerControllerHm.CanPlay = onPlayerHuman;
            PlayerCntrlerRbt.CanPlay = !onPlayerHuman;

            if (canvasManager.UIHuman != null && canvasManager.UIRobot != null)
            {
                canvasManager.UIHuman.SetActive(onPlayerHuman);
                canvasManager.UIRobot.SetActive(!onPlayerHuman);
            }
        }
        else
        {
            onPlayerHuman = true;
            //if (canvasManager.ProfilPlayer != null)
            //    canvasManager.ProfilPlayer.sprite = null;

            cameraManager.Target = PlayerHumanTransform;
            PlayerControllerHm.CanPlay = onPlayerHuman;

            if (canvasManager.UIHuman != null && canvasManager.UIRobot != null)
            {
                canvasManager.UIHuman.SetActive(true);
                canvasManager.UIRobot.SetActive(false);
            }
        }
    }



    #region Cursor
    [Space]
    CursorMode cursorMode = CursorMode.Auto;
    Vector2 hotSpot = Vector2.zero;
    [SerializeField] LayerMask layerCursor;
    private void SetCursor(string nom)
    {
        if (Resources.Load<Texture2D>("UI/Cursor/" + nom))
        {
            Cursor.SetCursor(Resources.Load<Texture2D>("UI/Cursor/" + nom), hotSpot, cursorMode);
        }
        else
        {
            Cursor.SetCursor(null, hotSpot, cursorMode);
        }
    }

    public void CursorStyle()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerCursor))
        {
            if (hit.collider.TryGetComponent(out anAI ai) && ai.Killable() && onPlayerHuman)
            {
                SetCursor("Attack");
            }
            else if (hit.collider.CompareTag("Interaction"))
            {
                if (hit.collider.TryGetComponent(out Interaction interaction))
                {
                    PlayerController pc = onPlayerHuman ? PlayerControllerHm : PlayerCntrlerRbt;
                    if (interaction.PlayerControlRef == pc)
                    {
                        SetCursor("Interact");
                    }
                }
                else if (hit.collider.GetComponent<trap_interaction>() != null && !onPlayerHuman)
                {
                    SetCursor("Interact");
                }
                else
                {
                    SetCursor(null);
                }
            }
        }
        else
        {
            SetCursor(null);
        }
    }

    #endregion
    

}
