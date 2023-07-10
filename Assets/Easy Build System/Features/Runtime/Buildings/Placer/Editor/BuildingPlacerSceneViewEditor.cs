/// <summary>
/// Project : Easy Build System
/// Class : BuildingPlacerSceneViewEditor.cs
/// Namespace : EasyBuildSystem.Features.Buildings.Placer.Editor
/// Copyright : © 2015 - 2022 by PolarInteractive
/// </summary>

using UnityEngine;

using UnityEditor;

using EasyBuildSystem.Features.Runtime.Buildings.Placer;
using EasyBuildSystem.Features.Runtime.Buildings.Manager;

using EasyBuildSystem.Features.Editor.Extensions;

namespace EasyBuildSystem.Features.Buildings.Placer.Editor
{
    public class BuildingPlacerSceneViewEditor : EditorWindow
    {
        #region Fields

        static BuildingPlacer m_Builder;

        static Rect m_WindowRect = new Rect(10, 30, 375, 200);
        static Vector2 m_ScrollPosition;

        static bool m_Opened;

        #endregion

        #region Unity Methods

        [MenuItem("Tools/Easy Build System/Tools/Editor Building Placer...")]
        public static void Enable()
        {
            if (m_Opened)
            {
                return;
            }

            if (BuildingManager.Instance == null)
            {
                Debug.LogWarning("<b>Easy Build System</b> : The system is not setup on this scene!");
                return;
            }

            SceneView.duringSceneGui += OnScene;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            m_Opened = true;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                SceneView.duringSceneGui -= OnScene;
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

                if (m_Builder != null)
                {
                    DestroyImmediate(m_Builder.gameObject);
                }
            }
        }

        static void OnScene(SceneView sceneview)
        {
            if (m_Builder == null)
            {
                if (FindObjectOfType<EditorBuildingPlacer>() != null)
                {
                    m_Builder = FindObjectOfType<EditorBuildingPlacer>();
                }
                else
                {
                    m_Builder = new GameObject("(Instance) Building Placer Editor").AddComponent<EditorBuildingPlacer>();
                }

                m_Builder.GetRaycastSettings.ViewType = BuildingPlacer.RaycastSettings.RaycastType.TOP_DOWN_VIEW;
                m_Builder.GetRaycastSettings.Distance = 100f;

                m_Builder.GetSnappingSettings.MaxAngles = 5f;
            }

            if (m_Builder.GetBuildMode != BuildingPlacer.BuildMode.NONE)
            {
                if (Event.current.type == EventType.Layout)
                {
                    HandleUtility.AddDefaultControl(0);
                }
            }

            Handles.BeginGUI();

            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            GUI.backgroundColor = new Color(1, 1, 1, 0f);
            m_WindowRect = GUILayout.Window(controlId, m_WindowRect, WindowContent, "");
            GUI.backgroundColor = new Color(1, 1, 1, 1f);

            Handles.EndGUI();
        }

        static void WindowContent(int id)
        {
            GUILayout.Space(-20);

            GUILayout.BeginVertical("window");

            GUILayout.Space(-20);

            EditorGUIUtilityExtension.DrawHeader("Easy Build System - Editor Building Placer",
                "Place, destroy or edit directly from the Unity Editor scene viewport.\n" +
                "(Experimental) If you encounter any issues, thank you to report them!");

            GUILayout.BeginHorizontal();

            GUILayout.Label("Building Mode : " + m_Builder.GetBuildMode);

            GUILayout.Label("Building Part : " + (m_Builder.GetSelectedBuildingPart != null ? m_Builder.GetSelectedBuildingPart.GetGeneralSettings.Name : "NONE"));

            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            GUILayout.Label("Left Click : Place Preview | Right Click : Cancel Preview | R : Rotate Preview");

            EditorGUILayout.Separator();

            GUILayout.Label("Building Placer Settings", EditorStyles.boldLabel);

            EditorGUILayout.Separator();

            m_Builder.GetRaycastSettings.LayerMask = EditorGUIUtilityExtension.LayerMaskField("Raycast Layer :", m_Builder.GetRaycastSettings.LayerMask);

            m_Builder.GetSnappingSettings.Type =
                (BuildingPlacer.SnappingSettings.DetectionType)EditorGUILayout.EnumPopup("Raycast Type :", m_Builder.GetSnappingSettings.Type);

            m_Builder.GetSnappingSettings.MaxAngles = EditorGUILayout.Slider("Snapping Angle :", m_Builder.GetSnappingSettings.MaxAngles, 0, 90);

            EditorGUILayout.Separator();

            GUILayout.Label("Building Modes", EditorStyles.boldLabel);

            EditorGUILayout.Separator();

            GUILayout.BeginHorizontal();

            GUI.color = m_Builder.GetBuildMode == BuildingPlacer.BuildMode.PLACE ? Color.yellow : Color.white;
            if (GUILayout.Button("Place Mode"))
            {
                m_Builder.ChangeBuildMode(BuildingPlacer.BuildMode.PLACE);
            }
            GUI.color = Color.white;

            GUI.color = m_Builder.GetBuildMode == BuildingPlacer.BuildMode.DESTROY ? Color.yellow : Color.white;
            if (GUILayout.Button("Remove Mode"))
            {
                m_Builder.ChangeBuildMode(BuildingPlacer.BuildMode.DESTROY);
            }
            GUI.color = Color.white;

            GUI.color = m_Builder.GetBuildMode == BuildingPlacer.BuildMode.EDIT ? Color.yellow : Color.white;
            if (GUILayout.Button("Edit Mode"))
            {
                m_Builder.ChangeBuildMode(BuildingPlacer.BuildMode.EDIT);
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            GUILayout.Label("Building Parts", EditorStyles.boldLabel);

            EditorGUILayout.Separator();

            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, false, true, GUILayout.Height(100));

            if (BuildingManager.Instance.BuildingPartReferences.Count <= 0)
            {
                GUILayout.Label("No Building Parts available from Building Manager.");
            }
            else
            {
                for (int i = 0; i < BuildingManager.Instance.BuildingPartReferences.Count; i++)
                {
                    if (GUILayout.Button(new GUIContent(BuildingManager.Instance.BuildingPartReferences[i].GetGeneralSettings.Name,
                        BuildingManager.Instance.BuildingPartReferences[i].GetGeneralSettings.Thumbnail),
                        GUILayout.Width(430), GUILayout.Height(30)))
                    {
                        m_Builder.ChangeBuildMode(BuildingPlacer.BuildMode.NONE);
                        m_Builder.ChangeBuildMode(BuildingPlacer.BuildMode.PLACE);
                        m_Builder.SelectBuildingPart(BuildingManager.Instance.BuildingPartReferences[i]);
                    }
                }
            }

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close"))
            {
                m_Opened = false;

                SceneView.duringSceneGui -= OnScene;
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

                if (m_Builder != null)
                {
                    DestroyImmediate(m_Builder.gameObject);
                }
            }

            GUI.DragWindow();
            GUILayout.EndVertical();
        }

        #endregion
    }
}