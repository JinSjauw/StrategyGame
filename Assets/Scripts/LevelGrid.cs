using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] private int _width, _height, _cellSize;
    [SerializeField] private Transform _debugObjectPrefab;
    [SerializeField] private Transform _tileVisualObjectPrefab;
    
    private GridSystem<TileGridObject> _levelGrid;
    private void Awake()
    {
        _levelGrid = new GridSystem<TileGridObject>(_width, _height, _cellSize, (GridPosition gridPosition, Vector3 worldPosition) => new TileGridObject(gridPosition, worldPosition));
    }

    // Start is called before the first frame update
    void Start()
    {
        _levelGrid.CreateCheckerBoard(_tileVisualObjectPrefab);
        _levelGrid.CreateDebugObjects(_debugObjectPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
