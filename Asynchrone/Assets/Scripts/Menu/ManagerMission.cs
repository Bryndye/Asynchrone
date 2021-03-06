using System.Collections;
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
    [TextArea(5, 30)] public string Context;
}
