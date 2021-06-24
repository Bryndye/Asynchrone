using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Bt_On : MonoBehaviour
{
    Animator MineAnim;
    Button bt;

    SoundManager SM;

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

        SM = SoundManager.Instance;
    }

    public void AnimPointer()
    {
        MineAnim.Play("btOnThis");
        if (Mm != null)
            Mm.LoadDetails(index);

        SM.GetASound("Bouton_Over", null, true);
    }

    public void AnimPointerExit() => MineAnim.Play("btExit");
}
