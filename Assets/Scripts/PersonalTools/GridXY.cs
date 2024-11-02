using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
        private Vector3 _origin;
        private Vector3 _offset;

        TObject[,] _gridArray;
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

            _offset = new Vector3(_cellSize, 0, _cellSize) * 0.5f; //origin of cells set to middle

            _gridArray = new TObject[columns, rows];
            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < _gridArray.GetLength(1); z++)
                {

                    _gridArray[x, z] = defaultObject(this, x, z);
                }
            }
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
            return ((x >= 0 && y >= 0) && (x < _columns && y < _rows));
        }

        /// <summary>
        /// Convert grid coords to world position. Needs to be within the grid.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns>World position at location column x, row z.</returns>
        public Vector3 GridToWorldPosition(int x, int y)
        {
            if (CheckInBounds(x, y)) return new Vector3(x, y, 0) * _cellSize + _origin;

            else return new Vector3(-1, 0, -1);
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
        /// Set grid object on grid using grid coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="value"></param>
        public void SetGridObject(int x, int z, TObject value) //
        {
            if (CheckInBounds(x, z))
            {
                _gridArray[x, z] = value;
            }
        }
        /// <summary>
        /// Set grid object on grid using world position.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="value"></param>
        public void SetGridObject(Vector3 worldPosition, TObject value) //set value based on world position
        {
            Vector2Int coords = WorldToGridPostion(worldPosition);
            SetGridObject(coords.x, coords.y, value);
        }


        /// <summary>
        /// Find object on grid coords x, z. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public TObject GetCellContent(int x, int z)
        {
            if (CheckInBounds(x, z))
            {
                return _gridArray[x, z];
            }
            else return default;
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
    }
}

