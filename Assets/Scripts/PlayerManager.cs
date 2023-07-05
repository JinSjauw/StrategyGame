using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Unit> _playerUnits;
    [SerializeField] private LevelGrid _levelGrid;

    //Selection Box
    [SerializeField] private MeshFilter selectionBox;
    /*private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();*/
    private Mesh mesh;
    
    private bool _isDragging;
    private Vector2 _startPoint;
    private Vector2 _endPoint;
    
    //Selection Box End
    
    private Pathfinding _pathfinding;
    private List<Vector2> _path;
    private Unit _currentUnit;
    
    private void Start()
    {
        _pathfinding = new Pathfinding(_levelGrid);
        _currentUnit = _playerUnits[0];
        
        //On Awaking Subscribe levelGrid to all unitMoved events;
        foreach (Unit unit in _playerUnits)
        {
            unit.OnUnitMove += _levelGrid.Unit_OnUnitMoved;
        }
    }

    private void Update()
    {
        if (_isDragging)
        {
            DrawBoxSelection();
        }
    }
    
    //Be able to make an selection of the units.
    //If selectedUnits are more than 1 have the rest select an adjacent tile next to currentUnit;
    //Is to make moving in overworld more handy.

    private void DrawBoxSelection()
    {
        Vector2 endPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        mesh = new Mesh();
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector2> newUV = new List<Vector2>();
        
        newVertices.Add( new Vector3(_startPoint.x, _startPoint.y, 0));
        newVertices.Add( new Vector3(_startPoint.x, endPoint.y, 0));
        newVertices.Add( new Vector3(endPoint.x, _startPoint.y, 0));
        newVertices.Add( new Vector3(endPoint.x, endPoint.y, 0));
        
        newTriangles.Add(0);
        newTriangles.Add(1);
        newTriangles.Add(3);
        
        newTriangles.Add(0);
        newTriangles.Add(2);
        newTriangles.Add(3);
        
        newUV.Add( new Vector2( 0, 0));
        newUV.Add( new Vector2( 0, 1));
        newUV.Add( new Vector2( 1, 1));
        newUV.Add( new Vector2( 1, 0));

        mesh.vertices = newVertices.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.triangles = newTriangles.ToArray();

        selectionBox.mesh = mesh;
    }
    
    //Save startpoint
    //Check for difference between current mouseposition and startpoint
    public void OnBoxSelection(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isDragging = true;
            _startPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

        if (context.canceled)
        {
            _isDragging = false;
            selectionBox.mesh = null;
        }
    }
    
    
    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RaycastHit2D hit =
                Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));

            if (hit.collider)
            {
                Debug.Log(hit.collider.name);

                if (hit.collider.TryGetComponent<Unit>(out Unit selectedUnit))
                {
                    _currentUnit = selectedUnit;
                }
                
                return;
            }

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            GridPosition clickGridPosition = _levelGrid.GetGridPosition(mousePosition);
            
            if (_levelGrid.IsOnGrid(clickGridPosition) && !_levelGrid.GetTileGridObject(clickGridPosition).isOccupied)
            {
                Debug.Log(_currentUnit.transform.position);
                _path = _pathfinding.FindPath(_levelGrid.GetGridPosition(_currentUnit.transform.position), clickGridPosition);
                if (!_currentUnit.IsActive)
                {
                    _currentUnit.Move(_path);
                }
            }
        }
    }

    public Unit GetCurrentUnit()
    {
        return _currentUnit;
    }
}
