using System.Collections;
using UnityEngine;
using UnityEditor;
using System.IO;


[CustomEditor(typeof(ScondMapGenerator))]
public class SecondaryMapGeneratorEditor : Editor
{/*
    public override void OnInspectorGUI()
    {
        ScondMapGenerator mapGen = (ScondMapGenerator)target;
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {

                mapGen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generator"))
        {
            mapGen.GenerateMap();
        }
    }*/
}
