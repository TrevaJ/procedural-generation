using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TileCoordinate // struct to contain the cordinates of the tiles being checked
{
    public int tileX;
    public int tileY;

    public TileCoordinate(int x, int y)
    {
        tileX = x;
        tileY = y;
    }

}