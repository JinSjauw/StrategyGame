using System.Collections;
using System.Collections.Generic;
using UnitSystem;
using UnityEngine;

[System.Serializable]
public class TileGridObject
{
    public GridPosition m_GridPosition { get; private set; }
    public Vector2 m_WorldPosition { get; private set; }
    public Transform m_TileHighlight;

    
    //Unit data
    private Unit _unit;
    private bool _isOcuppied;
    private bool _isWalkable;
    public Unit unit { get => _unit; }
    public bool isOccupied { get => _isOcuppied; }
    public bool isWalkable { get; set; }

    //Pathfinding
    public int m_Fcost;
    public int m_Gcost;
    public int m_Hcost;

    public TileGridObject m_Parent;
    
    public TileGridObject(GridPosition gridPosition, Vector2 worldPosition)
    {
        m_GridPosition = gridPosition;
        m_WorldPosition = worldPosition;
    }

    public void ClearTile()
    {
        _isOcuppied = false;
        _unit = null;
    }

    public void OccupyTile(Unit unit)
    {
        _isOcuppied = true;
        _unit = unit;
    }
    
    public void CalculateFCost()
    {
        m_Fcost = m_Gcost + m_Hcost;
    }
    
    public override string ToString()
    {
        return m_GridPosition + "r\n" + "Occupied :" + isOccupied;
    }
}
