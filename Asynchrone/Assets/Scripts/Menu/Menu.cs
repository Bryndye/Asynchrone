using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string Name;
    public bool Openned;

    public List<Bt_On> btAnim;

    public void Open()
    {
        Openned = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        Openned = false;
        gameObject.SetActive(false);
    }

    /*
    private void OnDisable()
    {
        for (int i = 0; i < btAnim.Length; i++)
        {
            btAnim[i].DisableParent();
        }
    }*/
}
