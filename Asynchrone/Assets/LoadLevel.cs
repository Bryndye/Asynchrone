using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    ManagerPlayers mp
    {
        get
        {
            return ManagerPlayers.Instance;
        }
    }
    CanvasManager cm
    {
        get
        {
            return CanvasManager.Instance;
        }
    }
    CameraManager camM
    {
        get
        {
            return CameraManager.Instance;
        }
    }

    [SerializeField] private int indexOfNextlevel;
    [SerializeField] private string nameOfNextlevel;
    [SerializeField] private List<GameObject> players;

    bool done = false;

    private void OnTriggerStay(Collider other)
    {
        if (done)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            if (!players.Contains(other.gameObject))
            {
                players.Add(other.gameObject);
            }
        }

        if (players.Count >= 2 || mp.Player2 == null && players.Count >= 1)
        {
            mp.pc1.InCinematic = true;
            mp.pc2.InCinematic = true;
            done = true;
            canvasLoading.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (players.Contains(other.gameObject))
            {
                players.Remove(other.gameObject);
            }
        }
    }


    [SerializeField]
    private GameObject canvasLoading, textLoading, textEspace, loadingScreen;

    public void ActiveLoadScreen()
    {
        loadingScreen.SetActive(true);
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("fondu");

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

    /*if load
     *             if (!string.IsNullOrEmpty(nameOfNextlevel) && SceneManager.GetSceneByName(nameOfNextlevel).IsValid())
            {         
                SceneManager.LoadScene(nameOfNextlevel);
            }
            else
            {
                SceneManager.LoadScene(indexOfNextlevel);
            }
    */

}
