using UnityEngine;
using System.Collections;

public class SimpleConvex
{
    Mesh mesh;

    // Use this for initialization
    public SimpleConvex(Mesh mesh)
    {
        this.mesh = mesh;
    }

    public Mesh BuildSimplifiedConvexMesh()
    {
        Debug.Log(mesh.triangles.Length / 3 + " tris");

        var builder = new MeshBuilder();

        for (int i = 0; i < 64; i++)
        {
            int index = Random.Range(0, mesh.triangles.Length / 3) * 3;

            Vector3[] triangle = new Vector3[] { mesh.vertices[mesh.triangles[index]], mesh.vertices[mesh.triangles[index + 1]], mesh.vertices[mesh.triangles[index + 2]] };
            Vector3[] norms = new Vector3[] { Vector3.up, Vector3.up, Vector3.up };
            Vector2[] uvs = new Vector2[] { mesh.uv[mesh.triangles[index]], mesh.uv[mesh.triangles[index + 1]], mesh.uv[mesh.triangles[index + 2]] };

            builder.AddTriangleToMesh(triangle, norms, uvs);
        }

        Mesh polygonSoup = builder.Build();
        Debug.Log(polygonSoup.triangles.Length / 3 + " tris");

        return polygonSoup;
    }
}