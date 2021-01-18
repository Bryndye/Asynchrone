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
    [SerializeField] Image illustration;
    [SerializeField] Text context;


    private void Awake()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            var bt = Instantiate(Bouton);
            bt.transform.SetParent(transform);
            bt.transform.localScale = new Vector3(1,1,1);

            Bt_On bo = bt.GetComponent<Bt_On>();
            menu.btAnim.Add(bo);
            bo.InitBt(i, this);

            Button b = bt.GetComponentInChildren<Button>();
            string lvl = missions[i].LevelName;
            b.onClick.AddListener(delegate { LoadScene(lvl); });
            
            Text t = bt.GetComponentInChildren<Text>();
            t.text = "MISSION " + i + " : " + missions[i].Name;
        }
    }
    public void LoadScene(string kk) => SceneManager.LoadScene(kk, LoadSceneMode.Single);

    public void LoadDetails(int index)
    {
        if (illustration != null)
            illustration.sprite = missions[index].Illustration;
        if (context != null)
            context.text = missions[index].Context;
    }

}

[System.Serializable]
public class MissionDetails
{
    public string Name;
    public string LevelName;

    public Sprite Illustration;
    public string Context;
}
