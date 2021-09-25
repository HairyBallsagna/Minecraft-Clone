using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        World world = (World) target;

        if (DrawDefaultInspector())
        {
            if (world.autoUpdate)
            {
                world.GenerateWorld();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            world.GenerateWorld();
        }

        if (GUILayout.Button("Destroy Chunks"))
        {
            world.DestroyChunks();
        }
    }
}
