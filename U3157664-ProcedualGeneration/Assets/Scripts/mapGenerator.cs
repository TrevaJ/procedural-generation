using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;// added for the random function
using System.Linq;
//following along with  https://unity3d.com/learn/tutorials/projects/procedural-cave-generation-tutorial/collisions-textures?playlist=17153
public class mapGenerator : MonoBehaviour {
    /*
    
        1. create the height and width of the grid that will be used as ints
        2. create the seed to be used (as a string) and the option of a random seed (as a bool)
        3. create a int to give the option for how much to randomly fill and set a range for it from 0 to 100
        4. create the map 2D array
        5. in a seperate void for the map generator assign the map to equal the width and heights
        6.for each positon in the map 2d array compare randomized numbers to determine if the positon is 1 or 0 (two for loops)
        7. check the array psiton is around the edge of the grid, if so then make it a wall (1) 
        8. check in a 3X3 grid around each postion and if its not on the edge then add the centeral point onto the new grids postion value 
        9. smooth the map out useing the new values  and apply a rule (see the amoothmap and neighbour tile mehtods)

    */
    public int width;
    public int height;
    public GameObject player;

    public string seed;//the seed used to generate pattern
    public bool useRandomSeed;//used if user wants to have no control over the seed
    [Range(30,50)]
    public int randomFillPercent;//how much to randomy fill (will aslo give indication as to the desity of finihsed map) 30-50 gives a good range without going too litte or too much
     
    int[,] map; //map coordinates 

    public int smoothValue = 3;
    void Start()
    {
        GenerateMap();
        
    }
     void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        map = new int[width, height];
        randomFillMap();
        for (int i = 0; i < smoothValue; i++)// how many times to run over the smooth method
        {
            SmoothMap();
        }

        ProcessMap();

 #region change the size of the border around the edge of the map
        int borderSize = 5;
        int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)//making sure that the 
                {
                    borderedMap[x, y] = map[x-borderSize,y-borderSize];
                }
                else
                {
                    borderedMap[x, y] = 1;
                }
            }
        }
#endregion
        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(borderedMap, 1);

        
        
    }

    void ProcessMap() //control over the size of wall and room space
    {
        
        List<List<TileCoordinate>> wallRegions = GetRegions(1);

        int wallThreshHoldSize = 50; //any wall region taht is made up of less that 50 tiles will be removed

        foreach (List<TileCoordinate> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThreshHoldSize)// if the total size of patch of walls is less than  provided threshhold then all the wall tiles involved will be set to 0 (floor)
            {
                foreach (TileCoordinate tile in wallRegion)
                {
                    map[tile.tileX, tile.tileY] = 0;

                }
            }
        }
        List<List<TileCoordinate>> roomRegions = GetRegions(0);

        int roomThreshHoldSize = 60; //any room region taht is made up of less that set amount of tiles will be removed
        List<Room> survivingRooms = new List<Room>();

        foreach (List<TileCoordinate> roomRegion in roomRegions)// if the totalSize of the room is less than the thresholdSize then each of the tiles included are set to 1 (wall)
        {
            if (roomRegion.Count < roomThreshHoldSize)
            {
                foreach (TileCoordinate tile in roomRegion)
                {
                    map[tile.tileX, tile.tileY] = 1;

                }
            }
            else //if the room count is greater than threshold then add its coord List to the surviveingrooms
            {
                survivingRooms.Add(new Room(roomRegion, map));
            }
        }
        survivingRooms.Sort(); //sorts ehe roomsizes into order
        survivingRooms[0].isMainRoom = true;// largest room is set to mainroom  
        survivingRooms[0].isAccessibleFromMainRoom = true;//largest room is accessible from mainroom

        player.transform.position = CoordToWorldPoint(survivingRooms[0].tiles[survivingRooms[0].tiles.Count()/2]);// moves the player when the scene spawns to the middle tile of largest room( this may not be in the middle of the room due to 


        ConnectClosestRooms(survivingRooms);
    }

    void ConnectClosestRooms (List<Room> allRooms,bool forceAccessibilityFromMainRoom = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in allRooms)
            {
                if (room.isAccessibleFromMainRoom) roomListB.Add(room);
                else roomListA.Add(room);
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;

        }

        int bestDistance = 0;
        TileCoordinate bestTileA = new TileCoordinate();
        TileCoordinate bestTileB = new TileCoordinate();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;


        foreach (Room roomA in roomListA)
        {
            if(!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if(roomA.connectedRooms.Count>0)
                {
                    continue;
                }
            }


            foreach (Room roomB in roomListB)
            {
                if (roomA==roomB||roomA.IsConected(roomB))//if the room is checking itself or is already connected to the room its checking then skip to the next loop
                {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        TileCoordinate tileA = roomA.edgeTiles[tileIndexA];
                        TileCoordinate tileB = roomB.edgeTiles[tileIndexB];

                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));// checking the distance between the two rooms (formula provided by tutorial)

                        if (distanceBetweenRooms<bestDistance || !possibleConnectionFound)  // if the distance found is less than the current best distance 
                                                                                            //and a possible connection has not been found then a new best result has been identified
                                                                                            //and a new set of results need to be applied
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if (possibleConnectionFound&&!forceAccessibilityFromMainRoom)//if the room has  a possible connection and isnt set to be forced to connect then crate the best passage
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound&&forceAccessibilityFromMainRoom)// if a possible connection has been found and is set to force accessibilty thn craete teh bassage and rerun the method
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        if (!forceAccessibilityFromMainRoom) ConnectClosestRooms(allRooms, true);
    }

    void CreatePassage(Room roomA, Room roomB, TileCoordinate tileA, TileCoordinate tileB)//used to draw a debug line allowing for a early visual represenation of the connections being made
    {
        Room.ConnectRooms(roomA, roomB);
        //Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 2);
        int passageSize = 5;
        List<TileCoordinate> line = GetLine(tileA, tileB);// the tiles that will be the start points between the connection
        foreach (TileCoordinate c in line)
        {
            for (int x = -passageSize; x <= passageSize + 1; x++)
            {
                for (int y = -passageSize; y <= passageSize + 1; y++)
                {
                    if (x * x + y * y <= passageSize * passageSize)// if the squared values of both x and y is less than the squared value of r then
                    {
                        int passageTilesX = c.tileX + x;
                        int passageTilesY = c.tileY + y;
                        if (IsInMapRange(passageTilesX, passageTilesY))// check if the tiles are in the grid
                        {
                            map[passageTilesX, passageTilesY] = 0;
                        }
                    }
                }
            }
        }
    }



#region tutorial Formula
    List<TileCoordinate> GetLine(TileCoordinate from, TileCoordinate to)// the following is based completely off the mathimatical formula demonstrated and provided during the tuorial at the start of the video linked below
                                             //https://unity3d.com/learn/tutorials/projects/procedural-cave-generation-tutorial/passageways?playlist=17153

    {
        List<TileCoordinate> line = new List<TileCoordinate>();// the line that the connection will follow

        int startingX = from.tileX;//starting tile x value
        int startingY = from.tileY;//starting tile y value

        int dx = to.tileX - from.tileX;//delta x value
        int dy = to.tileY - from.tileY;//delta y value

        bool inverted = false;
        int stepX = Math.Sign(dx);
        int stepY = Math.Sign(dy);

        int longest = Mathf.Abs(dx);//the step values for x and y
        int shortest = Mathf.Abs(dy);

        if(longest<shortest)// if longest is the lower value then swap around
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            stepX = Math.Sign(dy);
            stepY = Math.Sign(dx);
        }
        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new TileCoordinate(startingX, startingY));
            if (inverted) startingY += stepX;
            else startingX += stepX;

            gradientAccumulation += shortest;
            if(gradientAccumulation >= longest)
            {
                if (inverted) startingX += stepY;
                else startingY += stepY;
                gradientAccumulation -= longest;
            }
        }
        return line;
    }
    #endregion

    Vector3 CoordToWorldPoint(TileCoordinate tile)// change the coordinate to a vector 3 location
    {
        return new Vector3(-width / 2 + .5f + tile.tileX,  -height / 2 + .5f + tile.tileY);
    }

    bool IsInMapRange(int x, int y)// method to determine if what is being checked is within the bounds of the generated map
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    List<List<TileCoordinate>> GetRegions(int tiletype)
    {
        List<List<TileCoordinate>> regions = new List<List<TileCoordinate>>();
        int[,] mapChecked = new int[width, height];//the 2d array map of points that have been checked arleady (0= unchecked 1= checked)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapChecked[x,y]==0 && map[x,y] == tiletype)
                {
                    List<TileCoordinate> newRegion = GetRegionTiles(x, y);//sets the list of Coord accoding to the GetRegionTiles methods
                    regions.Add(newRegion);

                    foreach (TileCoordinate tile in newRegion)
                    {
                        mapChecked[tile.tileX, tile.tileY] = 1; //has been checked
                    }
                }
            }
        }
        return regions;
    }


    List<TileCoordinate> GetRegionTiles(int startX, int startY)
    {
        List<TileCoordinate> tiles = new List<TileCoordinate>();
        int[,] mapChecked = new int[width, height];//the 2d array map of points that have been checked arleady (0= unchecked 1= checked)
        int tileType = map[startX, startY];// sets the tiletype being checked as the same type that is registered in the map 2D array

        Queue<TileCoordinate> queue = new Queue<TileCoordinate>();
        queue.Enqueue(new TileCoordinate(startX, startY));// adds the current tile that is being checked to the Queue
        mapChecked[startX, startY] = 1;//assigns the map coordinate to the mapFlags as being checked (0== unchecked  1== checked)
        while(queue.Count>0)
        {
            TileCoordinate tile = queue.Dequeue();// removes the first item in the queue and assigns it to the Coord tile
            tiles.Add(tile);//assigns the removed tile Coord to the tiles list

            for (int x = tile.tileX-1; x <= tile.tileX+1; x++) // cycle through the tiles before and after the current tile on x axis (centre tile has already been checked off)
            {
                for (int y = tile.tileY-1; y <= tile.tileY+1; y++) // cycle through the tiles below and above the current tile on y axis (centre tile has already been checked off)
                {
                    if (IsInMapRange(x,y)&&(y== tile.tileY ||x== tile.tileX))// (is the tile in the bounds of the generated map) and (the tile being checked aligns with the x or y axis)(check the ones above,below, either side of the centre tile)
                    {
                        if (mapChecked [x,y]== 0&& map[x,y]== tileType) // check if the tile has already been checked and that it is infact the same tile that is being checked
                        {
                            mapChecked[x, y] = 1;// confirms that the tile has been checked so is does not have to be rechecked 
                            queue.Enqueue(new TileCoordinate(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }




    void randomFillMap()//set the random fill that will be applied
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();// sets the seed to something that will change everytime
        }
        System.Random pseudorandom = new System.Random(seed.GetHashCode());// used the hashcode version of the seed to provide a unique rnadom
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x==0||x == width - 1||y== 0||y== height-1)// check if the spot is around the edge of the map to make it a wall
                {
                    map[x, y] = 1;
                }
                else map[x, y] = (pseudorandom.Next(0, 100) < randomFillPercent) ? 1 : 0; //if the random number generated is less than the provided randomFillAmount then tile is 1 else its 0 
            }
        }
    }

    void SmoothMap()//allows for implamentation of rules set to allow for difference in generation
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbouringwallTiles = GetSurroundingWallCount(x,y);//changing these rules will result in a change n the shape of the environment
                if (neighbouringwallTiles>4)
                {
                    map[x, y] = 1;
                }
                else if (neighbouringwallTiles<4)
                {
                    map[x, y] = 0;
                }
            }
        }
    }
    int GetSurroundingWallCount(int gridX, int gridY)//uses a 3x3 grid but can be expanded to a larger one it needed
    {
        int wallcount = 0;
        for (int neighbourX = gridX-1 ; neighbourX <= gridX +1; neighbourX++)// loops through a 3x3 grid centered on gridY and y
        {
            for (int neighbourY = gridY-1 ; neighbourY <= gridY+1; neighbourY++)
            {
                if (IsInMapRange(neighbourX,neighbourY))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallcount += map[neighbourX, neighbourY];// if the positon in the 3x3 grid is not the centeral location (the grid postion its currently working on) then wall count equals itselft plus the centreal point (1 and 0)
                    }
                }
                else wallcount++;
            }
        }
        return wallcount;
    }

}
