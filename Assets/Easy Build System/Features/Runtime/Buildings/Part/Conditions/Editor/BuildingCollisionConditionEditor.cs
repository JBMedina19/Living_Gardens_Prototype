﻿/// <summary>
/// Project : Easy Build System
/// Class : BuildingCollisionConditionEditor.cs
/// Namespace : EasyBuildSystem.Features.Runtime.Buildings.Part.Conditions.Editor
/// Copyright : © 2015 - 2022 by PolarInteractive
/// </summary>

using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using EasyBuildSystem.Features.Runtime.Buildings.Manager;

namespace EasyBuildSystem.Features.Runtime.Buildings.Part.Conditions.Editor
{
    [CustomEditor(typeof(BuildingCollisionCondition))]
    public class BuildingCollisionConditionEditor : UnityEditor.Editor
    {
        #region Fields

        BuildingCollisionCondition Target
        {
            get
            {
                return ((BuildingCollisionCondition)target);
            }
        }

        #endregion

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_LayerMask"), new GUIContent("Building Collision Layers",
                "Layers that will be taken into account during the detection.."));

            if (BuildingManager.Instance.AllSurfaces.Count == 0)
            {
                EditorGUILayout.HelpBox("No Building Surface found in the current scene!", MessageType.Warning);
                GUI.enabled = false;
                Target.CollisionBuildingSurfaceFlags = EditorGUILayout.MaskField(new GUIContent("Building Collision Require Surfaces",
                    "Required collision surface(s) for allowing the placement."),
                    Target.CollisionBuildingSurfaceFlags, new string[1] { "Empty" });
                GUI.enabled = true;
            }
            else
            {
                serializedObject.FindProperty("m_BuildingSurfaceFlags").intValue =
                    EditorGUILayout.MaskField(new GUIContent("Building Collision Require Surfaces", "Required collision surface(s) for allowing the placement."), 
                    Target.CollisionBuildingSurfaceFlags, BuildingManager.Instance.AllSurfaces.ToArray());
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Tolerance"), new GUIContent("Building Collision Tolerance",
                "Collision tolerance for allowing the placement."));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ShowDebugs"), new GUIContent("Show Debugs"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ShowGizmos"), new GUIContent("Show Gizmos"));

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        #endregion
    }
}