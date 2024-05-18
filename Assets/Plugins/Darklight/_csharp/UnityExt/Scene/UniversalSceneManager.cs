using System.Collections.Generic;
using System.Linq;

using Darklight.Utility;
using Darklight.UnityExt.Editor;

using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;



#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace Darklight.UnityExt.Scene
{

    /// <summary>
    /// Universal Scene Manager, used to manage scenes in all aspects
    /// </summary>
    public class UniversalSceneManager : MonoBehaviourSingleton<UniversalSceneManager>
    {
        [SerializeField, ShowOnly] string sceneBuildDirectory = "Assets/Scenes/Build"; // Updated path to be relative to the Assets folder
        [SerializeField, ShowOnly] string currentScene;
        [SerializeField, ShowOnly] private List<SceneAsset> scenesInBuild = new List<SceneAsset>();
        public string SceneDirectory => sceneBuildDirectory;
        public List<SceneAsset> ScenesInBuild { get => scenesInBuild; set => scenesInBuild = value; }
        public string CurrentScene { get => currentScene; set => currentScene = value; }

        public bool IsSceneInBuild(SceneAsset scene)
        {
            return scenesInBuild.Contains(scene);
        }

        public bool GoToScene(string sceneName)
        {
            if (scenesInBuild.Count == 0)
            {
                Debug.LogError("No scenes in build settings");
                return false;
            }

            SceneAsset scene = scenesInBuild.Find(s => s.name == sceneName);
            if (scene == null)
            {
                Debug.LogError($"Scene {sceneName} not found in build settings");
                return false;
            }

            //Addressables.LoadScene(sceneName);
            SceneManager.LoadScene(sceneName);
            currentScene = sceneName;
            return true;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(UniversalSceneManager))]
    public class SceneManagerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        UniversalSceneManager _script;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (UniversalSceneManager)target;
            LoadBuildScenes();

            _script.CurrentScene = SceneManager.GetActiveScene().name;
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
                LoadBuildScenes();
            }


            // Save changes
            if (GUI.changed)
            {
                SceneSelector();
                EditorUtility.SetDirty(_script);
                EditorSceneManager.MarkSceneDirty(_script.gameObject.scene);
            }
        }

        public void SceneSelector()
        {
            // Get all scenes in the build settings
            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            // Create an array of scene names to display in the dropdown
            string[] sceneNames = scenes
                .Select(s => System.IO.Path.GetFileNameWithoutExtension(s))
                .ToArray();

            // Current selected index
            int currentIndex = System.Array.IndexOf(scenes, _script.CurrentScene);
            if (currentIndex == -1) currentIndex = 0;

            // Dropdown for selecting scene
            int selectedIndex = EditorGUILayout.Popup("Select Scene", currentIndex, sceneNames);
            _script.CurrentScene = scenes[selectedIndex];
        }

        private void LoadBuildScenes()
        {
            string[] scenePaths = Directory.GetFiles(_script.SceneDirectory, "*.unity", SearchOption.AllDirectories);
            List<SceneAsset> scenes = new List<SceneAsset>();
            foreach (string scenePath in scenePaths)
            {
                SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                scenes.Add(scene);
            }
            _script.ScenesInBuild = scenes;
        }
    }
#endif
}


