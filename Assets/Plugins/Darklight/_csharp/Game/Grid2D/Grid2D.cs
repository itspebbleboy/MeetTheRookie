using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Editor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Grid
{

    #region ---- [[ GRID2D ]] -----------------------------------------------------------
    public abstract class Grid2D : MonoBehaviour
    {
        [Header("Grid2D Settings")]
        [SerializeField] private Grid2D_Preset _preset; // The settings for the grid
        [SerializeField, ShowOnly] private Vector2Int _gridArea; // The size of the grid2D
        [SerializeField, ShowOnly] private Vector2Int _originKey; // The origin key of the grid2D
        [SerializeField] public bool showGizmos = true;

        // ------------------- [[ PUBLIC ACCESSORS ]] -------------------
        private const string DEFAULT_PRESET_PATH = "Grid2D/Simple_1x3";
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
        public Vector2Int GridArea => new Vector2Int(_preset.gridSizeX, _preset.gridSizeY);
        public Vector2Int OriginKey => new Vector2Int(_preset.originKeyX, _preset.originKeyY);


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
            // Calculate the world space position
            Vector2Int offsetPosition = positionKey - OriginKey;
            Vector3 vec3_position = new Vector3(offsetPosition.x, offsetPosition.y, 0);
            vec3_position *= Preset.cellSize;

            // Transform the position to world space using this transform as the parent
            Vector3 worldSpacePosition = transform.TransformVector(transform.position + vec3_position);
            return worldSpacePosition;
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
        public static void DrawGrid2D(Grid2D_Preset preset, Vector3 originWorldPosition)
        {
            if (preset == null) return;
            if (originWorldPosition == null) originWorldPosition = Vector3.zero;

            Vector3 origin = originWorldPosition + new Vector3(preset.originKeyX * preset.cellSize, preset.originKeyY * preset.cellSize, 0);
            for (int x = 0; x < preset.gridSizeX; x++)
            {
                for (int y = 0; y < preset.gridSizeY; y++)
                {
                    Vector3 cellPos = origin + new Vector3(x * preset.cellSize, y * preset.cellSize, 0);
                    CustomGizmos.DrawWireSquare(cellPos, preset.cellSize, Vector3.forward, Color.green);
                }
            }
        }

        public void OnDrawGizmosSelected()
        {
            if (showGizmos)
            {
                DrawGrid2D(Preset, transform.position);
            }
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