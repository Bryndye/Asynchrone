using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trap_interaction : MonoBehaviour
{
    private Trap_script traps;
    [SerializeField]
    private GameObject myUI;


    private void Awake()
    {
        traps = GetComponentInParent<Trap_script>();

        myUI = Instantiate(Resources.Load<GameObject>("UI/Following/Interaction Robot"));
        myUI.GetComponent<InteractionUI>().Declaration(GetComponentInChildren<Renderer>());
    }
    public void Called()
    {
        if (traps != null)
        {
            traps.Called(gameObject);
        }
    }
}
