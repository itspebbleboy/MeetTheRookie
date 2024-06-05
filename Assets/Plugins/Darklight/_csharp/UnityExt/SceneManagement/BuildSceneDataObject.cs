using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.SceneManagement
{
    /// <summary>
    /// Base class for scene data objects.
    /// </summary>
    public class BuildSceneDataObject<TSceneData> : ScriptableObject
        where TSceneData : BuildSceneData, new()
    {
        protected string[] buildScenePaths = new string[0];
        [SerializeField] protected TSceneData[] buildSceneData = new TSceneData[0];

        /// <summary>
        /// Saves the build scene data by updating the paths of the BuildSceneData objects
        /// based on the paths in the EditorBuildSettingsScene array.
        /// </summary>
        public virtual void SaveBuildSceneData(string[] buildScenePaths)
        {
            this.buildScenePaths = buildScenePaths;
            int buildScenePathsLength = buildScenePaths.Length;
            List<TSceneData> newSceneData = new List<TSceneData>(buildScenePathsLength);

            for (int i = 0; i < buildScenePathsLength; i++)
            {
                string scenePath = buildScenePaths[i];

                // If the current data array is smaller than the build scene paths array, or the path at the current index is different, create a new scene data object.
                if (this.buildSceneData.Length <= i || this.buildSceneData[i].Path != scenePath)
                {
                    //Debug.Log($"{this.name} -> Creating new scene data for {scenePath}.");
                    newSceneData.Add(new TSceneData());
                }
                else
                {
                    newSceneData.Add(this.buildSceneData[i]);
                }

                // Initialize the scene data.
                newSceneData[i].InitializeData(scenePath);
            }

            // Update the build scene data.
            this.buildSceneData = newSceneData.ToArray();

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
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
