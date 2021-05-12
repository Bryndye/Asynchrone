using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelManager : MonoBehaviour
{
    public int indexOfNextlevel;
    public string nameOfNextlevel;

    [SerializeField]
    private GameObject textLoading, textEspace;


    void Awake()
    {
        indexOfNextlevel = PlayerPrefs.GetInt("indexLevel");
        nameOfNextlevel = PlayerPrefs.GetString("nameLevel");
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = !string.IsNullOrEmpty(nameOfNextlevel) && SceneManager.GetSceneByName(nameOfNextlevel).IsValid() ?
            SceneManager.LoadSceneAsync(nameOfNextlevel) : SceneManager.LoadSceneAsync(indexOfNextlevel);

        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;

        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            //Debug.Log("Pro :" + asyncOperation.progress);

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                textLoading.SetActive(false);
                textEspace.SetActive(true);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //Wait to you press the space key to activate the Scene
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
