using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGridObject
{
    public GridPosition m_GridPosition { get; private set; }
    public Vector2 m_WorldPosition { get; private set; }
    
    //Unit data
    
    //Pathfinding
    public int m_Fcost;
    public int m_Gcost;
    public int m_Hcost;

    //Debugging
    public float GCOST;
    public float HCOST;
    
    public TileGridObject m_Parent;
    
    public TileGridObject(GridPosition gridPosition, Vector2 worldPosition)
    {
        m_GridPosition = gridPosition;
        m_WorldPosition = worldPosition;
    }
    
    public void CalculateFCost()
    {
        m_Fcost = m_Gcost + m_Hcost;
    }
    
    public override string ToString()
    {
        return m_GridPosition + "r\n" + " G: " + GCOST + " H: " + HCOST;
    }
}
