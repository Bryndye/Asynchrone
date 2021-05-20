using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string Name;
    public bool Openned;

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
}
