using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridMaker))]

public class GridMakerEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

       
        GridMaker myScript = (GridMaker)target;
        if (GUILayout.Button("Refresh Grid"))
        {
            myScript.RefreshGrid();
        }
    }

}
