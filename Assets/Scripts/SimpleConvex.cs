using UnityEngine;

/*
Class written to generate a simple convex hull from a mesh
Useful for MeshColliders which require that the mesh be < 256 triangles

    Credit to mathmos_ http://answers.unity3d.com/questions/380233/generating-a-convex-hull.html
*/
public class SimpleConvex
{
    Mesh mesh;

    public SimpleConvex(Mesh mesh)
    {
        this.mesh = mesh;
    }

    public Mesh BuildSimplifiedConvexMesh()
    {
        var builder = new MeshBuilder();

        for (int i = 0; i < 64; i++)
        {
            int index = Random.Range(0, mesh.triangles.Length / 3) * 3;

            Vector3[] triangle = new Vector3[]
            {
                mesh.vertices[mesh.triangles[index + 0]],
                mesh.vertices[mesh.triangles[index + 1]],
                mesh.vertices[mesh.triangles[index + 2]]
            };

            Vector3[] norms = new Vector3[]
            {
                Vector3.up,
                Vector3.up,
                Vector3.up
            };

            Vector2[] uvs = new Vector2[]
            {
                mesh.uv[mesh.triangles[index + 0]],
                mesh.uv[mesh.triangles[index + 1]],
                mesh.uv[mesh.triangles[index + 2]]
            };

            builder.AddTriangleToMesh(triangle, norms, uvs);
        }

        return builder.Build();
    }
}