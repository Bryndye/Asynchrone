using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInGame : Singleton<MenuInGame>
{

    [SerializeField]
    private GameObject menuInGame;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ActiveMenu(true);
            Time.timeScale = 0;
        }
    }

    public void BackToPlay()
    {
        Time.timeScale = 1;
        ActiveMenu(false);
    }

    private void ActiveMenu(bool active) => menuInGame.SetActive(active);


    public void BackToMenu()
    {
        Time.timeScale = 1;
        PlayerPrefs.SetInt("indexLevel", 0);
        SceneManager.LoadScene(7);
    }
}
