using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Node:INode// used to generate a Vector3 postition for a node to be generated
{
    public Vector3 position { get; set; }
    public int vertexIndex { get; set; }
    
    public  Node(Vector3 _pos)// the new Vector3 location being assigned
    {
        position = _pos;
        vertexIndex = -1;

    }
}


interface INode
{
    int vertexIndex { get; set; }
    Vector3 position { get; set; }

}