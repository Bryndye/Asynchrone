using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Inventory : MonoBehaviour
{
    public Transform Arms_spawn;

    [HideInInspector] public bool FirstWeapon;

    [HideInInspector] public GameObject Weapon_1;
    [HideInInspector] public GameObject Weapon_2;

    public int W_Index1;
    public int W_Index2;

    Loader_weapons lw;
    //Player_Health ph;
    //Player_shoot ps;

    [Header("UI for inv")]
    public Image Ret_Image;
    public Text NameWeapon_text;
    public Text NameWeaponBack_text;

    private void Awake()
    {
        lw = GetComponent<Loader_weapons>();
        //ph = GetComponent<Player_Health>();
        //ps = GetComponent<Player_shoot>();

        lw.LoadW1(W_Index1);
        lw.LoadW2(W_Index2);

        InitNewWeapon1();
        InitNewWeapon2();

        FirstWeapon = true;
        SwitchWeapon();
    }

    #region Init Weapons
    void InitNewWeapon1()
    {
        if (lw.modelWeapon1 != null && Arms_spawn != null)
        {
            Weapon_1 = Instantiate(lw.modelWeapon1, Arms_spawn.position, Arms_spawn.rotation);
            Weapon_1.transform.SetParent(Arms_spawn);
        }
    }

    void InitNewWeapon2()
    {
        if (lw.ModelWeapon2 != null && Arms_spawn != null)
        {
            Weapon_2 = Instantiate(lw.ModelWeapon2, Arms_spawn.position, Arms_spawn.rotation);
            Weapon_2.transform.SetParent(Arms_spawn);
        }
    }
    #endregion

    void Update()
    {
        if (/*!ph.dead*/ true)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                FirstWeapon = true;
                SwitchWeapon();
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                FirstWeapon = false;
                SwitchWeapon();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                FirstWeapon = !FirstWeapon;
                SwitchWeapon();
            }
            UI_Enable();
        }
        //Debug.Log(Weapon_1 + " w1");
    }

    void SwitchWeapon()
    {
        //ps.reloading = false;
        //ps.CancelInvoke();
        lw.ChangeCurrentW(FirstWeapon);

        Weapon_1.SetActive(FirstWeapon);
        Weapon_2.SetActive(!FirstWeapon);
    }

    void UI_Enable()
    {
        if (NameWeapon_text != null)
            NameWeapon_text.text = lw.NameC;

        if (FirstWeapon && NameWeaponBack_text != null)
            NameWeaponBack_text.text = lw.Name2;

        if(!FirstWeapon && NameWeaponBack_text != null)
            NameWeaponBack_text.text = lw.Name1;      

        if (lw.RetC != null && Ret_Image != null)     
            Ret_Image.sprite = lw.RetC;
        else if(Ret_Image != null)
            Ret_Image.sprite = Resources.Load<W_Scriptable_s>("WScriptable/Arme0").Ret;
    }
}
