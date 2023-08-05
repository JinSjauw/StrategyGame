using System.Collections;
using System.Collections.Generic;
using UnitSystem;
using UnityEngine;

[System.Serializable]
public class TileGridObject
{
    public GridPosition m_GridPosition { get; private set; }
    public Vector2 m_WorldPosition { get; private set; }
    public TileVisual m_TileVisual;

    //Unit data
    private Unit _unit;
    private bool _isOcuppied;
    private bool _isWalkable;
    private bool _isReserved;
    
    public Unit unit { get => _unit; }
    public bool isOccupied { get => _isOcuppied; set => _isOcuppied = value; }
    public bool isWalkable { get => _isWalkable; set => _isWalkable = value; }
    public bool isReserved { get => _isReserved; }

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

    private void UnitRendererState(bool state)
    {
        if (_unit != null)
        {
            _unit.unitRenderer.enabled = state;
            _unit.weaponRenderer.enabled = state;
        }
    }
    
    public void ClearTile()
    {
        _isOcuppied = false;
        _isReserved = false;
        _unit = null;
    }

    public void OccupyTile(Unit unit)
    {
        _isOcuppied = true;
        _unit = unit;
        UnitRendererState(!m_TileVisual.fogState);
    }

    public void ClearFog()
    {
        m_TileVisual.FogOff();
        UnitRendererState(true);
    }

    public void FogOn()
    {
        m_TileVisual.FogOn();
        UnitRendererState(false);
    }
    
    public void ReserveTile(Unit unit)
    {
        _isReserved = true;
        _unit = unit;
        UnitRendererState(!m_TileVisual.fogState);
    }
    
    public void CalculateFCost()
    {
        m_Fcost = m_Gcost + m_Hcost;
    }
    
    public override string ToString()
    {
        return m_GridPosition + "\n" + "Occupied :" + isOccupied + "\n" + "isWalkable: " + isWalkable;
    }
}
