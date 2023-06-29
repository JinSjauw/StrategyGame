using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGridObject
{
    public GridPosition m_GridPosition { get; private set; }
    public Vector3 m_WorldPosition { get; private set; }
    
    //Unit data


    public TileGridObject(GridPosition gridPosition, Vector3 worldPosition)
    {
        m_GridPosition = gridPosition;
        m_WorldPosition = worldPosition;
    }

    public override string ToString()
    {
        return m_GridPosition.ToString();
    }
}
