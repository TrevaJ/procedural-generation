using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlNode : Node
{

    public bool active;
    public Node above, right;

    public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) //position of the control node (its active status)(size of the the square)
    {
        active = _active; //is the control node active
        above = new Node(position + Vector3.up * squareSize / 2f);// creates the node above the controlnode
        right = new Node(position + Vector3.right * squareSize / 2f);//creates the node to the right of the controlNode
    }

}
