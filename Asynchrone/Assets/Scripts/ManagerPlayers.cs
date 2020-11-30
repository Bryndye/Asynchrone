using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerPlayers : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] Transform Player1;
    [SerializeField] Transform Player2;

    [SerializeField] CameraSmooth cSmooth;

    [SerializeField] bool onPlayer1;

    [Header("UI")]
    [SerializeField] Text QuelPlayer;


    private void Awake()
    {
        CameraManager();
        Player1.GetComponent<PlayerController>().mP = this;
        Player2.GetComponent<PlayerController>().mP = this;
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
            Player1.GetComponent<PlayerController>().CanPlay = true;
            Player2.GetComponent<PlayerController>().CanPlay = false;
        }
        else
        {
            if (QuelPlayer != null)
                QuelPlayer.text = Player2.name;

            cSmooth.Target = Player2;
            Player1.GetComponent<PlayerController>().CanPlay = false;
            Player2.GetComponent<PlayerController>().CanPlay = true;
        }
    }


}
