#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlacementGenerator))]
public class PlacementGUI : Editor
{/*
    public override void OnInspectorGUI()
    {
        PlacementGenerator placemetGen = (PlacementGenerator)target;
        if (DrawDefaultInspector())
        {
            if (placemetGen.autoUpdate)
            {

                placemetGen.Generate();
            }
        }

        if (GUILayout.Button("Generator"))
        {
            placemetGen.Generate();
        }
        else if (GUILayout.Button("Clear"))
        {
            placemetGen.Clear();
        }

        if (GUILayout.Button("Enemy Generator"))
        {
            placemetGen.GenerateEnemy();
        }
        else if (GUILayout.Button("Enemy Clear"))
        {
            placemetGen.ClearEnemy();
        }
    }*/
}
#endif
