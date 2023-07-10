/// <summary>
/// Project : Easy Build System
/// Class : BuildingTypeDrawer.cs
/// Namespace : EasyBuildSystem.Features.Runtime.Buildings.Part.Editor
/// Copyright : © 2015 - 2022 by PolarInteractive
/// </summary>

using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Features.Runtime.Buildings.Part.Editor
{
    [CustomPropertyDrawer(typeof(BuildingTypeAttribute))]
    public class BuildingTypeDrawer : PropertyDrawer
    {
        #region Fields

        static string[] m_AllTypes;

        #endregion

        #region Unity Methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GetData();

            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "The attribute runs just on strings.", MessageType.Error);
            }

            if (m_AllTypes != null)
            {
                int selectedItem = IndexOfString(property.stringValue, m_AllTypes);
                selectedItem = EditorGUI.Popup(position, label.text, selectedItem, m_AllTypes);
                property.stringValue = StringAtIndex(selectedItem, m_AllTypes);
            }
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        #endregion

        #region Internal Methods

        void GetData()
        {
            m_AllTypes = GetBuildingPartTypes().ToArray();
        }

        int IndexOfString(string str, string[] allStrings)
        {
            for (int i = 0; i < allStrings.Length; i++)
            {
                if (allStrings[i] == str)
                {
                    return i;
                }
            }

            return 0;
        }

        string StringAtIndex(int i, string[] allStrings)
        {
            return allStrings.Length > i ? allStrings[i] : "";
        }

        List<string> GetBuildingPartTypes()
        {
            List<string> types = new List<string>();

            List<BuildingPart> buildingParts = FindAllBuildingPart();

            for (int i = 0; i < buildingParts.Count; i++)
            {
                if (buildingParts[i].GetGeneralSettings.Type != string.Empty)
                {
                    types.Add(buildingParts[i].GetGeneralSettings.Type);
                }
            }

            return types;
        }

        List<BuildingPart> FindAllBuildingPart()
        {
            List<BuildingPart> buildingParts = new List<BuildingPart>();

            string[] guids = AssetDatabase.FindAssets("l:BuildingPart", new[] { "Assets/" });

            foreach (string guid in guids)
            {
                string objectPath = AssetDatabase.GUIDToAssetPath(guid);

                Object[] loadedObjects = LoadAllAssetsAtPath(objectPath);

                foreach (Object loadedObject in loadedObjects)
                {
                    if (loadedObject != null && loadedObject.GetType().Name == "BuildingPart")
                    {
                        BuildingPart buildingPart = ((BuildingPart)loadedObject);

                        if (buildingPart != null)
                        {
                            buildingParts.Add(buildingPart);
                        }
                    }
                }
            }

            return buildingParts;
        }

        Object[] LoadAllAssetsAtPath(string assetPath)
        {
            return typeof(SceneAsset).Equals(AssetDatabase.GetMainAssetTypeAtPath(assetPath)) ?
                new[] { AssetDatabase.LoadMainAssetAtPath(assetPath) } :
                AssetDatabase.LoadAllAssetsAtPath(assetPath);
        }

        #endregion
    }
}