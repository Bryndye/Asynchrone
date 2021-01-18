using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInGame : Singleton<MenuInGame>
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        Scene sc = SceneManager.GetActiveScene();
        if (Input.GetKeyDown(KeyCode.Escape) && sc.buildIndex != 0)
        {
            SceneManager.LoadScene(0);
            //Debug.Log("Back to menu");
        }
    }
}
