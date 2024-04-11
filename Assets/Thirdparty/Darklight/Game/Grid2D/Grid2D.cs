using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darklight.Game.Grid2D
{

    /// <summary>
    /// A 2D Grid that can be used to store any type of data.
    /// </summary>
    /// <typeparam name="Data">The type of data to be stored in the grid.</typeparam>
    [System.Serializable]
    public class Grid2D<Data>
    {
        #region  [[ Coordinate Class ]] =============================== >>
        /// <summary>
        /// Represents a coordinate in the grid.
        /// </summary>
        class Coordinate
        {
            public bool active = true;
            public Vector2Int positionKey = Vector2Int.zero;
            public Data dataValue = default;
            public Coordinate(Vector2Int key, Data value)
            {
                this.positionKey = key;
                this.dataValue = value;
            }
        }
        #endregion

        #region [[ Public Properties ]] =============================== >>
        const int MIN = 1;
        const int MAX = 10;
        [SerializeField, Range(MIN, MAX)] public int gridSizeX = 3;
        [SerializeField, Range(MIN, MAX)] public int gridSizeY = 3;
        [SerializeField, Range(0.1f, 1f)] public float coordinateSize = 1;
        [SerializeField, Range(-MAX, MAX)] public int originKeyX = 0;
        [SerializeField, Range(-MAX, MAX)] public int originKeyY = 0;
        #endregion

        #region  Constructors -------------- 
        private Vector2Int gridXAxis => new Vector2Int(1, 0); // create x Axis Vector
        private Vector2Int gridYAxis => new Vector2Int(0, 1); // create y Axis Vector
        private Dictionary<Vector2Int, Coordinate> coordinateGrid = new Dictionary<Vector2Int, Coordinate>();
        private Vector2Int gridArea
        {
            get => new Vector2Int((int)gridSizeX, (int)gridSizeY);
            set
            {
                gridSizeX = value.x;
                gridSizeY = value.y;
            }
        }

        /// <summary> The parent transform of the grid. </summary>
        private Transform gridParent = null;
        private Vector2Int gridParentPositionKey => new Vector2Int(originKeyX, originKeyY);
        public Grid2D(Transform parent)
        {
            this.gridParent = parent;
            InitializeGridToSetValues();
        }

        public Grid2D(Transform parent, Vector2Int gridSize, int coordinateSize)
        {
            this.gridParent = parent;
            this.gridArea = gridSize;
            this.coordinateSize = coordinateSize;
            InitializeGridToSetValues();
        }
        #endregion

        #region [[ Public Methods ]] =============================== >>
        /// <summary>
        /// Initializes the grid by creating coordinates for each position in the grid.
        /// </summary>
        public void InitializeGridToSetValues()
        {
            coordinateGrid = new Dictionary<Vector2Int, Coordinate>();
            for (int x = 0; x < gridArea.x; x++)
            {
                for (int y = 0; y < gridArea.y; y++)
                {
                    Vector2Int position = gridXAxis * x + gridYAxis * y;
                    Coordinate coordinate = new Coordinate(position, default);
                    coordinateGrid.Add(position, coordinate);
                }
            }
        }

        /// <summary>
        /// Sets the parent transform of the grid.
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(Transform parent)
        {
            gridParent = parent;
        }

        /// <summary>
        /// Sets the value of a coordinate in the grid.
        /// </summary>
        /// <param name="position">The position of the coordinate.</param>
        /// <param name="value">The value to set.</param>
        public void SetCoordinateValue(Vector2Int position, Data value)
        {
            if (coordinateGrid.ContainsKey(position))
            {
                coordinateGrid[position].dataValue = value;
            }
        }

        /// <summary>
        /// Gets the dataValue of a coordinate in the grid.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Data GetData(Vector2Int position)
        {
            if (coordinateGrid.ContainsKey(position))
            {
                return coordinateGrid[position].dataValue;
            }
            return default;
        }

        /// <summary>
        /// Gets a list of all position keys in the grid.
        /// </summary>
        /// <returns>A list of all position keys in the grid.</returns>
        public List<Vector2Int> GetPositionKeys()
        {
            if (coordinateGrid != null)
                return new List<Vector2Int>(coordinateGrid.Keys);
            return new List<Vector2Int>();
        }

        public List<Data> GetDataValues()
        {
            List<Data> values = new List<Data>();
            if (values == null || values.Count == 0) return values;
            foreach (Coordinate coordinate in coordinateGrid.Values)
            {
                values.Add(coordinate.dataValue);
            }
            return values;
        }

        /// <summary>
        /// Gets the world space position of the specified position key in the grid.
        /// </summary>
        /// <param name="positionKey">The position key in the grid.</param>
        /// <returns>The world space position of the specified position key.</returns>
        public Vector3 GetWorldSpacePosition(Vector2Int positionKey)
        {
            if (this.gridParent == null) { return Vector3.zero; }

            Vector2Int offsetPosition = positionKey - gridParentPositionKey;
            Vector3 vec3_position = new Vector3(offsetPosition.x, offsetPosition.y, 0);
            vec3_position *= coordinateSize;

            Vector3 worldSpacePosition = gridParent.TransformVector(gridParent.position + vec3_position);
            return worldSpacePosition;
        }
        #endregion
    }
}
