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

        if (GUILayout.Button("Changer l'ennemi de classe"))
        {
            myAI.ChangeClass();
        }
        EditorGUILayout.Space();

        if (GUILayout.Button("Ajouter la position actuelle à la patrouille"))
        {
            myAI.AddPatrolPoint();
        }
        if (GUILayout.Button("Ajouter la position actuelle à la patrouille secondaire"))
        {
            myAI.AddSecondaryPatrolPoint();
        }
        if (GUILayout.Button("Supprimer la patrouille"))
        {
            myAI.ResetPath();
        }
        EditorGUILayout.Space();

        if (GUILayout.Button("Ajouter la rotation actuelles au balayage de garde"))
        {
            myAI.AddRotationPoint();
        }
        if (GUILayout.Button("Supprimer le balayage de garde"))
        {
            myAI.ResetRotations();
        }
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        if (GUILayout.Button("Aller à la position de base"))
        {
            myAI.ResetPositionToFirstPatrolPoint();
        }
    }
}
