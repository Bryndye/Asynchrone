using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerPlayers : Singleton<ManagerPlayers>
{
    [Header("Managers")]
    [SerializeField] Transform Player1;
    [SerializeField] Transform Player2;

    CameraSmooth cSmooth;

    [SerializeField] bool onPlayer1;

    [Header("UI")]
    [SerializeField] Text QuelPlayer;


    private void Awake()
    {
        if (Instance != this)
            Destroy(this);

        cSmooth = CameraSmooth.Instance;
        CameraManager();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CameraManager();
        }
    }

    private void CameraManager()
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

        Player1.GetComponent<PlayerController>().CanPlay = onPlayer1;
        Player2.GetComponent<PlayerController>().CanPlay = !onPlayer1;
    }

    public void RobotBackToHuman()
    {
        Player2.GetComponent<PlayerController>().nav.SetDestination(Player1.position);
    }
}
