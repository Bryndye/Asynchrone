using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    //public static CameraManager Instance;

    public Transform Target;
    public Transform[] TargetPorte;

    private int index;
    private float timer;

    [Space]
    [SerializeField]
    private float speed = 40;

    private void Update() => SmoothFollow();                       //UPDATE

    private void Awake()                                           //AWAKE
    {
        if (Instance != this)
            Destroy(this);

        audioS = GetComponent<AudioSource>();
    }

    private void FixedUpdate()                                     //FIXED UPDATE
    {
        AudioPlaying();
    }




    private void SmoothFollow()                                            
    {
        if (TargetPorte.Length > 0)
        {
            timer += Time.deltaTime;
            //Debug.Log("timing...");

            Vector3 smooth = new Vector3(TargetPorte[index].position.x, 1, TargetPorte[index].position.z) - transform.position;
            transform.position += smooth / speed;

            if (timer >= 2)
            {
                ResetTargets();
            }
        }
        else if (Target != null)
        {
            Vector3 smooth = new Vector3(Target.position.x, 1, Target.position.z) - transform.position;
            transform.position += smooth / speed;
        }
    }




    public void GetTargetPorte(Transform[] target)
    {
        TargetPorte = target;
        index = 0;
    }

    private void ResetTargets()
    {
        index++;
        if (TargetPorte.Length > index)
        {
            timer = 0;
            //Debug.Log("reset time");
        }
        else
        {
            timer = 0;
            //Debug.Log("reset array");
            TargetPorte = new Transform[0];
        }
    }



    #region Sound
    private AudioSource audioS;
    private AudioClip PistStock;
    public void LaunchSound(AudioClip ac)
    {
        if (!audioS.isPlaying)
        {
            audioS.clip = ac;
            audioS.Play();
        }
        else
        {
            PistStock = ac;
        }
    }

    private void AudioPlaying()
    {
        if (!audioS.isPlaying)
        {
            if (PistStock)
            {
                audioS.clip = PistStock;
                PistStock = null;
                audioS.Play();
            }
            else
            {
                audioS.clip = null;
            }
        }
    }

    #endregion
}
