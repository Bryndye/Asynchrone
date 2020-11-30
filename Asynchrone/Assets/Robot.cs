using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    NavMeshAgent nav;

    public bool BackToHuman;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        
    }

    public void Linked()
    {
        if (true)
        {

        }
    }
}
