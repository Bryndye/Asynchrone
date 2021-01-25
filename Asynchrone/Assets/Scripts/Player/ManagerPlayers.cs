using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerPlayers : Singleton<ManagerPlayers>
{
    [Header("Human")]
    public Transform Player1;
    [HideInInspector] public Human Hm;
    [HideInInspector] public PlayerController pc1;

    [Header("Robot")]
    public Transform Player2;
    [HideInInspector] public Robot Rbt;
    [HideInInspector] public PlayerController pc2;

    CameraManager cSmooth;
    CanvasManager cm;
    public bool onPlayer1;

    [Header("UI")]
    [SerializeField] Text QuelPlayer;
    [SerializeField] GameObject UIHuman;
    [SerializeField] GameObject UIRobot;
    [Space]
    [SerializeField] Texture2D cursorTexture;
    CursorMode cursorMode = CursorMode.Auto;
    Vector2 hotSpot = Vector2.zero;

    private void Awake()
    {
        if (Instance != this)
            Destroy(this);

        cSmooth = CameraManager.Instance;
        if (Player1 != null)
            pc1 = Player1.GetComponent<PlayerController>();

        if (Player2 != null)
            pc2 = Player2.GetComponent<PlayerController>();
        Camera_Manager();
        cm = CanvasManager.Instance;
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
                if (QuelPlayer != null)
                    QuelPlayer.text = Player1.name;

                cSmooth.Target = Player1;
            }
            else
            {
                if (QuelPlayer != null)
                    QuelPlayer.text = Player2.name;

                cSmooth.Target = Player2;
            }

            pc1.CanPlay = onPlayer1;
            pc2.CanPlay = !onPlayer1;

            if (UIHuman != null && UIRobot != null)
            {
                UIHuman.SetActive(onPlayer1);
                UIRobot.SetActive(!onPlayer1);
            }

            if (!onPlayer1 && Hm.intoMe)
            {
                Hm.RobotIntoMe(false);
            }
        }
        else
        {
            onPlayer1 = true;
            if (QuelPlayer != null)
                QuelPlayer.text = Player1.name;

            cSmooth.Target = Player1;
            pc1.CanPlay = onPlayer1;
        }

    }

    #region Huamn fct

    public void RobotBackToHuman()
    {
        if (Rbt)
        {
            Rbt.BackToHuman = !Rbt.BackToHuman;
            if (!Rbt.BackToHuman)
            {
                Rbt.CancelBack();
            }
        }
    }
    #endregion

    public void CursorStyle()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Interaction"))
            {
                Debug.Log("Interact");
                Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
            }
            else
            {
                Cursor.SetCursor(null, hotSpot, cursorMode);
            }
        }
    }
}
