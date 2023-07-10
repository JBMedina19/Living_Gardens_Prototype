/// <summary>
/// Project : Easy Build System
/// Class : EditorBuildingPlacer.cs
/// Namespace : EasyBuildSystem.Features.Runtime.Buildings.Placer
/// Copyright : © 2015 - 2022 by PolarInteractive
/// </summary>

using UnityEngine;

using UnityEditor;

namespace EasyBuildSystem.Features.Runtime.Buildings.Placer
{
    [ExecuteInEditMode]
    public class EditorBuildingPlacer : BuildingPlacer
    {
        #region Unity Methods

#if UNITY_EDITOR
        void OnEnable()
        {
            SceneView.duringSceneGui += OnScene;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnScene;
        }

        void OnScene(SceneView sceneview)
        {
            base.Update();

            if (Event.current != null)
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    if (GetBuildMode != BuildMode.NONE)
                    {
                        ChangeBuildMode(BuildMode.NONE);
                    }
                }

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    if (GetBuildMode == BuildMode.PLACE)
                    {
                        PlacingBuildingPart();
                    }
                    else if (GetBuildMode == BuildMode.DESTROY)
                    {
                        DestroyBuildingPart();
                    }
                    else if (GetBuildMode == BuildMode.EDIT)
                    {
                        EditingBuildingPart();
                    }
                }

                if (Event.current.keyCode == KeyCode.R && Event.current.type == EventType.KeyUp)
                {
                    RotatePreview();
                }
            }
        }
#endif

        #endregion
    }
}