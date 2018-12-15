using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Camera_ObjectLocker))]

public class CameraLockerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Camera_ObjectLocker myScript = (Camera_ObjectLocker)target;
        if (GUILayout.Button("Center camera on object"))
        {
            myScript.Lock();
        }

    }

}

