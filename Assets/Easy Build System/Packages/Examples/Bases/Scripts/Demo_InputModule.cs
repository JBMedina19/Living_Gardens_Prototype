/// <summary>
/// Project : Easy Build System
/// Class : Demo_InputModule.cs
/// Namespace : EasyBuildSystem.Examples.Bases.Scripts
/// Copyright : © 2015 - 2022 by PolarInteractive
/// </summary>

using UnityEngine;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace EasyBuildSystem.Examples.Bases.Scripts
{
    [ExecuteInEditMode]
    public class Demo_InputModule : MonoBehaviour
    {
#if ENABLE_INPUT_SYSTEM

    void OnEnable()
    {
        if (GetComponent<InputSystemUIInputModule>() == null)
        {
            gameObject.AddComponent<InputSystemUIInputModule>();
            DestroyImmediate(GetComponent<StandaloneInputModule>());
        }
    }

#else

        void OnEnable()
        {
            if (GetComponent<StandaloneInputModule>() == null)
            {
                gameObject.AddComponent<StandaloneInputModule>();
            }
        }

#endif
    }
}