using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class aVoicePack : ScriptableObject
{
    public List<AudioClip> Seen;
    public List<AudioClip> StartPursuit;
    public List<AudioClip> EndPursuit;
    public List<AudioClip> BackToNormal;
}
