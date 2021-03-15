using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    private bool activated;
    [SerializeField] private bool cinematic = false;
    private CameraManager cm;

    public Transform[] Influence;

    private void Awake() => cm = CameraManager.Instance;

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
                    if (cm != null && cinematic)
                        cm.GetTargetPorte(Influence);
                }
            }
        }
    }
}
