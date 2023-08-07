using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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

    #region Helper Functions

    //Method for Vector2
    public List<Vector2> FindPath(Vector2 origin, Vector2 destination, bool checkOccupied)
    {
        GridPosition gridOrigin = _levelGrid.GetGridPosition(origin);
        GridPosition gridDestination = _levelGrid.GetGridPosition(destination);

        if (!_levelGrid.IsOnGrid(gridDestination))
        {
            return new List<Vector2>(); 
        }
        
        TileGridObject tileGridObject = _levelGrid.GetTileGridObject(gridDestination);
        
        if (!tileGridObject.isWalkable)
        {
            return new List<Vector2>();
        }
        
        return FindPath(gridOrigin, gridDestination, checkOccupied);
    }

    public Vector2 GetRandomNeighbour(Vector2 origin, bool checkWalkable, bool checkOccupied, Vector2 target = new Vector2(), bool getFurthest = false)
    {
        TileGridObject startNode = _levelGrid.GetTileGridObject(origin);

        List<TileGridObject> neighbourList = new List<TileGridObject>();
        
        foreach (TileGridObject neighbour in _levelGrid.GetNeighbours(startNode.m_GridPosition, true))
        {
            if (checkWalkable && !neighbour.isWalkable)
            {
                continue;
            }
            if (checkOccupied && neighbour.isOccupied)
            {
                continue;
            }

            if (getFurthest)
            {
                Vector2 directionToGo = (neighbour.m_WorldPosition - origin).normalized;
                Vector2 directionToTarget = (target - origin).normalized;

                float dotProduct = Vector2.Dot(directionToGo, directionToTarget);
                if (dotProduct > 0)
                {
                    continue;
                }
            }
            
            neighbourList.Add(neighbour);
        }

        if (neighbourList.Count <= 0)
        {
            return origin;
        }
        
        return neighbourList[Random.Range(0, neighbourList.Count)].m_WorldPosition;
    }

    #endregion

    #region Breadth First Search

    public List<TileGridObject> FindPossiblePaths(List<TileGridObject> _nodeGrid, Vector2 _origin, float _maxDistance)
    {
        Dictionary<TileGridObject, int> visited = new Dictionary<TileGridObject, int>();
        Queue<TileGridObject> unvisited = new Queue<TileGridObject>();

        TileGridObject origin = _levelGrid.GetTileGridObject(_origin);

        visited[origin] = 0;
        unvisited.Enqueue(origin);
        int distance = 0;
        
        while (unvisited.Count > 0 && distance < _maxDistance)
        {
            TileGridObject current = unvisited.Dequeue();
            foreach (var neighbour in _levelGrid.GetNeighbours(current.m_GridPosition, true))
            {
                if (!visited.ContainsKey(neighbour) && _nodeGrid.Contains(neighbour))
                {
                    visited[neighbour] = 1 + visited[current];
                    unvisited.Enqueue(neighbour);
                }
            }
            distance = visited[current];
        }
        List<TileGridObject> validTiles = new List<TileGridObject>();
        foreach (var tile in visited)
        {
            validTiles.Add(tile.Key);
        }
        return validTiles;
    }
    #endregion
    
    #region AStar

    //A* Pathfinding
    public List<Vector2> FindPath(GridPosition origin, GridPosition destination, bool checkOccupied)
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

            List<TileGridObject> neighboursList = _levelGrid.GetNeighbours(currentNode.m_GridPosition, true);
            
            foreach (TileGridObject neighbour in neighboursList)
            {
                if (_closedList.Contains(neighbour) || !neighbour.isWalkable)
                {
                    continue;
                }
                
                if (checkOccupied)
                {
                    if (neighbour.isOccupied)
                    {
                        continue;
                    }
                }

                GridPosition neighbourPosition = neighbour.m_GridPosition;
                GridPosition currentPosition = currentNode.m_GridPosition;

                if (neighbourPosition.IsDiagonal(currentPosition))
                {
                    //Check if shared neighbours are walkable
                    TileGridObject sharedNeighbourA = neighboursList.Find(n => n.m_GridPosition == new GridPosition(currentPosition.x, neighbourPosition.y));
                    TileGridObject sharedNeighbourB = neighboursList.Find(n => n.m_GridPosition == new GridPosition(neighbourPosition.x, currentPosition.y));

                    if (!sharedNeighbourA.isWalkable || !sharedNeighbourB.isWalkable)
                    {
                        continue;
                    }
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

        //Debug.Log("Found No Path");
        
        return new List<Vector2>();
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

    #endregion

    #region BFS

    

    #endregion
    
}
