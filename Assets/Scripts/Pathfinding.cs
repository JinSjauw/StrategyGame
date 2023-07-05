using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Pathfinding
{
    private LevelGrid _levelGrid;
    
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    
    private List<TileGridObject> _nodeGrid;
    private List<TileGridObject> _openList;
    private List<TileGridObject> _closedList;
    
    public Pathfinding(LevelGrid levelGrid)
    {
        _levelGrid = levelGrid;
        _nodeGrid = levelGrid.GetTileGridList();
    }
    
    //A* Pathfinding
    public List<Vector2> FindPath(GridPosition origin, GridPosition destination)
    {
        TileGridObject startNode = _levelGrid.GetTileGridObject(origin);
        TileGridObject endNode = _levelGrid.GetTileGridObject(destination);

        _openList = new List<TileGridObject> { startNode };
        _closedList = new List<TileGridObject>();

        foreach (TileGridObject node in _nodeGrid)
        {
            node.m_Gcost = int.MaxValue;
            node.CalculateFCost();
            node.m_Parent = null;
        }

        startNode.m_Gcost = 0;
        startNode.m_Hcost = CalculateDistance(startNode, endNode);
        startNode.CalculateFCost();

        while (_openList.Count > 0)
        {
            TileGridObject currentNode = GetLowestFcost(_openList);

            if (currentNode.m_GridPosition == endNode.m_GridPosition)
            {
                //Return path
                return ReturnPath(startNode, endNode);
            }

            _openList.Remove(currentNode);
            _closedList.Add(currentNode);

            List<TileGridObject> neighboursList = _levelGrid.GetNeighbours(currentNode.m_GridPosition);

            foreach (TileGridObject neighbour in neighboursList)
            {
                if (_closedList.Contains(neighbour) || neighbour.isOccupied)
                {
                    continue;
                }
                
                int tentativeGcost = currentNode.m_Gcost + CalculateDistance(currentNode, neighbour);
                if (tentativeGcost < neighbour.m_Gcost)
                {
                    neighbour.m_Parent = currentNode;
                    neighbour.m_Gcost = tentativeGcost;
                    neighbour.m_Hcost = CalculateDistance(neighbour, endNode);
                    neighbour.CalculateFCost();

                    if (!_openList.Contains(neighbour))
                    {
                        _openList.Add(neighbour);
                    }
                }
            }
        }

        Debug.Log("Found No Path");
        
        return null;
    }

    private List<Vector2> ReturnPath(TileGridObject startNode, TileGridObject endNode)
    {
        List<Vector2> path = new List<Vector2>();
        TileGridObject currentNode = endNode;
        path.Add(currentNode.m_WorldPosition);
        while (currentNode != startNode)
        {
            path.Add(currentNode.m_Parent.m_WorldPosition);
            currentNode = currentNode.m_Parent;
        }
        path.Reverse();
        return path;
    }

    private TileGridObject GetLowestFcost(List<TileGridObject> openList)
    {
        TileGridObject lowestFcost = openList[0];
        foreach (TileGridObject node in openList)
        {
            if (node.m_Fcost < lowestFcost.m_Fcost)
            {
                lowestFcost = node;
            }
        }
        return lowestFcost;
    }

    private int CalculateDistance(TileGridObject a, TileGridObject b)
    {
        if(a == null || b == null)
        {
            Debug.Log("Pathnode missing");
            return 0;
        }

        int distanceX = (int)MathF.Abs(a.m_GridPosition.x - b.m_GridPosition.x);
        int distanceY = (int)MathF.Abs(a.m_GridPosition.y - b.m_GridPosition.y);
        int remaining = Mathf.Abs(distanceX - distanceY);
        int score = MOVE_DIAGONAL_COST * Mathf.Min(distanceX, distanceY) + MOVE_STRAIGHT_COST * remaining;
        return score;
    }
}
