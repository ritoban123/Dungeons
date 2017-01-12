using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public struct Vertex
{
    public Vector3 position;
    public int vertexIndex;

    public Vertex(Vector3 position, int vertexIndex)
    {
        this.position = position;
        this.vertexIndex = vertexIndex;
    }
}

public struct Coord
{
    public int x;
    public int y;

    public Coord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class MeshData
{
    public List<Vector3> verticies = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector3> normals = new List<Vector3>();
    public List<Vector2> uvs = new List<Vector2>();


}