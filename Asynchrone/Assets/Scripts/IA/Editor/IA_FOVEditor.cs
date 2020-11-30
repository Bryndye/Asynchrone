using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(anAI))]
public class IA_FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        anAI myAI = (anAI)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(myAI.transform.position, Vector3.up, Vector3.forward, 360, myAI.ViewRadius);
        Vector3 ViewAngleA = myAI.DirFromAngle(-myAI.ViewAngle / 2, false);
        Vector3 ViewAngleB = myAI.DirFromAngle(myAI.ViewAngle / 2, false);

        Handles.DrawLine(myAI.transform.position, myAI.transform.position + ViewAngleA * myAI.ViewRadius);
        Handles.DrawLine(myAI.transform.position, myAI.transform.position + ViewAngleB * myAI.ViewRadius);

        Handles.color = Color.red;
    }
}
