using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trap_interaction : MonoBehaviour
{
    private Trap_script traps;

    private void Awake()
    {
        traps = GetComponentInParent<Trap_script>();
    }
    public void Called()
    {
        if (traps != null)
        {
            traps.Called(gameObject);
        }
    }
}
