using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmooth : Singleton<CameraSmooth>
{
    public Transform Target;

    void FixedUpdate() => SmoothFollow();

    void SmoothFollow()
    {
        if (Target != null)
        {
            Vector3 smooth = new Vector3(Target.position.x, 1, Target.position.z) - transform.position;
            transform.position += smooth / 20;
        }
    }

    private void Awake()
    {
        if (Instance != this)
            Destroy(this);
    }
}
