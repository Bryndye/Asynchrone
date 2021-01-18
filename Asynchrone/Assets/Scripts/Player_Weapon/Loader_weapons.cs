using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader_weapons : MonoBehaviour
{
    public enum TypeShooting
    {
        Automatic,
        Rafale,
        SemiAuto
    }

    [HideInInspector] public GameObject ModelWeaponC;
    //public ParticleSystem Particle_muzzleC;
    //public Animator Anim_WC;
    [HideInInspector] public Sprite RetC;
    [HideInInspector] public Sprite MunUI_C;
    [HideInInspector] public AudioClip FireSound_C;
    [HideInInspector] public bool CanAimC;
    [HideInInspector] public int FovInAimC;
    [Space]
    [HideInInspector] public TypeShooting TypeC;
    [Space]

    [HideInInspector] public string NameC;
    [HideInInspector] public int DmgC;
    [HideInInspector] public int MunC;
    [HideInInspector] public int MunStock;
    [HideInInspector] public float FireRateC;
    [HideInInspector] public float ImprecisionC;
    [HideInInspector] public float TimeToReloadC;

    [HideInInspector] public GameObject BulletPhysC;
    [HideInInspector] public int NumberOfBulletsC;

    [Space]
    [Space]

    [HideInInspector] public GameObject modelWeapon1;
    //public ParticleSystem Particle_muzzle1;
    //public Animator Anim_W1;
    Sprite ret1;
    Sprite munUI_1;
    AudioClip fireSound_1;
    bool canAim1;
    int fovInAim1;

    TypeShooting Type1;

    [HideInInspector] public string Name1;
    int dmg1;
    int mun1;
    int munStock1;
    float fireRate1;
    float imprecision1;
    float timeToReload1;
    GameObject bulletPhys1;
    int numberOfBullets1;


    [HideInInspector] public GameObject ModelWeapon2;
    //public ParticleSystem Particle_muzzle2;
    //public Animator Anim_W2;
    TypeShooting Type2;
    Sprite ret2;
    Sprite munUI_2;
    AudioClip fireSound_2;
    bool canAim2;
    int fovInAim2;

    [HideInInspector] public string Name2;
    int dmg2;
    int mun2;
    int munStock2;
    float fireRate2;
    float imprecision2;
    float timeToReload2;
    GameObject bulletPhys2;
    int numberOfBullets2;

    public void LoadW1(int i)
    {
        W_Scriptable_s Ws = (W_Scriptable_s)Resources.Load("WScriptable/Arme"+ i);
        modelWeapon1 = Ws.ModelWeapon;
        //Particle_muzzle1 = Ws.Particle_muzzle;
        //Anim_W1 = Ws.Anim_W;
        ret1 = Ws.Ret;
        munUI_1 = Ws.Mun_UI;
        fireSound_1 = Ws.Fire_sound;

        canAim1 = Ws.CanAim;
        fovInAim1 = Ws.FovInAim;

        switch (Ws.Type)
        {
            case W_Scriptable_s.TypeShooting.Automatic:
                Type1 = TypeShooting.Automatic;
                break;
            case W_Scriptable_s.TypeShooting.Rafale:
                Type1 = TypeShooting.Rafale;

                break;
            case W_Scriptable_s.TypeShooting.SemiAuto:
                Type1 = TypeShooting.SemiAuto;

                break;
            default:
                break;
        }
        Name1 = Ws.Name;
        dmg1= Ws.Dmg;
        mun1 = Ws.Mun;
        munStock1 = Ws.MunStock;
        fireRate1 = Ws.FireRate;
        imprecision1 = Ws.Imprecision;
        timeToReload1 = Ws.TimeToReload;

        bulletPhys1 = Ws.BulletPhys;
        numberOfBullets1 = Ws.NumberOfBullets;
    }

    public void LoadW2(int i)
    {
        W_Scriptable_s Ws = (W_Scriptable_s)Resources.Load("WScriptable/Arme" + i);
        ModelWeapon2 = Ws.ModelWeapon;
        //Particle_muzzle2 = Ws.Particle_muzzle;
        //Anim_W2 = Ws.Anim_W;
        ret2 = Ws.Ret;
        fireSound_2 = Ws.Fire_sound;

        canAim2 = Ws.CanAim;
        fovInAim2 = Ws.FovInAim;

        munUI_2 = Ws.Mun_UI;
        switch (Ws.Type)
        {
            case W_Scriptable_s.TypeShooting.Automatic:
                Type2 = TypeShooting.Automatic;
                break;
            case W_Scriptable_s.TypeShooting.Rafale:
                Type2 = TypeShooting.Rafale;

                break;
            case W_Scriptable_s.TypeShooting.SemiAuto:
                Type2 = TypeShooting.SemiAuto;

                break;
            default:
                break;
        }
        Name2 = Ws.Name;
        dmg2 = Ws.Dmg;
        mun2 = Ws.Mun;
        munStock2 = Ws.MunStock;
        fireRate2 = Ws.FireRate;
        imprecision2 = Ws.Imprecision;
        timeToReload2 = Ws.TimeToReload;

        bulletPhys2 = Ws.BulletPhys;
        numberOfBullets2 = Ws.NumberOfBullets;
    }

    public void ChangeCurrentW(bool F)
    {
        if (F)
        {
            ModelWeaponC = modelWeapon1;
            //Particle_muzzleC = Particle_muzzle1;
            //Anim_WC = Anim_W1;
            FireSound_C = fireSound_1;

            CanAimC = canAim1;
            FovInAimC = fovInAim1;

            RetC = ret1;
            MunUI_C = munUI_1;
            TypeC = Type1;
            NameC = Name1;
            DmgC = dmg1;
            MunC = mun1;
            FireRateC = fireRate1;
            ImprecisionC = imprecision1;
            TimeToReloadC = timeToReload1;

            BulletPhysC = bulletPhys1;
            NumberOfBulletsC = numberOfBullets1;
        }
        else
        {
            ModelWeaponC = ModelWeapon2;
            //Particle_muzzle2 = Ws.Particle_muzzle;
            //Anim_WC = Anim_W2;
            FireSound_C = fireSound_2;

            CanAimC = canAim2;
            FovInAimC = fovInAim2;

            RetC = ret2;
            MunUI_C = munUI_2;
            TypeC = Type2;
            NameC = Name2;
            DmgC = dmg2;
            MunC = mun2;
            FireRateC = fireRate2;
            ImprecisionC = imprecision2;
            TimeToReloadC = timeToReload2;

            BulletPhysC = bulletPhys2;
            NumberOfBulletsC = numberOfBullets2;
        }

        //Player_shoot ps = GetComponent<Player_shoot>();
        //ps.Init_MunUI();
    }
}
