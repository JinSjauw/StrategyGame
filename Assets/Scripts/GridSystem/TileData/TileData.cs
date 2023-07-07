using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/TileData")]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;

    public bool walkable;
}
