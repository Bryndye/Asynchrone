using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(anAI))]
public class IAEditor : Editor
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

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        anAI myAI = (anAI)target;

        if (GUILayout.Button("Add current position to patrol"))
        {
            myAI.AddPatrolPoint();
        }
        if (GUILayout.Button("Add current position to secondary patrol"))
        {
            myAI.AddSecondaryPatrolPoint();
        }
        if (GUILayout.Button("Go to base position"))
        {
            myAI.ResetPositionToFirstPatrolPoint();
        }
        if (GUILayout.Button("Supprimer la patrouille"))
        {
            myAI.ResetPath();
        }
    }
}
