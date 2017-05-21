public class Square// the setting for each individual square and assigning the Node and control nodes accoring to each generated square (control node controls the nodes above and the the right)
{

    public ControlNode topLeft, topRight, bottomRight, bottomLeft;// 4 corners are the main control nodes
    public Node centreTop, centreRight, centreBottom, centreLeft;//4 points along the edge are the normal nodes
    public int configuration;

    public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)// creating the control nodes and finding their positions
    {
        //control nodes
        topLeft = _topLeft;
        topRight = _topRight;
        bottomRight = _bottomRight;
        bottomLeft = _bottomLeft;

        //nodes ( position the nodes to the right or above the contorl node)
        centreTop = topLeft.right;
        centreRight = bottomRight.above;
        centreBottom = bottomLeft.right;
        centreLeft = bottomLeft.above;

        if (topLeft.active)// calcualting what the configuration fo the marching squares will equate to
            configuration += 8;
        if (topRight.active)//checking if the conteolNode is active to calculate the configuration
            configuration += 4;
        if (bottomRight.active)
            configuration += 2;
        if (bottomLeft.active)
            configuration += 1;
    }

}