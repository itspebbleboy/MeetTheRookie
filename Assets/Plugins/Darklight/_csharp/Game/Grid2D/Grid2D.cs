using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Grid
{

    #region ---- [[ GRID2D ]] -----------------------------------------------------------
    public abstract class Grid2D : MonoBehaviour
    {
        private const string DEFAULT_PRESET_PATH = "Grid2D/Simple_1x1";

        [Header("Grid2D")]
        [Expandable, Tooltip("A scriptable object that contains the preset settings for a Grid2D instance.")]
        [SerializeField] private Grid2D_Preset _preset; // The settings for the grid


        // ------------------- [[ PUBLIC ACCESSORS ]] -------------------
        public Grid2D_Preset Preset
        {
            get
            {
                if (_preset == null)
                {
                    _preset = Resources.Load<Grid2D_Preset>(DEFAULT_PRESET_PATH);
                    if (_preset == null)
                    {
                        Debug.LogError("Default Grid2D_Preset not found. Please assign a valid preset.");
                    }
                }
                return _preset;
            }
        }
        public Vector2Int GridArea => new Vector2Int(_preset.gridSizeX, _preset.gridSizeY); // The Vect2Int size of the grid
        public Vector2Int OriginKey => new Vector2Int(_preset.originKeyX, _preset.originKeyY); // The origin key of the grid


        /// <summary>
        /// Initializes the data map with default grid data values
        /// </summary>
        protected abstract void InitializeDataMap();

        /// <summary>
        /// Calculates the world space position of the specified position key in the grid.
        /// </summary>
        /// <param name="positionKey">The position key in the grid.</param>
        /// <returns>The world space position of the specified position key.</returns>
        public Vector3 GetWorldPositionOfCell(Vector2Int positionKey)
        {
            Vector3 origin = transform.position + new Vector3(OriginKey.x * Preset.cellSize, OriginKey.y * Preset.cellSize, 0);
            Vector3 cellPos = origin + (new Vector3(positionKey.x, positionKey.y) * Preset.cellSize);
            return cellPos;
        }

        /// <summary>
        /// Draws the grid in the scene view from the given preset and origin position.
        /// </summary>
        /// <param name="preset">
        ///     The preset settings for the grid.
        /// </param>
        /// <param name="originWorldPosition">
        ///     The world position of the origin cell of the grid.
        /// </param>
        public static void DrawGrid2D(Grid2D grid2D)
        {
            Grid2D_Preset preset = grid2D.Preset;
            for (int x = 0; x < preset.gridSizeX; x++)
            {
                for (int y = 0; y < preset.gridSizeY; y++)
                {
                    Vector3 cellPos = grid2D.GetWorldPositionOfCell(new Vector2Int(x, y));
                    CustomGizmos.DrawWireSquare(cellPos, preset.cellSize, Vector3.forward, Color.green);
                }
            }
        }

        public virtual void OnDrawGizmosSelected()
        {
            DrawGrid2D(this);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Grid2D))]
    public class Grid2DCustomEditor : Editor
    {
        SerializedObject _serializedObject;
        Grid2D _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (Grid2D)target;
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
    #endregion --------------------------------------------------------------------------


    /// <summary>
    /// An adapted version of the Grid2D class that stores a generic data type.
    /// </summary>
    /// <typeparam name="TData">
    ///     The type of inherited Grid2D_Data to store in the grid.
    /// </typeparam>
    public class Grid2D<TData> : Grid2D where TData : IGrid2D_Data, new()
    {
        protected Dictionary<Vector2Int, TData> DataMap { get; private set; } = new Dictionary<Vector2Int, TData>();
        public IEnumerable<Vector2Int> PositionKeys => DataMap.Keys;
        public IEnumerable<TData> DataValues => DataMap.Values;

        public virtual void Awake()
        {
            InitializeDataMap();
        }

        /// <summary>
        /// Initializes the data map with default grid data values
        /// </summary>
        protected override void InitializeDataMap()
        {
            if (Preset == null) return;

            DataMap.Clear();
            for (int x = 0; x < GridArea.x; x++)
            {
                for (int y = 0; y < GridArea.y; y++)
                {
                    Vector2Int positionKey = new Vector2Int(x, y);
                    Vector3 worldPosition = GetWorldPositionOfCell(positionKey);

                    // Create a new data object & initialize it
                    TData newData = new TData();
                    Grid2D_SerializedData existingData = Preset.LoadData(positionKey);
                    if (existingData != null)
                    {
                        newData.Initialize(existingData, worldPosition, Preset.cellSize);
                    }
                    else
                    {
                        newData.Initialize(positionKey, true, 0, worldPosition, Preset.cellSize);
                    }

                    // Set the data in the map
                    if (DataMap.ContainsKey(positionKey))
                        DataMap[positionKey] = newData;
                    else
                        DataMap.Add(positionKey, newData);

                    // Notify listeners of the data change
                    newData.OnDataStateChanged += (data) =>
                    {
                        Preset.SaveData(data);
                    };
                }
            }
        }

        /// <summary>
        /// Retrieves the data at a given position in the grid.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual TData GetData(Vector2Int position)
        {
            DataMap.TryGetValue(position, out TData data);
            return data;
        }
    }


}