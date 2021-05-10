using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerPlayers : Singleton<ManagerPlayers>
{
    //public static ManagerPlayers Instance;

    [Header("Human")]
    public Transform Player1;
    [HideInInspector] public Human HumanPlayer;
    [HideInInspector] public PlayerController PlayerCtrlerHm;

    [Header("Robot")]
    public Transform Player2;
    [HideInInspector] public Robot RobotPlayer;
    [HideInInspector] public PlayerController PlayerCntrlerRbt;

    CameraManager cameraManager;
    CanvasManager canvasManager;
    public bool onPlayer1;



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


        if (Player1 != null)
            PlayerCtrlerHm = Player1.GetComponent<PlayerController>();

        if (Player2 != null)
            PlayerCntrlerRbt = Player2.GetComponent<PlayerController>();

        if (Player1 != null && Player2 != null)
        {
            Camera_Manager();
        }
        else
        {
            cameraManager.Target = PlayerCtrlerHm.transform;
            PlayerCtrlerHm.CanPlay = true;
            onPlayer1 = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Camera_Manager();
        }
        CursorStyle();
    }

    public void Camera_Manager()
    {
        if (Player2 != null)
        {
            onPlayer1 = !onPlayer1;

            if (onPlayer1)
            {
                if (canvasManager.QuelPlayer != null)
                    canvasManager.QuelPlayer.text = Player1.name;

                cameraManager.Target = Player1;
            }
            else
            {
                if (!canvasManager.QuelPlayer)
                    canvasManager.QuelPlayer.text = Player2.name;

                cameraManager.Target = Player2;
            }

            PlayerCtrlerHm.CanPlay = onPlayer1;
            PlayerCntrlerRbt.CanPlay = !onPlayer1;

            if (canvasManager.UIHuman != null && canvasManager.UIRobot != null)
            {
                canvasManager.UIHuman.SetActive(onPlayer1);
                canvasManager.UIRobot.SetActive(!onPlayer1);
            }
        }
        else
        {
            onPlayer1 = true;
            if (canvasManager.QuelPlayer != null)
                canvasManager.QuelPlayer.text = Player1.name;

            cameraManager.Target = Player1;
            PlayerCtrlerHm.CanPlay = onPlayer1;

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
            if (hit.collider.GetComponent<anAI>() != null && hit.collider.GetComponent<anAI>().Killable() && onPlayer1)
            {
                SetCursor("Attack");
            }
            else if (hit.collider.CompareTag("Interaction"))
            {
                if (hit.collider.GetComponent<trap_interaction>() != null && !onPlayer1)
                {
                    SetCursor("Interact");
                }
                else
                {
                    SetCursor(null);
                }
                if (hit.collider.GetComponent<trap_interaction>() == null)
                {
                    SetCursor("Interact");
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
