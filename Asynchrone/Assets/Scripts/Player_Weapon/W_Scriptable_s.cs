using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="ScriptableObject Weapon", menuName ="New W" ,order = 0)]
public class W_Scriptable_s : ScriptableObject
{
    public string Name;
    public enum TypeShooting
    {
        Automatic,
        Rafale,
        SemiAuto
    }
    public TypeShooting Type;


    [Header("Bullets")]
    [Tooltip("Si c'est vide, l'arme tire en Raycast")] public GameObject BulletPhys;
    [Tooltip("Le nombre de balles tirées par mun")] [Range(0, 20)] public int NumberOfBullets;
    [Range(0, 200)] public int Dmg;
    [Range(0, 200)] public int Mun;
    [Range(0, 600)] public int MunStock;
    public float FireRate;
    [Tooltip("En degree")][Range(0,20)] public float Imprecision;
    [Range(0, 10)] public float TimeToReload;


    [Header("Aim")]
    public bool CanAim;
    [Range(30, 100)] public int FovInAim;


    [Header("UI")]
    [Tooltip("Son sprite de réticule")] public Sprite Ret;
    [Tooltip("Son sprite de Mun en haut à droite")] public Sprite Mun_UI;


    [Header("Visu3D/fx")]
    [Tooltip("3D de l'arme")] public GameObject ModelWeapon;
    [Tooltip("Le feu de l'arme quand on tire")] public ParticleSystem Particle_muzzle;
    [Tooltip("L'animator de l'arme")] public Animator Anim_W;


    [Header("Sound")]
    [Tooltip("Le Son quand il tire")] public AudioClip Fire_sound;

}
