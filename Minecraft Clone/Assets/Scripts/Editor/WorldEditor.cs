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

        base.DrawDefaultInspector();
        
        if (world.autoUpdate)
        {
            if (DrawDefaultInspector())
            {
                world.GenerateWorld();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            world.GenerateWorld();
        }
    }
}
