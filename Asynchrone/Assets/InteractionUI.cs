using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    CanvasManager canvasManager;


    public void Declaration(Renderer renderer)
    {
        canvasManager = CanvasManager.Instance;
        transform.parent = canvasManager.accrocheInteraction;

        GetComponent<aFollowingUI>().theRenderer = renderer;
    }
}
