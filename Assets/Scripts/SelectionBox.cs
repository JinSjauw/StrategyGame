using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBox : MonoBehaviour
{
    private Mesh _mesh;
    private MeshFilter _renderer;
    private BoxCollider2D _collider;
    private List<Unit> _selectedUnits;

    private Vector2 _startPoint;
    private Vector2 _endPoint;

    [SerializeField] private LayerMask unitLayer;
    // Start is called before the first frame update
    private void Awake()
    {
        _renderer = GetComponent<MeshFilter>();
        _selectedUnits = new List<Unit>();
    }

    private void DetectUnits(Vector2 startPoint, Vector2 endPoint)
    {
        Vector2 center = startPoint + (endPoint - startPoint) / 2;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, new Vector2(Mathf.Abs(startPoint.x - endPoint.x), 
        Mathf.Abs(startPoint.y - endPoint.y)), 0, unitLayer);
        foreach (Collider2D collider in hits)
        {
            if (collider.TryGetComponent<Unit>(out Unit detectedUnit))
            {
                if (!_selectedUnits.Contains(detectedUnit))
                {
                    _selectedUnits.Add(detectedUnit);
                }
            }
        }
    }
    
    public List<Unit> GetSelection()
    {
        DetectUnits(_startPoint, _endPoint);
        return _selectedUnits;
    }
    
    public void DrawSelectionBox(Vector2 startPoint, Vector2 endPoint)
    {
        _startPoint = startPoint;
        _endPoint = endPoint;
        
        _mesh = new Mesh();
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector2> newUV = new List<Vector2>();
        
        newVertices.Add( new Vector3(startPoint.x, startPoint.y, 0));
        newVertices.Add( new Vector3(startPoint.x, endPoint.y, 0));
        newVertices.Add( new Vector3(endPoint.x, startPoint.y, 0));
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

        _mesh.vertices = newVertices.ToArray();
        _mesh.uv = newUV.ToArray();
        _mesh.triangles = newTriangles.ToArray();

        _renderer.mesh = _mesh;
    }
    
    public void Clear()
    {
        _renderer.mesh = null;
    }
}
