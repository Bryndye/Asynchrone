using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasHolder : Singleton<CanvasHolder>
{
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != this)
            Destroy(this);
    }
}
