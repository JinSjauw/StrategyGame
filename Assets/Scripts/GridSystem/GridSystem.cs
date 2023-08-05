using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    public bool IsDiagonal(GridPosition other)
    {
        if (x != other.x && y != other.y)
        {
            return true;
        }

        return false;
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
    private int _cellSize;
    
    private TGridObject[,] _gridObjectArray;

    private List<GridPosition> _debugObjects;

    public GridSystem(int width, int height, int cellSize, Func<GridPosition, Vector3, TGridObject> CreateGridObject)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        
        _gridObjectArray = new TGridObject[width, height];
        _debugObjects = new List<GridPosition>();
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
    
    public GridSystem(Tilemap tilemap, Func<GridPosition, Vector3, TGridObject> CreateGridObject)
    {
        Vector3Int mapSize = tilemap.size;
        _width = mapSize.x;
        _height = mapSize.y;
        _cellSize = mapSize.z;
        
        _gridObjectArray = new TGridObject[_width, _height];
        _debugObjects = new List<GridPosition>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
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
    public List<TileGridObject> GetTileGridList()
    {
        TGridObject[,] gridArray = new TGridObject[_width, _height];
        Array.Copy(_gridObjectArray, gridArray, _gridObjectArray.Length);

        return gridArray.Cast<TileGridObject>().ToList();
    }
    public List<TileGridObject> GetTileGridNeighbours(GridPosition gridPosition, bool allowDiagonal)
    {
        List<TileGridObject> neighbours = new List<TileGridObject>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                if (!allowDiagonal)
                {
                    if (Mathf.Abs(x) + Mathf.Abs(y) > 1)
                    {
                        continue;
                    }
                }
                
                GridPosition neighbourGridPosition = new GridPosition(gridPosition.x + x, gridPosition.y + y);
                
                if (IsOnGrid(neighbourGridPosition))
                {
                    neighbours.Add(GetTileGridObject(neighbourGridPosition));
                }
            }
        }
        return neighbours;
    }
    public bool IsOnGrid(GridPosition gridPosition)
    {
        if (gridPosition.x >= 0 && gridPosition.y >= 0 && gridPosition.x < _width && gridPosition.y < _height)
        {
            return true;
        }
        return false;
    }
    public TileGridObject GetTileGridObject(GridPosition gridPosition)
    {
        if (gridPosition.x >= 0 && gridPosition.x <= _width &&
            gridPosition.y >= 0 && gridPosition.y <= _height)
        {
            return _gridObjectArray[gridPosition.x, gridPosition.y] as TileGridObject;
        }
        return null;
    }
    public bool insideCircle(Vector2 _center, Vector2 _tile, float _radius)
    {
        float distanceX = _center.x - _tile.x;
        float distanceZ = _center.y - _tile.y;

        float distance = Mathf.Sqrt(distanceX * distanceX + distanceZ * distanceZ);
        return distance <= _radius;
    }
    public List<TileGridObject> GetTilesInCircle(Vector2 _center, float radius)
    {
        radius += 0.5f;
        
        int top = (int)Mathf.Ceil(_center.y - radius);
        int bottom = (int)Mathf.Floor(_center.y + radius);
        int left = (int)Mathf.Ceil(_center.x - radius);
        int right  = (int)Mathf.Floor(_center.x + radius);

        List<TileGridObject> validTiles = new List<TileGridObject>();

        for (int z = top; z <= bottom; z++)
        {
            for (int x = left; x <= right; x++)
            {
                GridPosition tile = new GridPosition(x / _cellSize, z / _cellSize);
                if (insideCircle(_center, new Vector2(tile.x * _cellSize, tile.y * _cellSize), radius))
                {
                    if (IsOnGrid(tile))
                    {
                        validTiles.Add(GetTileGridObject(tile));
                    }
                }
            }
        }
        return validTiles;
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
    public void CreateDebugObjects(Transform _debugPrefab, TileGridObject tileGridObject)
    {
        GridPosition gridPosition = tileGridObject.m_GridPosition;

        if (!_debugObjects.Contains(gridPosition))
        {
            Transform debugTransform = GameObject.Instantiate(_debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
            GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
            gridDebugObject.SetGridObject(GetTileGridObject(gridPosition));
            
            _debugObjects.Add(gridPosition);
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
