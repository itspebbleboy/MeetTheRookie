using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Darklight.UnityExt.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.SceneManagement
{
    public class BuildSceneDataManager<TSceneData> : BuildSceneManager
        where TSceneData : BuildSceneData, new()
    {
        public const string DATA_PATH = "Assets/Resources/BuildSceneData";
        public const string DATA_FILENAME = "BuildSceneDataObject";

        [SerializeField] protected BuildSceneDataObject<TSceneData> buildSceneDataObject;

        public override void Awake()
        {
            base.Awake();
        }

        public override void Initialize()
        {
#if UNITY_EDITOR
            base.LoadBuildScenes();
            CreateBuildSceneDataObject();
            buildSceneDataObject.SaveBuildSceneData(buildScenePaths);
#endif
        }

        public virtual void CreateBuildSceneDataObject()
        {
            buildSceneDataObject = ScriptableObjectUtility.CreateOrLoadScriptableObject<BuildSceneDataObject<TSceneData>>(
                DATA_PATH,
                DATA_FILENAME
            );
        }

        /// <summary>
        /// Subscribes to SceneManager events.
        /// </summary>
        void OnEnable()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        /// <summary>
        /// Unsubscribes from SceneManager events.
        /// </summary>
        void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

#if UNITY_EDITOR

        public override void ClearBuildScenes()
        {
            buildSceneDataObject.ClearBuildSceneData();
            base.ClearBuildScenes();
        }
#endif
    }
}
