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



    private void Awake()
    {
        Time.timeScale = 1;
        if (Instance != this)
            Destroy(this);

        cSmooth = CameraManager.Instance;
        cm = CanvasManager.Instance;

        if (Player1 != null)
            pc1 = Player1.GetComponent<PlayerController>();

        if (Player2 != null)
            pc2 = Player2.GetComponent<PlayerController>();
        Camera_Manager();
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
                if (cm.QuelPlayer != null)
                    cm.QuelPlayer.text = Player1.name;

                cSmooth.Target = Player1;
            }
            else
            {
                if (!cm.QuelPlayer)
                    cm.QuelPlayer.text = Player2.name;

                cSmooth.Target = Player2;
            }

            pc1.CanPlay = onPlayer1;
            pc2.CanPlay = !onPlayer1;

            if (cm.UIHuman != null && cm.UIRobot != null)
            {
                cm.UIHuman.SetActive(onPlayer1);
                cm.UIRobot.SetActive(!onPlayer1);
            }
        }
        else
        {
            onPlayer1 = true;
            if (cm.QuelPlayer != null)
                cm.QuelPlayer.text = Player1.name;

            cSmooth.Target = Player1;
            pc1.CanPlay = onPlayer1;

            if (cm.UIHuman != null && cm.UIRobot != null)
            {
                cm.UIHuman.SetActive(true);
                cm.UIRobot.SetActive(false);
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
