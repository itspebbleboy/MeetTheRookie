

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using Darklight.UnityExt.Editor;

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Darklight.UnityExt.SceneManagement
{
    /// <summary>
    /// A Serializable class that stores the data for a scene in the UnityEditor's build settings.
    /// </summary>
    [System.Serializable]
    public class BuildSceneData
    {
        [SerializeField, ShowOnly] private string _name = "default";
        [SerializeField, ShowOnly] private string _path;
        [SerializeField] private bool _sceneFound;
        private Scene _scene;

        public string Name => _name;
        public string Path => _path;

        // ------------------- [[ CONSTRUCTORS ]] -------------------
        public BuildSceneData(){}
        public BuildSceneData(string path) => InitializeData(path);

        public void InitializeData(string path)
        {
            this._path = path.Replace("\\", "/"); // Replace all backslashes with forward slashes
            this._name = _path.Replace(BuildSceneManager.BUILD_SCENE_DIRECTORY + "/", "").Replace(".unity", "");
            
            // Check if the scene is in the build settings
            bool isLocatedInBuildSettings = BuildSceneManager.IsSceneInBuild(this._path);
            if (isLocatedInBuildSettings)
            {
                // Get the scene
                this._scene = SceneManager.GetSceneByPath(this._path);
                this._sceneFound = this._scene != null;
            }
        }
    }
}