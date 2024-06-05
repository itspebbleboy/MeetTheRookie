using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Darklight.UnityExt.SceneManagement
{
#if UNITY_EDITOR

    public class BuildSceneManagerWindow : EditorWindow
    {
        private BuildSceneManager buildSceneManager => BuildSceneManager.Instance;
        private Type _dataManagerType;
        private Type _dataType;
        private Vector2 scrollPosition;
        private SerializedObject serializedObject;
        private SerializedObject dataObjectSerializedObject;

        [MenuItem("DarklightExt/BuildSceneManager")]
        public static void ShowWindow()
        {
            GetWindow<BuildSceneManagerWindow>("BuildSceneManager");
        }

        void OnEnable()
        {
            serializedObject = new SerializedObject(buildSceneManager);

            // Get the manager type
            _dataManagerType = buildSceneManager.GetType();

            // Get the data type
            bool result = IsSubclassOfRawGeneric(typeof(BuildSceneDataManager<>), _dataManagerType);
            if (result)
            {
                _dataType = _dataManagerType.BaseType.GetGenericArguments()[0];
            }

            // Load the build scenes
            buildSceneManager.Initialize();

            // Get the buildSceneDataObject field
            FieldInfo buildSceneDataObjectField = _dataManagerType.GetField(
                "buildSceneDataObject",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            // If the buildSceneDataObject field is found, and is of type BuildSceneDataObject, get the serialized object
            if (buildSceneDataObjectField != null)
            {
                Debug.Log("BuildSceneDataObject found.");

                ScriptableObject buildSceneDataObject =
                    buildSceneDataObjectField.GetValue(buildSceneManager) as ScriptableObject;
                if (buildSceneDataObject != null)
                {
                    dataObjectSerializedObject = new SerializedObject(buildSceneDataObject);
                }
            }
        }

        static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                Type cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        void OnGUI()
        {
            serializedObject.Update();

            if (_dataType == null)
            {
                GUILayout.Label("No BuildSceneDataManager found.");
                return;
            }

            GUILayout.Label("DataManager Type: " + _dataManagerType.Name);
            GUILayout.Label("Data Type: " + _dataType.Name);

            GUILayout.Space(10);

            GUILayout.Label("BuildSceneData", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                buildSceneManager.Initialize();
            }

            if (GUILayout.Button("Clear All Data"))
            {
                buildSceneManager.ClearBuildScenes();
            }

            if (GUILayout.Button("Open Build Settings"))
            {
                EditorApplication.ExecuteMenuItem("File/Build Settings...");
            }
            EditorGUILayout.EndHorizontal();

            // Start the scroll view
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Display the properties of buildSceneDataObject
            if (dataObjectSerializedObject != null)
            {
                GUILayout.Label("BuildSceneDataObject", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("buildSceneDataObject"));

                // Draw the data object properties
                dataObjectSerializedObject.Update();
                SerializedProperty dataProperty = dataObjectSerializedObject.FindProperty("buildSceneData");
                EditorGUILayout.PropertyField(dataProperty, true);
                dataObjectSerializedObject.ApplyModifiedProperties();
            }
            else
            {
                GUILayout.Label("No BuildSceneDataObject found.");
            }

            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
