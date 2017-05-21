using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


class Room : IComparable<Room>   //class holder for the room to identity tiles(edge and filled tiles) if the room is accassible from the mian room or if it is the main room
                                 //which rooms are connected to the room
                                 //IComparable allows this class to be sorted 
{
    public List<TileCoordinate> tiles;
    public List<TileCoordinate> edgeTiles;
    public List<Room> connectedRooms;
    public int roomSize;

    public bool isAccessibleFromMainRoom;
    public bool isMainRoom;

    public Room()
    {

    }

    public Room(List<TileCoordinate> roomtiles, int[,] map)
    {
        tiles = roomtiles;
        roomSize = tiles.Count;
        connectedRooms = new List<Room>();

        edgeTiles = new List<TileCoordinate>();
        foreach (TileCoordinate tile in tiles)
        {
            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)//cycle through the tiles above,below,and the right and left of current tile being checked
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (x == tile.tileX || y == tile.tileY)
                    {
                        if (map[x, y] == 1)
                        {
                            edgeTiles.Add(tile);//if the tile being checked is a wall then its listed as a edgetile
                        }
                    }
                }
            }
        }
    }
    public void SetAccessibleFromMainRoom()
    {
        if (!isAccessibleFromMainRoom)
        {
            isAccessibleFromMainRoom = true;
            foreach (Room connectedRoom in connectedRooms)
            {
                connectedRoom.SetAccessibleFromMainRoom();
            }
        }
    }

    public static void ConnectRooms(Room roomA, Room roomB)
    {
        if (roomA.isAccessibleFromMainRoom) roomB.SetAccessibleFromMainRoom();//if roomA is connected to main then roomB can access the main room
        else if (roomB.isAccessibleFromMainRoom) roomA.SetAccessibleFromMainRoom();//else if roomA cant and rooB can if room B can access the main room then room A can access the mainroom

        roomA.connectedRooms.Add(roomB);//list the roomA as connected to roomB
        roomB.connectedRooms.Add(roomA);//list the roomB as connected to roomA
    }
    public bool IsConected(Room otherRoom)
    {
        return connectedRooms.Contains(otherRoom);//is it connected to other rooms
    }
    public int CompareTo(Room otherRoom)// allows the IComparable inheritance to be applied to room class and be ordered based on roomSize int
    {
        return otherRoom.roomSize.CompareTo(roomSize);
    }
}
