struct Triangle // assignign the triangles that will be generated betweenn activated nodes
{
    //the 3 points that make up the tri
    public int vertexIndexA;
    public int vertexIndexB;
    public int vertexIndexC;
    //holding the vertices
    int[] vertices;

    public Triangle(int a, int b, int c)//used to call and assign the vertices being checked
    {
        vertexIndexA = a;
        vertexIndexB = b;
        vertexIndexC = c;

        vertices = new int[3];//assinging the verts in order into the array
        vertices[0] = a;
        vertices[1] = b;
        vertices[2] = c;
    }

    public int this[int i]
    {
        get
        {
            return vertices[i];
        }
    }


    public bool Contains(int vertexIndex)
    {
        return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;//checking if the associated trianlge shares any of the verts
    }
}