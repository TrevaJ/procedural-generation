using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    /*
    follow from bottom up to see the needed steps 
    */
    public SquareGrid squareGrid;
    //public MeshFilter walls;
    public MeshFilter cave;

    List<Vector3> vertices;
    List<int> triangles;

    Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
    List<List<int>> outlines = new List<List<int>>();
    HashSet<int> checkedVertices = new HashSet<int>();

    public void GenerateMesh(int[,] map, float squareSize) // used to create base mesh generated
    {

        triangleDictionary.Clear();//clear the licss and dictioanries each time that a new mesh is generated so that it all resets as needed
        outlines.Clear();
        checkedVertices.Clear();

        squareGrid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        cave.mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        int tileAmount = 10;

        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            float percentX = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, vertices[i].x)*tileAmount;
            float percentY = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, vertices[i].y)*tileAmount;
            uvs[i] = new Vector2(percentX, percentY);
        }
        mesh.uv = uvs;
            Generate2DColliders();
        
        
    }


    void Generate2DColliders()// generating the 2d colliders used for 2d play
    {
        EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D>();// destroy any currently existing colliders for new generation
        for (int i = 0; i < currentColliders.Length; i++)
        {
            Destroy(currentColliders[i]);
        }

        CalculateMeshOutlines();// recalculate the generated mesh (from top view)
        foreach (List<int> outline in outlines)
        {
            EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();// add the collider to the game object
            Vector2[] edgePoints = new Vector2[outline.Count];//the array containsing the vertex points that will be used identify the edges
            for (int i = 0; i < outline.Count; i++)
            {
                edgePoints[i] = new Vector2(vertices[outline[i]].x, vertices[outline[i]].y);// assign the vector 3 location  of the vertecies List using the outline index 
            }
            edgeCollider.points = edgePoints;// assign the array of endepoints needed to generate the needed mesh

        }
    }

    void TriangulateSquare(Square square) //setting the test cases for the marching squares cases (provided by tutorial)
    {
        switch (square.configuration)
        {
            case 0:
                break;

            // 1 points:
            case 1:
                MeshFromPoints(square.centreLeft, square.centreBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centreBottom, square.centreRight);
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centreRight, square.centreTop);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft);
                break;

            // 2 points:
            case 3:
                MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 6:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
                break;
            case 5:
                MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            // 3 point:
            case 7:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            // 4 point:
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);// all 4 points of the grid will hve their vertecies already checked
                checkedVertices.Add(square.topLeft.vertexIndex);
                checkedVertices.Add(square.topRight.vertexIndex);
                checkedVertices.Add(square.bottomRight.vertexIndex);
                checkedVertices.Add(square.bottomLeft.vertexIndex);
                break;
        }

    }

    void MeshFromPoints(params Node[] points)//which nodes to connect to form a mesh
    {
        AssignVertices(points);

        if (points.Length >= 3)
            CreateTriangle(points[0], points[1], points[2]);
        if (points.Length >= 4)
            CreateTriangle(points[0], points[2], points[3]);
        if (points.Length >= 5)
            CreateTriangle(points[0], points[3], points[4]);
        if (points.Length >= 6)
            CreateTriangle(points[0], points[4], points[5]);

    }

    void AssignVertices(Node[] points)// assign the vertecies being used to the array
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;// assign the current vertex into the array index
                vertices.Add(points[i].position); //assign the vector3 postion as te vertex postiong
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c)// the 3 nodes being used as the vertecies for the triabgle being checked and adding them to the triangel dictionary along wiht their vertex
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexA, triangle);
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
    }

    void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
    {
        if (triangleDictionary.ContainsKey(vertexIndexKey)) // if the dictioanry already contains the key checked then add 
        {
            triangleDictionary[vertexIndexKey].Add(triangle);//add the triangle to the list in the dictionary with the coorrosponding key value
        }
        else// if the list already exists then add ot exist else generate a new list and add it to it
        {
            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(triangle);// adds the current trianlge toa  list to be added to the dictioanry
            triangleDictionary.Add(vertexIndexKey, triangleList);// adds the new triangle list to the triangle dictionary using the vertexindexKey as the keyset
        }
    }

    void CalculateMeshOutlines()
    {

        for (int i = 0; i < vertices.Count; i++)
        {
            if (!checkedVertices.Contains(i))// if the checked verecies does no include the the one its currently working on
            {
                int newOutlineVertex = GetConnectedOutlineVertex(i);// sets the new outline being checked 

                if (newOutlineVertex != -1) 
                {
                    checkedVertices.Add(i);//add the instance to the checked vertecies

                    List<int> newOutline = new List<int>();
                    newOutline.Add(i);//adds a new entry into the list
                    outlines.Add(newOutline);// assign the new list into the outlines list<List<>>
                    FollowOutline(newOutlineVertex, outlines.Count - 1);  
                    outlines[outlines.Count - 1].Add(i);
                }
            }
        }
    }

    void FollowOutline(int vertexIndex, int outlineIndex) 
    {
        outlines[outlineIndex].Add(vertexIndex);//add the indexed vertex to the outlines array using the outlineIndex as the index

        checkedVertices.Add(vertexIndex);// adds what is being checked to a hashset so that it doesnt need to recheck 

        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);//sets the next vertext to be checked

        if (nextVertexIndex != -1)// if the returned value isnt -1--
        {
            FollowOutline(nextVertexIndex, outlineIndex);// set the edge between the verts
        }
    }

    int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex]; //assign the Triangle variable to the new list using the vertex index as the key search

        for (int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            Triangle triangle = trianglesContainingVertex[i];// new instance of triangle becomes the listed triangle variable

            for (int j = 0; j < 3; j++)
            {
                int vertexB = triangle[j];
                if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB))//check if the vertext being checked is not the same vertext that its being compared to and it it has not already been checked
                {
                    if (IsOutlineEdge(vertexIndex, vertexB))//compare the vertex being checked with the next triangle
                    {
                        return vertexB;// if criteria are met then return the vertexB value--
                    }
                }
            }
        }

        return -1; // if conditions are not met then return a value of -1
    }

    bool IsOutlineEdge(int vertexA, int vertexB)// checking to see if the edge between the two verts is on the outside of the mesh
    {
        List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];//list of triangle structs adding the Triangle variable  using the vertexA as the key search

        int sharedTriangleCount = 0;//how many triangles share the same vert

        for (int i = 0; i < trianglesContainingVertexA.Count; i++)
        {
            if (trianglesContainingVertexA[i].Contains(vertexB))//if the vert being checked alerady exists in the vertexA--
            {
                sharedTriangleCount++;
                if (sharedTriangleCount > 1)// if the shared count is greater that one then break out and stop checking
                {
                    break;
                }
            }
        }
        return sharedTriangleCount == 1;// return true if the vert shares no triangles (only connected to one)
    }




}