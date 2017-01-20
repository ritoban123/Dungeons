using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Obsolete
{
    public class DungeonMeshGenerator
    {
        MeshData meshData = new MeshData();
        Dictionary<Coord, Vertex> coordVertMap = new Dictionary<Coord, Vertex>();
        public MeshData GenerateMesh(Dungeon dungeon)
        {

            for (int y = 0; y < dungeon.Height; y++)
            {
                for (int x = 0; x < dungeon.Width; x++)
                {

                    // Verticies
                    Tile t = dungeon.GetTileAt(x, y);
                    if (t.IsWall == false)
                        continue;

                    Coord c = new Coord(x + 1, y + 1);
                    CreateVertex(c);

                    if (t.SouthNeighbor == null || t.SouthNeighbor.IsWall == false)
                    {
                        c = new Coord(x + 1, y);
                        CreateVertex(c);
                    }

                    if (t.WestNeighbor == null || t.WestNeighbor.IsWall == false)
                    {
                        c = new Coord(x, y + 1);
                        CreateVertex(c);
                    }
                    Tile southWest = dungeon.GetTileAt(t.X - 1, t.Y - 1);
                    if (southWest == null || southWest.IsWall == false)
                    {
                        c = new Coord(x, y);
                        CreateVertex(c);
                    }

                    // Triangles
                    CreateTriangle(
                        new Coord(x, y),
                        new Coord(x + 1, y + 1),
                        new Coord(x + 1, y)
                        );
                    meshData.normals.Add(Vector3.up); // FIXME: This works for now, but could easily break
                    CreateTriangle(
                        new Coord(x, y),
                        new Coord(x, y + 1),
                        new Coord(x + 1, y + 1)
                        );
                    meshData.normals.Add(Vector3.up); // FIXME: This works for now, but could easily break
                }
            }
            return meshData;
        }

        private void CreateTriangle(Coord c1, Coord c2, Coord c3)
        {
            meshData.triangles.Add(coordVertMap[c1].vertexIndex);
            meshData.triangles.Add(coordVertMap[c2].vertexIndex);
            meshData.triangles.Add(coordVertMap[c2].vertexIndex);
        }

        private void CreateVertex(Coord c)
        {
            if (coordVertMap.ContainsKey(c))
                return;
            Vertex v = new Vertex(new Vector3(c.x, 0, c.y), meshData.verticies.Count);
            coordVertMap.Add(c, v);
            meshData.verticies.Add(v.position);
        }
    }
}