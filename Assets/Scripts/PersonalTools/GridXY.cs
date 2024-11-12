using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using Personal.Utilities;

namespace Personal.GridFramework
{
    /// <summary>
    /// 2D grid placed on XY coordinates
    /// </summary>
    /// <typeparam name="TObject"> Object contained in the grid cells</typeparam>
    //TO DO: instead of remaking the entire class for every grid (XY, XZ, YZ),
    //make the grids reuse same code between each other
    public class GridXY<TObject>
    {
        int _columns;
        int _rows;
        float _cellSize;
        Vector3 _origin;
        Vector3 _offset;

        
        TObject[,] _gridArray;

        public bool debug = true;
        TextMesh[,] _debugTextArray;

        Action<Vector2Int> OnObjectChanged;

        /// <summary>
        /// Horizontal grid, set on the XZ dimensions. Can contain anything. 
        /// Default object: first int = x, 2nd int = y, TObject = type of object the grid contains
        /// </summary>
        /// <param name="columns"> the width paramter (worldspace x) </param>
        /// <param name="rows"> the height parameter (worldspace y)</param>
        /// <param name="cellSize">size of a each cell</param>
        /// <param name="origin"></param>
        /// <param name="defaultObject"></param>
        public GridXY(int columns, int rows, float cellSize, Vector3 origin,
                        Func<GridXY<TObject>, int, int, TObject> defaultObject)
        {
            _columns = columns;
            _rows = rows;
            _cellSize = cellSize;
            _origin = origin;

            _offset = new Vector3(_cellSize, _cellSize, 0) * 0.5f; //origin of cells set to middle

            _gridArray = new TObject[columns, rows];
            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {

                    _gridArray[x, y] = defaultObject(this, x, y);
                }
            }

            if (debug) 
            {
                _debugTextArray = new TextMesh[columns, rows];
                showDebug(_columns, _rows);
                OnObjectChanged += changeDebugText;
            } 
        }

        /// <summary>
        /// Show the debug gizmos; XZ coords, grid lines, name of the grid object content. 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        private void showDebug(int columns, int rows)
        {
            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {

                    Debug.DrawLine(GridToWorldPosition(x, y, true), GridToWorldPosition(x, y + 1, true), Color.white, 1000f);
                    Debug.DrawLine(GridToWorldPosition(x, y, true), GridToWorldPosition(x + 1, y, true), Color.white, 1000f);
                    _debugTextArray[x, y] = TextTools.CreateTextInWorld(_gridArray[x, y]?.ToString(), null, GridToWorldPosition(x, y) + _offset, 
                                                                        10, Color.white, new Vector3(0, 0, 0), TextAnchor.MiddleCenter); //TODO: these variables should be changeable in editor
                }
            }
            Debug.DrawLine(GridToWorldPosition(0, rows, true), GridToWorldPosition(columns, rows, true), Color.white, 1000f);
            Debug.DrawLine(GridToWorldPosition(columns, 0, true), GridToWorldPosition(columns, rows, true), Color.white, 1000f);

            
        }

        public Vector3 GetCellOffset()
        {
            return _offset;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>Amount of columns in the grid (int)</returns>
        public int GetWidthInColumns()
        {
            return _columns;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Amount of rows in the grid (int)</returns>
        public int GetHeightInRows()
        {
            return _rows;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetCellSize()
        {
            return _cellSize;
        }

        public bool CheckInBounds(int x, int y)
        {
            return ((x >= 0 && y >= 0) && (x < _columns && y <_rows));
        }
        public bool CheckInBounds(Vector2Int coords)
        {
            return CheckInBounds(coords.x, coords.y);
        }

        /// <summary>
        /// Convert grid coords to world position. Needs to be within the grid.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns>World position at location column x, row z.</returns>
        public Vector3 GridToWorldPosition(int x, int y, bool ignoreBounds = false)
        {

            if (CheckInBounds(x, y) || ignoreBounds) return new Vector3(x, y) * _cellSize + _origin;

            else return new Vector3(-1, -1, -1);
        }

        /// <summary>
        /// Converts world postition to grid coords.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns>Returns the grid coords (x, z), if either is -1, location is out of bounds </returns>
        public Vector2Int WorldToGridPostion(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt((worldPosition - _origin).x / _cellSize);
            int y = Mathf.FloorToInt((worldPosition - _origin).y / _cellSize);

            if (CheckInBounds(x, y)) return new Vector2Int(x, y);
            else return new Vector2Int(-1, -1);
        }

        /// <summary>
        /// Set grid object on grid using grid coordinates, 2 in format
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        public void SetCellContent(int x, int y, TObject value) //
        {
            if (CheckInBounds(x, y))
            {
                _gridArray[x, y] = value;
                OnObjectChanged?.Invoke(new Vector2Int(x, y));
            }
        }
        /// <summary>
        /// Set grid object on grid using grid coordinates, Vector2Int format.
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="value"></param>
        public void SetCellContent(Vector2Int coords, TObject value) //
        {
            SetCellContent(coords.x, coords.y, value);
        }
        /// <summary>
        /// Set grid object on grid using world position.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="value"></param>
        public void SetCellContent(Vector3 worldPosition, TObject value) //set value based on world position
        {
            Vector2Int coords = WorldToGridPostion(worldPosition);
            SetCellContent(coords.x, coords.y, value);
        }


        /// <summary>
        /// Find object on grid coords x, z. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public TObject GetCellContent(int x, int y)
        {
            if (CheckInBounds(x, y))
            {
                return _gridArray[x, y];
            }
            else return default;
        }

        public TObject GetCellContent(Vector2Int coords)
        {
            return GetCellContent(coords.x, coords.y);
        }

        /// <summary>
        /// Returns object on grid at world position.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public TObject GetCellContent(Vector3 worldPosition)
        {
            Vector2Int gridCoords = WorldToGridPostion(worldPosition);
            return GetCellContent(gridCoords.x, gridCoords.y);
        }

        void changeDebugText(Vector2Int coordinates)
        {
            _debugTextArray[coordinates.x, coordinates.y].text = _gridArray[coordinates.x, coordinates.y].ToString();
        }
    }
}

