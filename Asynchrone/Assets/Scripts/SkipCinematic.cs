using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipCinematic : MonoBehaviour
{
    [SerializeField]
    private GameObject spaceText;

    [SerializeField]
    int indexLevel;

    bool isSpace;
    float time;

    private void Start()
    {
        isSpace = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isSpace)
            {
                PlayerPrefs.SetInt("indexLevel", indexLevel);
                SceneManager.LoadScene(7);

                return;
            }

            spaceText.SetActive(true);
            isSpace = true;
        }

        time += Time.deltaTime;
        if (time > 3 && isSpace)
        {
            isSpace = false;
            spaceText.SetActive(false);
            time = 0;
        }
    }
}
