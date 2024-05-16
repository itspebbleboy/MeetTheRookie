using System;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using UnityEngine;
using Darklight.Game.Grid;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Darklight/Grid2DPreset")]
/// <summary>
/// A scriptable object that contains the preset settings for a Grid2D instance.
/// </summary>
public class Grid2D_Preset : ScriptableObject
{

    #region << Create Unique ID >> ------------------------------------ >>
    [ShowOnly] public string uniqueID;
    private void OnValidate()
    {

#if UNITY_EDITOR
        if (uniqueID == "")
        {
            uniqueID = GUID.Generate().ToString();
        }
        EditorUtility.SetDirty(this);
#endif
    }
    #endregion

    #region << Grid Layout Settings >> -------------------------------- >>
    const int MIN = 1;
    const int MAX = 10;

    [Header("Grid Layout Settings")]
    [Range(MIN, MAX)] public int gridSizeX = 3;
    [Range(MIN, MAX)] public int gridSizeY = 3;
    [Range(0.1f, 10f)] public float cellSize = 1;
    [Range(-MAX, MAX)] public int originKeyX = 1;
    [Range(-MAX, MAX)] public int originKeyY = 1;
    #endregion

    [Header("Serialized Data Values")]
    [SerializeField, ShowOnly] private List<Vector2Int> keys = new List<Vector2Int>();
    [SerializeField, ShowOnly] private List<Grid2D_SerializedData> values = new List<Grid2D_SerializedData>();

    // Not serialized, rebuilt on load
    private Dictionary<Vector2Int, Grid2D_SerializedData> dataMap = new Dictionary<Vector2Int, Grid2D_SerializedData>();

    private void OnEnable()
    {
        // Rebuild the dictionary from the serialized lists
        dataMap.Clear();
        for (int i = 0; i < keys.Count && i < values.Count; i++)
        {
            dataMap[keys[i]] = values[i];
        }
    }

    /// <summary>
    /// Save the data to the dictionary and serialized lists
    /// </summary>
    public void SaveData(Grid2D_Data data)
    {
        Vector2Int positionKey = data.positionKey;
        Grid2D_SerializedData serializedData = new Grid2D_SerializedData(data);

        if (dataMap.ContainsKey(positionKey))
        {
            dataMap[positionKey] = serializedData;
            int index = keys.IndexOf(positionKey);
            values[index] = serializedData;
        }
        else
        {
            dataMap.Add(positionKey, serializedData);
            keys.Add(positionKey);
            values.Add(serializedData);
        }
        MarkAsDirty();
    }

    public Grid2D_SerializedData LoadData(Vector2Int positionKey)
    {
        if (dataMap.ContainsKey(positionKey))
        {
            return dataMap[positionKey];
        }
        return null;
    }

    private void MarkAsDirty()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }

#if UNITY_EDITOR

    // Custom Editor for the Grid2D_Preset ScriptableObject
    [CustomEditor(typeof(Grid2D_Preset))]
    public class Grid2DPresetEditor : Editor
    {
        GameObject selectedGameObject;

        void OnEnable()
        {
            // Subscribe to the Scene GUI event
            SceneView.duringSceneGui += Draw;
        }

        void OnDisable()
        {
            // Unsubscribe to prevent memory leaks
            SceneView.duringSceneGui -= Draw;
        }

        void Draw(SceneView sceneView)
        {
            Grid2D_Preset preset = target as Grid2D_Preset;
            if (preset != null && selectedGameObject != null)
            {
                //Grid2D.DrawGrid2D(preset);
            }
        }

        public override void OnInspectorGUI()
        {

            Grid2D_Preset script = (Grid2D_Preset)target;

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            #region ---- [[ SELECT GAMEOBJECT FROM SCENE ]] ----

            // Inspector Button to select a GameObject from the scene
            if (selectedGameObject == null && GUILayout.Button("Select Temporary Origin GameObject"))
            {
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, true, "", 0);
            }
            else if (selectedGameObject != null && GUILayout.Button("Clear Selected GameObject"))
            {
                selectedGameObject = null;
            }

            // Check if the object picker has been closed
            if (Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == 0)
            {
                // Get the selected GameObject
                selectedGameObject = EditorGUIUtility.GetObjectPickerObject() as GameObject;
                if (selectedGameObject != null)
                {
                    EditorUtility.SetDirty(script);
                }
            }

            // Display the selected GameObject
            if (selectedGameObject)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Selected GameObject:", selectedGameObject.name);
                EditorGUILayout.LabelField("Position:", selectedGameObject.transform.position.ToString());
            }
            #endregion

            // Draw the default inspector
            CustomInspectorGUI.DrawDefaultInspectorWithoutSelfReference(serializedObject);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

#endif

}