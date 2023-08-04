using System.Collections.Generic;
using UnitSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] private int _width, _height, _cellSize;
    [SerializeField] private Transform _debugObjectPrefab;
    [SerializeField] private Transform _tileVisualPrefab;
    [SerializeField] private Transform _tileVisual;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private List<TileData> tileTypes;

    private GridSystem<TileGridObject> _gridSystem;
    private Dictionary<TileBase, TileData> _tileData;

    private void Awake()
    {
        Vector3Int mapSize = _tilemap.size;
        Debug.Log(mapSize);
        _tileData = new Dictionary<TileBase, TileData>();
        foreach (TileData tileData in tileTypes)
        {
            foreach (TileBase tile in tileData.tiles)
            {
                _tileData.Add(tile, tileData);
            }
        }
        _gridSystem = new GridSystem<TileGridObject>(mapSize.x, mapSize.y, mapSize.z,
            (GridPosition gridPosition, Vector3 worldPosition) => CreateTileGridObject(gridPosition, worldPosition));
    }

    // Start is called before the first frame update
    void Start()
    {
        //_gridSystem.CreateCheckerBoard(_tileVisualPrefab);
        //_gridSystem.CreateDebugObjects(_debugObjectPrefab);
    }

    private TileGridObject CreateTileGridObject(GridPosition gridPosition, Vector2 worldPosition)
    {
        TileGridObject tileGridObject = new TileGridObject(gridPosition, worldPosition);
        Vector3Int cellPosition = _tilemap.WorldToCell(worldPosition);
        TileBase tile = _tilemap.GetTile(cellPosition);
        tileGridObject.m_TileVisual = Instantiate(_tileVisual).GetComponent<TileVisual>();
        tileGridObject.m_TileVisual.transform.position = worldPosition;
        
        if (tile == null)
        {
            tileGridObject.isWalkable = false;
            return tileGridObject;
        }

       
        tileGridObject.isWalkable = _tileData[tile].walkable;
        
        return tileGridObject;
    }
    
    private void UpdateTileGridObjectState(GridPosition fromGridPosition, GridPosition toGridPosition,  Unit unit)
    {
        TileGridObject fromTile = GetTileGridObject(fromGridPosition);
        fromTile.ClearTile();
        
        TileGridObject toTile = GetTileGridObject(toGridPosition);
        toTile.OccupyTile(unit);
    }
    public void Unit_OnUnitMoved(object sender, UnitMovedEventArgs e)
    {
        UpdateTileGridObjectState(GetGridPosition(e.originPosition), GetGridPosition(e.targetPosition), e.unit);
    }
    public Vector2 GetWorldPositionOnGrid(Vector2 worldPosition)
    {
        return GetWorldPosition(GetGridPosition(worldPosition));
    }
    public TileGridObject GetTileGridObject(Vector2 worldPosition)
    {
        return _gridSystem.GetTileGridObject(_gridSystem.GetGridPosition(worldPosition));
    }

    public bool isTileWalkable(Vector2 worldPosition)
    {
        return _gridSystem.GetTileGridObject(_gridSystem.GetGridPosition(worldPosition)).isWalkable;
    }

    public int GetCellSize() { return _cellSize; }
    public void CreateDebugObjects(TileGridObject tileGridObject) => _gridSystem.CreateDebugObjects(_debugObjectPrefab, tileGridObject);
    public GridPosition GetGridPosition(Vector2 worldPosition) => _gridSystem.GetGridPosition(worldPosition);

    public bool InsideCircle(Vector2 center, Vector2 tile, float radius) =>
        _gridSystem.insideCircle(center, tile, radius);
    public List<TileGridObject> GetTilesInCircle(Vector2 center, float radius) => _gridSystem.GetTilesInCircle(center, radius);
    public Vector2 GetWorldPosition(GridPosition gridPosition) => _gridSystem.GetWorldPosition(gridPosition);
    public TileGridObject GetTileGridObject(GridPosition gridPosition) => _gridSystem.GetTileGridObject(gridPosition);
    public List<TileGridObject> GetNeighbours(GridPosition gridPosition, bool allowDiagonal) => _gridSystem.GetTileGridNeighbours(gridPosition, allowDiagonal);
    public List<TileGridObject> GetTileGridList() => _gridSystem.GetTileGridList();
    public bool IsOnGrid(GridPosition gridPosition) => _gridSystem.IsOnGrid(gridPosition);
    //public List<GridPosition> GetRadialGrid(GridPosition gridPosition, int xLength, int yLength) => _gridSystem.GetRadialGrid(gridPosition, xLength, yLength);
    
}
