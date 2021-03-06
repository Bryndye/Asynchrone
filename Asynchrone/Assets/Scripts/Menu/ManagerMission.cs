﻿using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Resources;

public class ManagerMission : MonoBehaviour
{
    [SerializeField] Menu menu;
    [SerializeField] GameObject Bouton;
    public MissionDetails[] missions;
    [Space]
    [Header("Place")]
    [SerializeField] Image illustration;
    [SerializeField] Text context;
    [SerializeField] Transform list_bt;

    private void Awake()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            var bt = Instantiate(Bouton);
            bt.transform.SetParent(list_bt);
            bt.transform.localScale = new Vector3(1,1,1);

            bt.GetComponent<Bt_On>().InitBt(i, this);

            Button b = bt.GetComponentInChildren<Button>();
            int index = missions[i].indexLevel;
            b.onClick.AddListener(delegate { LoadScene(index); });
            
            Text t = bt.GetComponentInChildren<Text>();
            t.text = "MISSION " + (i+1) + " : " + missions[i].Name;
        }
        PlayerPrefs.SetInt("indexLevel", 0);
        PlayerPrefs.SetString("nameLevel", "");
    }
    public void LoadScene(int i) 
    {
        PlayerPrefs.SetInt("indexLevel", i);
        //PlayerPrefs.SetString("nameLevel", nameOfNextlevel);
        SceneManager.LoadScene("5.LoadLevel");
    }


    public void LoadDetails(int index)
    {
        if (illustration != null)
            illustration.sprite = missions[index].Illustration;
        if (context != null)
            context.text = missions[index].Context;
    }

    public void StartNewGame()
    {
        PlayerPrefs.SetInt("indexLevel", 1);
        SceneManager.LoadScene("5.LoadLevel");
    }
}

[System.Serializable]
public class MissionDetails
{
    public string Name;
    public int indexLevel;

    public Sprite Illustration;
    [TextArea(5, 30)] public string Context;
}
