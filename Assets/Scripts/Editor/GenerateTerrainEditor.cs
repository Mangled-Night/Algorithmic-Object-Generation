using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateTerrain))]
public class GenerateTerrainEditor : Editor
{

    public override void OnInspectorGUI()
    {
        GenerateTerrain TerGen = (GenerateTerrain)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            if (Application.isPlaying)
            {
                TerGen.Generate();
            }
            else
            {
                Debug.LogWarning("Plase Enter Play Mode before generating");
            }
        }

    }
}
