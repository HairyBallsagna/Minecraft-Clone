using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor
{
    private World world;
    private Editor biomeEditor;
    private Editor noiseEditor;
    
    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed && world.autoUpdate)
                world.GenerateWorld();
        }
        
        DrawSettingsEditor(world.terrainGenerator.biomeGenerator, ref world.biomeSettingsFoldout, ref biomeEditor);
        DrawSettingsEditor(world.terrainGenerator.biomeGenerator.biomeNoiseData, ref world.noiseSettingsFoldout, ref noiseEditor);

        GUILayout.Space(20);
        GUILayout.Box("Buttons");
        
        if (GUILayout.Button("Generate"))
        {
            world.GenerateWorld();
        }

        if (GUILayout.Button("Destroy Chunks"))
        {
            world.DestroyChunks();
        }
    }

    private void DrawSettingsEditor(Object settings, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed && world.autoUpdate)
                    {
                        world.GenerateWorld();
                    }
                }
            }
        }
    }
    
    private void OnEnable()
    {
        world = (World) target;
    }
}
