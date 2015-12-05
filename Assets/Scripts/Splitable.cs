using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Splitable : MonoBehaviour {

    private Mesh mesh;
    private Vector3[] vertices;
    private int verticesIndex;//the index of the next vertex to be added
    private int[] triangles;

    private List<int> posTriangles = new List<int>();
    private List<int> negTriangles = new List<int>();

    private List<Vector3> seamVertices = new List<Vector3>();

    private List<int> trackSplitEdges = new List<int>();//holds index of first vertex on edge, then second, then new vertice that splits edge

	// Use this for initialization
	private void Start()
    {
       mesh = gameObject.GetComponent<MeshFilter>().mesh;
       PlaneGenerator.OnGeneration += Split;
       vertices = mesh.vertices;
       verticesIndex = vertices.Length;
       triangles = mesh.triangles;
	}

    private void Split(Plane worldPlane)
    {
        float distance = worldPlane.GetDistanceToPoint(transform.position);

        if (distance > 2.0f)
            return;//arbitrary value

        Plane plane = new Plane();
        plane.SetNormalAndPosition(worldPlane.normal, worldPlane.normal * distance);//ignores model rotation

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i1 = triangles[i];//index 1, 2 and 3 of vertices
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];
            bool side1 = plane.GetSide(vertices[i1]);
            bool side2 = plane.GetSide(vertices[i2]);
            bool side3 = plane.GetSide(vertices[i3]);

            if (side1 == true && side2 == true && side3 == true)
            {
                posTriangles.Add(i1 + 1); //first index is reserved for interior face middle vertice so every index is incremented by 1
                posTriangles.Add(i2 + 1);
                posTriangles.Add(i3 + 1);
            }
            else if (side1 == false && side2 == false && side3 == false)
            {
                negTriangles.Add(i1 + 1);
                negTriangles.Add(i2 + 1);
                negTriangles.Add(i3 + 1);
            }
            else
            {
                //find odd boolean value(expression found using karnaugh map)
                bool odd = (!side1 && !side2) || (!side1 && !side3) || (!side2 && !side3);

                //find vertice with odd boolean value
                int vertice1, vertice2;
                if (side1 == odd)
                {
                    vertice1 = findNewVertex(i1, i2, plane);
                    vertice2 = findNewVertex(i1, i3, plane);
                    if (side1 == true)
                    {                                           //               i1 /\                  Positive Side
                        posTriangles.Add(i1 + 1);               //                 /  \
                        posTriangles.Add(vertice1 + 1);         //        vertice1/____\vertice2
                        posTriangles.Add(vertice2 + 1);         //               /   _-'\
                                                                //            i2/_.-'____\i3            Negative Side
                        negTriangles.Add(i2 + 1);
                        negTriangles.Add(i3 + 1);
                        negTriangles.Add(vertice2 + 1);

                        negTriangles.Add(i2 + 1);
                        negTriangles.Add(vertice2 + 1);
                        negTriangles.Add(vertice1 + 1);
                    }
                    else
                    {                                           //               i1 /\                  Negative Side
                        negTriangles.Add(i1 + 1);               //                 /  \
                        negTriangles.Add(vertice1 + 1);         //        vertice1/____\vertice2
                        negTriangles.Add(vertice2 + 1);         //               /   _-'\
                                                                //            i2/_.-'____\i3            Positive Side
                        posTriangles.Add(i2 + 1);
                        posTriangles.Add(i3 + 1);
                        posTriangles.Add(vertice2 + 1);

                        posTriangles.Add(i2 + 1);
                        posTriangles.Add(vertice2 + 1);
                        posTriangles.Add(vertice1 + 1);
                    }
                }
                else if (side2 == odd)
                {
                    vertice1 = findNewVertex(i2, i3, plane);
                    vertice2 = findNewVertex(i2, i1, plane);
                    if (side2 == true)
                    {                                           //               i2 /\                  Positive Side
                        posTriangles.Add(i2 + 1);               //                 /  \
                        posTriangles.Add(vertice1 + 1);         //        vertice1/____\vertice2
                        posTriangles.Add(vertice2 + 1);         //               /   _-'\
                                                                //            i3/_.-'____\i1            Negative Side
                        negTriangles.Add(i3 + 1);
                        negTriangles.Add(i1 + 1);
                        negTriangles.Add(vertice2 + 1);

                        negTriangles.Add(i3 + 1);
                        negTriangles.Add(vertice2 + 1);
                        negTriangles.Add(vertice1 + 1);
                    }
                    else
                    {                                           //               i2 /\                  Negative Side
                        negTriangles.Add(i2 + 1);               //                 /  \
                        negTriangles.Add(vertice1 + 1);         //        vertice1/____\vertice2
                        negTriangles.Add(vertice2 + 1);         //               /   _-'\
                                                                //            i3/_.-'____\i1            Positive Side
                        posTriangles.Add(i3 + 1);
                        posTriangles.Add(i1 + 1);
                        posTriangles.Add(vertice2 + 1);

                        posTriangles.Add(i3 + 1);
                        posTriangles.Add(vertice2 + 1);
                        posTriangles.Add(vertice1 + 1);
                    }
                }
                else
                {
                    vertice1 = findNewVertex(i3, i1, plane);
                    vertice2 = findNewVertex(i3, i2, plane);
                    if (side3 == true)
                    {                                           //               i3 /\                  Positive Side
                        posTriangles.Add(i3 + 1);               //                 /  \
                        posTriangles.Add(vertice1 + 1);         //        vertice1/____\vertice2
                        posTriangles.Add(vertice2 + 1);         //               /   _-'\
                                                                //            i1/_.-'____\i2            Negative Side
                        negTriangles.Add(i1 + 1);
                        negTriangles.Add(i2 + 1);
                        negTriangles.Add(vertice2 + 1);

                        negTriangles.Add(i1 + 1);
                        negTriangles.Add(vertice2 + 1);
                        negTriangles.Add(vertice1 + 1);
                    }
                    else
                    {                                           //               i3 /\                  Negative Side
                        negTriangles.Add(i3 + 1);               //                 /  \
                        negTriangles.Add(vertice1 + 1);         //        vertice1/____\vertice2
                        negTriangles.Add(vertice2 + 1);         //               /   _-'\
                                                                //            i1/_.-'____\i2            Positive Side
                        posTriangles.Add(i1 + 1);
                        posTriangles.Add(i2 + 1);
                        posTriangles.Add(vertice2 + 1);

                        posTriangles.Add(i1 + 1);
                        posTriangles.Add(vertice2 + 1);
                        posTriangles.Add(vertice1 + 1);
                    }
                }
                if (odd == true)
                {
                    //add inner triangles
                    posTriangles.Add(vertice1 + 1);
                    posTriangles.Add(0);
                    posTriangles.Add(vertice2 + 1);

                    negTriangles.Add(vertice1 + 1);
                    negTriangles.Add(vertice2 + 1);
                    negTriangles.Add(0);
                }
                else
                {
                    negTriangles.Add(vertice1 + 1);
                    negTriangles.Add(0);
                    negTriangles.Add(vertice2 + 1);

                    posTriangles.Add(vertice1 + 1);
                    posTriangles.Add(vertice2 + 1);
                    posTriangles.Add(0);
                }
            }
        }
        //now average all seam vertices to find center of inner face
        float x = 0;
        float y = 0;
        float z = 0;
        int n = seamVertices.Count;
        for (int j = 0; j < n; j++)
        {
            Vector3 current = seamVertices[j];
            x += current.x;
            y += current.y;
            z += current.z;
        }
        Vector3 center = new Vector3(x / n, y / n, z / n);

        var newVertices = new List<Vector3>();
        newVertices.Add(center); // at index 0
        newVertices.AddRange(vertices); // index 1 to vertices.length
        newVertices.AddRange(seamVertices); // then add the new seam vertices
        Vector3[] doneVertices = newVertices.ToArray();

        Vector2[] uvs = new Vector2[doneVertices.Length];//might be why mesh is black
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(doneVertices[i].x, doneVertices[i].z);
        }

        if (posTriangles.Count != 0 && negTriangles.Count != 0)//dont bother creating a gameobject if there are no triangles
        {
            CreateNewSplit(doneVertices, posTriangles.ToArray(), uvs);
            CreateNewSplit(doneVertices, negTriangles.ToArray(), uvs);
        }
        else
        {
            return;
        }

        PlaneGenerator.OnGeneration -= Split;
        Destroy(gameObject);
    }

    void CreateNewSplit(Vector3[] verts, int[] tris, Vector2[] uvs)
    {
        var split = new GameObject();
        split.transform.position = transform.position;

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.colors = this.mesh.colors;
        mesh.Optimize();

        split.AddComponent<MeshFilter>().mesh = mesh;

        var rend = split.AddComponent<MeshRenderer>();
        rend.material = GetComponent<MeshRenderer>().material;

        split.AddComponent<Splitable>();

        var convex = new SimpleConvex(mesh);
        var simpleMesh1 = convex.BuildSimplifiedConvexMesh();
        var collider = split.AddComponent<MeshCollider>();
        collider.sharedMesh = simpleMesh1;
        collider.convex = true;

        split.AddComponent<Rigidbody>();
    }

    int findNewVertex(int vertex1, int vertex2, Plane plane)//returns index of new vertice, creates new vertice if it doesnt already exist
    {
        //check if new vertice already exists
        for (int i = 0; i < trackSplitEdges.Count; i += 3)
        {
            if (vertex1 == trackSplitEdges[i] && vertex2 == trackSplitEdges[i + 1])
            {
                int seamVertice = trackSplitEdges[i + 2];
                trackSplitEdges.RemoveRange(i, 3);// since an edge is only used twice it can be removed after it is used the second time
                return seamVertice;
            }
        }
        //if it doesnt already exist
        Vector3 direction = Vector3.Normalize(vertices[vertex2] - vertices[vertex1]);
        float distance;
        plane.Raycast(new Ray(vertices[vertex1], direction), out distance);
        Vector3 newVertice = vertices[vertex1] + distance * direction;
        seamVertices.Add(newVertice);
        //add second before first because next time we check trackSplitEdges, the second vertex will be the first and vice versa
        trackSplitEdges.Add(vertex2); trackSplitEdges.Add(vertex1); trackSplitEdges.Add(verticesIndex);
        return verticesIndex++;
    }
}
