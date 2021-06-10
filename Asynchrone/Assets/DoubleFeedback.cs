using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleFeedback : MonoBehaviour
{
    [SerializeField]
    private GameObject lightHuman, lightRobot;

    private void Start()
    {
        lightHuman.SetActive(false);
        lightRobot.SetActive(false);
    }

    public void ActiveHuman()
    {
        lightHuman.SetActive(true);
    }

    public void ActiveRobot()
    {
        lightRobot.SetActive(true);
    }
}
