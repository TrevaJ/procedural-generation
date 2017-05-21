using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareGrid// calculate the positions of the square grids 
{
    public Square[,] squares;

    public SquareGrid(int[,] map, float squareSize)// a new grid created for use as the new generation
    {
        int nodeCountX = map.GetLength(0);//the x direction amount to create the grid horizonally
        int nodeCountY = map.GetLength(1);//the x direction amounnt to create the grid vertically
        float mapWidth = nodeCountX * squareSize;//the size the grid will be based on the size of each square
        float mapHeight = nodeCountY * squareSize;

        ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];// the 2d array version of the grids controle nodes

        for (int x = 0; x < nodeCountX; x++)
        {
            for (int y = 0; y < nodeCountY; y++)
            {
                Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, -mapHeight / 2 + y * squareSize + squareSize / 2,0 );//finding the center position of the square
                controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize); // crates the series of the control/nodes for each generated square
            }
        }

        squares = new Square[nodeCountX - 1, nodeCountY - 1];

        for (int x = 0; x < nodeCountX - 1; x++)//loop to check was position in  2d array squares
        {
            for (int y = 0; y < nodeCountY - 1; y++)
            {
                squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);// assings the instance of the control nodes in square (which inturn assings nodes)
            }//for each of the positions in the 2d array squares
        }
    }
}
