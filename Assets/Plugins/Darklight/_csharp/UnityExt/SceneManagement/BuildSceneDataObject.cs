using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.SceneManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Darklight.UnityExt.SceneManagement
{
    /// <summary>
    /// Base class for scene data objects.
    /// </summary>
    public class BuildSceneDataObject<TSceneData> : ScriptableObject
        where TSceneData : BuildSceneData, new()
    {
        private string[] buildScenePaths = new string[0];
        [SerializeField] protected TSceneData[] buildSceneData = new TSceneData[0];

        /// <summary>
        /// Saves the build scene data by updating the paths of the BuildSceneData objects
        /// based on the paths in the EditorBuildSettingsScene array.
        /// </summary>
        public void SaveBuildSceneData(string[] buildScenePaths)
        {
            this.buildScenePaths = buildScenePaths;
            TSceneData[] tempData = new TSceneData[buildScenePaths.Length];

            for (int i = 0; i < buildScenePaths.Length; i++)
            {
                string scenePath = buildScenePaths[i];

                // If the current data array is smaller than the build scene paths array, or the path at the current index is different, create a new scene data object.
                if (this.buildSceneData.Length < i + 1 || this.buildSceneData[i].Path != scenePath)
                {
                    Debug.Log($"{this.name} -> Creating new scene data for {scenePath}.");
                    tempData[i] = new TSceneData();
                    tempData[i].InitializeData(scenePath);
                }
                // Otherwise, use the existing scene data.
                else
                {
                    tempData[i] = this.buildSceneData[i];
                }
            }

            // Update the build scene data.
            this.buildSceneData = tempData;
            EditorUtility.SetDirty(this);
            Debug.Log($"{this.name} Saved build scene data.");
        }

        public virtual List<TSceneData> GetAllBuildSceneData()
        {
            return buildSceneData.ToList();
        }

        /// <summary>
        /// Retrieves the scene data for a given scene name.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <returns>The scene data for the specified scene name.</returns>
        public TSceneData GetSceneData(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning(
                    $"{this.name} Cannot get scene data for null or empty scene name."
                );
                return null;
            }

            // Get the scene data of the specified data type.
            TSceneData data = this.buildSceneData.ToList().Find(x => x.Name == sceneName);
            return data;
        }

        /// <summary>
        /// Retrieves the scene data for a given scene.
        /// </summary>
        /// <param name="scene"></param>
        /// <returns>The scene data for the specified scene.</returns>
        public TSceneData GetSceneData(Scene scene)
        {
            return GetSceneData(scene.name);
        }

        /// <summary>
        /// Retrieves the data for the active scene.
        /// </summary>
        public TSceneData GetActiveSceneData()
        {
            Scene scene = SceneManager.GetActiveScene();
            return GetSceneData(scene.name);
        }

        public void ClearBuildSceneData()
        {
            buildSceneData = new TSceneData[0];
            EditorUtility.SetDirty(this);
            Debug.Log($"{this.name} Cleared build scene data.");
        }
    }
}
