using UnityEngine;
using UnityEditor;
using static UnityEditor.PlayerSettings;

[CustomEditor(typeof(HeatMapParent))]
public class EditHeatMapParameters : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HeatMapParent heatMapParent = (HeatMapParent)target;

        GUILayout.Label("Color Ramp", EditorStyles.boldLabel);

        if (heatMapParent.colorRamp == null || heatMapParent.colorRamp.Count < 2)
        {
            EditorGUILayout.HelpBox("Ramp needs at least 2 colors.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < heatMapParent.colorRamp.Count; i++)
            {
                heatMapParent.colorRamp[i] = EditorGUILayout.ColorField(heatMapParent.colorRamp[i], GUILayout.Width(40));
            }
            EditorGUILayout.EndHorizontal();

            Rect rect = GUILayoutUtility.GetRect(200, 20);
            EditorGUI.DrawRect(rect, Color.black);

            int segments = heatMapParent.colorRamp.Count - 1;
            float segmentWidth = rect.width / segments;

            for (int i = 0; i < segments; i++)
            {
                Color startColor = heatMapParent.colorRamp[i];
                Color endColor = heatMapParent.colorRamp[i + 1];

                for (int x = 0; x < segmentWidth; x++)
                {
                    float t = x / segmentWidth;
                    Color interpolatedColor = Color.Lerp(startColor, endColor, t);
                    float xPos = rect.x + (i * segmentWidth) + x;
                    EditorGUI.DrawRect(new Rect(xPos, rect.y, 1, rect.height), interpolatedColor);
                }
            }
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Add Color"))
        {
            heatMapParent.colorRamp.Add(Color.white);
        }

        if (GUILayout.Button("Remove Last Color") && heatMapParent.colorRamp.Count > 2)
        {
            heatMapParent.colorRamp.RemoveAt(heatMapParent.colorRamp.Count - 1);
        }

        GUILayout.Space(20);
        heatMapParent.cubeSize = EditorGUILayout.IntSlider("Geometry Size", heatMapParent.cubeSize, 2, 15);
        GUILayout.Space(20);
        heatMapParent.transparency = EditorGUILayout.Slider("Material Transparency", heatMapParent.transparency, 0.0f, 1.0f);

        GUILayout.Space(20);
        if (GUILayout.Button("Apply Changes"))
        {
            heatMapParent.OnEditParameters();
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Load DataBase HeatMap"))
        {
            heatMapParent.sendToServer.ReceiveDataBaseHit();
        }
        if (GUILayout.Button("Clean Scene HeatMap"))
        {
            heatMapParent.ClearAll();
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Load DataBase KillMap"))
        {
            heatMapParent.sendToServer.ReceiveDataBaseKill();
        }
        if (GUILayout.Button("Clean Scene KillMap"))
        {
            heatMapParent.ClearAllKillMap();
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Load DataBase Path"))
        {
            heatMapParent.sendToServer.ReceiveDataBasePath();
        }
        if (GUILayout.Button("Clean Scene Path"))
        {
            heatMapParent.sendToServer.pathParent.ClearAll();
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Fix HeatMap Colors"))
        {
            heatMapParent.CheckCubesCollisions();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(heatMapParent);
        }
    }
}