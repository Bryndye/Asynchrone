using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    bool activated;

    public Transform[] Influence;

    public void CallEvent()
    {
        if (!activated)
        {
            //Debug.Log("Event called");
            activated = true;

            for (int i = 0; i < Influence.Length; i++)
            {
                if (Influence[i] != null)
                {
                    Influence[i].gameObject.SetActive(!Influence[i].gameObject.activeSelf);
                }
            }
        }
    }
}
