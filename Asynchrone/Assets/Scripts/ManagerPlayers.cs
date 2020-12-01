using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerPlayers : Singleton<ManagerPlayers>
{
    [Header("Player1")]
    public Transform Player1;
    [HideInInspector] public Human Hm;
    PlayerController pc1;

    [Header("Player 2")]
    public Transform Player2;
    [HideInInspector] public Robot Rbt;
    PlayerController pc2;

    CameraSmooth cSmooth;

    public bool onPlayer1;

    [Header("UI")]
    [SerializeField] Text QuelPlayer;
    [SerializeField] GameObject UIHuman;
    [SerializeField] GameObject UIRobot;


    private void Awake()
    {
        if (Instance != this)
            Destroy(this);

        cSmooth = CameraSmooth.Instance;
        pc1 = Player1.GetComponent<PlayerController>();
        Hm = Player1.GetComponent<Human>();
        pc2 = Player2.GetComponent<PlayerController>();
        Rbt = Player2.GetComponent<Robot>();
        CameraManager();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CameraManager();
        }
    }

    public void CameraManager()
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

    public void RobotBackToHuman()
    {
        Rbt.BackToHuman = !Rbt.BackToHuman;
        if (!Rbt.BackToHuman)
        {
            Rbt.CancelBack();
        }
    }
}
