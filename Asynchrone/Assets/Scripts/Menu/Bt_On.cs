using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Bt_On : MonoBehaviour
{
    Animator MineAnim;
    Button bt;

    int index;
    ManagerMission Mm;

    public void InitBt(int i, ManagerMission mm)
    {
        index = i;
        Mm = mm;
    }

    void Awake()
    {
        MineAnim = GetComponent<Animator>();
        bt = GetComponentInChildren<Button>();
    }

    public void AnimPointer()
    {
        MineAnim.Play("btOnThis");
        if (Mm != null)
            Mm.LoadDetails(index);
    }

    public void AnimPointerExit() => MineAnim.Play("btExit");
}
