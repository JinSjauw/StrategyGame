using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int y;

    public GridPosition(int X, int Y)
    {
        x = X;
        y = Y;
    }
   
    public override string ToString()
    {
        return "x: " + x + " Y: " + y;
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x == b.x && a.y == b.y;
    }
   
    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
               x == position.x &&
               y == position.y;
    }

    public float Distance(GridPosition other)
    {
        Vector2 a = new Vector2(x, y);
        Vector2 b = new Vector2(other.x, other.y);

        return Vector2.Distance(a, b);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }
}

public class GridSystem<TGridObject>
{
    private int _width;
    private int _height;
    private float _cellSize;
    
    private TGridObject[,] _gridObjectArray;
   
    public GridSystem(int width, int height, float cellSize, Func<GridPosition, Vector3, TGridObject> CreateGridObject)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        
        _gridObjectArray = new TGridObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                Vector3 worldPosition = GetWorldPosition(gridPosition);
                _gridObjectArray[x, y] = CreateGridObject(gridPosition, worldPosition);
            }
        }
    }
    
    public Vector2 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector2(gridPosition.x, gridPosition.y) * _cellSize;
    }
    
    public GridPosition GetGridPosition(Vector2 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / _cellSize),
            Mathf.RoundToInt(worldPosition.y / _cellSize)
        );
    }
    
    public TileGridObject GetTileGridObject(GridPosition gridPosition)
    {
        return _gridObjectArray[gridPosition.x, gridPosition.y] as TileGridObject;
    }

    public void CreateDebugObjects(Transform _debugPrefab)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform debugTransform =
                    GameObject.Instantiate(_debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetTileGridObject(gridPosition));
            }
        }
    }
    
    public void CreateCheckerBoard(Transform _tilePrefab)
    {
        Color oddColor = new Color(.70f, .70f, .75f);
        Color evenColor = new Color(.85f, .85f, .90f);
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {

                GridPosition gridPosition = new GridPosition(x, y);
                Transform tileTransform =
                    GameObject.Instantiate(_tilePrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                SpriteRenderer tileVisual = tileTransform.GetComponent<SpriteRenderer>();

                if (x % 2 == 0 && y % 2 != 0 || x % 2 != 0 && y % 2 == 0)
                {
                    tileVisual.color = oddColor;
                }
                else
                {
                    tileVisual.color = evenColor;
                }

                //tileVisual.color = new Color (0, 0, 0);
            }
        }
    }
}
