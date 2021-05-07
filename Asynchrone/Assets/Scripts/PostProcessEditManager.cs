using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessEditManager : MonoBehaviour
{
    private Volume v;
    private Bloom b;
    private Vignette vg;
    private LiftGammaGain liftGG;
    // Start is called before the first frame update
    void Start()
    {
        v = GetComponent<Volume>();
        //v.profile.TryGet(out b);
        //v.profile.TryGet(out vg);
        v.profile.TryGet(out liftGG);
        Test();
    }

    [ContextMenu("test")]
    private void Test()
    {
        v.weight = 1;
        //Debug.Log(v.ToString() + b + vg + liftGG.ToString());
        //DONE
        //b.scatter.value = 0.1f;
        //vg.intensity.value = 0.5f;
        liftGG.lift.value = Vector4.zero;
    }
}
