using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VoiceFor { Seen, StartPursuit, EndPursuit, BackToNormal }

public class IAManager : Singleton<IAManager>
{
    [HideInInspector] public List<anAI> LivingAI;
    public anAI ActualSpeakingIA;

    public aVoicePack RobotVoice;
    public aVoicePack DroneVoice;

    private void Awake()
    {
        if (Instance != this)
            Destroy(this);

        //RobotVoice = Resources.Load<aVoicePack>("AI/VoicePacks/Robot");
        //DroneVoice = Resources.Load<aVoicePack>("AI/VoicePacks/Drone");
    }

    public void RemoveIA(anAI myAI)
    {
        if (LivingAI.Contains(myAI))
            LivingAI.Remove(myAI);
    }

    public bool isSomeoneSpeaking()
    {
        bool toReturn = false;
        if (ActualSpeakingIA && ActualSpeakingIA.myVoice.isPlaying)
            toReturn = true;
        else
            toReturn = false;

        return toReturn;
    }
}
