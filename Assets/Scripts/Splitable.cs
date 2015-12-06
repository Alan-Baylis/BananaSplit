using UnityEngine;
using System.Collections.Generic;

public class Splitable : MonoBehaviour
{
	private bool splitting = false;

    private Mesh mesh;
    new private Transform transform;
    private Vector3[] vertices;
    private int verticesIndex;//the index of the next vertex to be added
    private int[] triangles;

    private List<Vector2> uv = new List<Vector2>();

    private List<int> posTriangles = new List<int>();
    private List<int> negTriangles = new List<int>();
    private List<int> posInnerTriangles = new List<int>();
    private List<int> negInnerTriangles = new List<int>();

    private List<Vector3> posNormals = new List<Vector3>();
    private List<Vector3> negNormals = new List<Vector3>();

    private List<Vector3> seamVertices = new List<Vector3>();

    private List<int> trackSplitEdges = new List<int>();//holds index of first vertex on edge, then second, then new vertex that splits edge

    // Use this for initialization
    private void Start()
    {
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        transform = gameObject.GetComponent<Transform>();

        PlaneGenerator.OnGeneration += Split;
        vertices = mesh.vertices;
        verticesIndex = vertices.Length;
        triangles = mesh.triangles;
        uv.Add(new Vector2(0, 0)); // center vertex
        uv.AddRange(mesh.uv); // pre-existing vertices
    }

    private void Split(Plane worldPlane, Vector3 lineStart, Vector3 lineEnd, int casts)
    {
		if (!splitting)
			return;

        float distance = worldPlane.GetDistanceToPoint(transform.position);

        if (distance > 2.0f)
            return;//arbitrary value

        Plane plane = new Plane();
        Vector3 newNormal = Quaternion.Inverse(transform.rotation) * worldPlane.normal;
        plane.SetNormalAndPosition(newNormal, worldPlane.normal * distance);

        posNormals.Add(-newNormal);
        negNormals.Add(newNormal);
        posNormals.AddRange(mesh.normals);
        negNormals.AddRange(mesh.normals);

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
                posTriangles.Add(i1 + 1); //first index is reserved for interior face middle vertex so every index is incremented by 1
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

                //find vertex with odd boolean value
                int vertex1, vertex2;
                if (side1 == odd)
                {
                    vertex1 = findNewVertex(i1, i2, plane);
                    vertex2 = findNewVertex(i1, i3, plane);
                    if (side1 == true)
                    {                                          //               i1 /\                  Positive Side
                        posTriangles.Add(i1 + 1);              //                 /  \
                        posTriangles.Add(vertex1 + 1);         //         vertex1/____\vertex2
                        posTriangles.Add(vertex2 + 1);         //               /   _-'\
                                                               //            i2/_.-'____\i3            Negative Side
                        negTriangles.Add(i2 + 1);
                        negTriangles.Add(i3 + 1);
                        negTriangles.Add(vertex2 + 1);

                        negTriangles.Add(i2 + 1);
                        negTriangles.Add(vertex2 + 1);
                        negTriangles.Add(vertex1 + 1);
                    }
                    else
                    {                                           //               i1 /\                  Negative Side
                        negTriangles.Add(i1 + 1);               //                 /  \
                        negTriangles.Add(vertex1 + 1);          //         vertex1/____\vertex2
                        negTriangles.Add(vertex2 + 1);          //               /   _-'\
                                                                //            i2/_.-'____\i3            Positive Side
                        posTriangles.Add(i2 + 1);
                        posTriangles.Add(i3 + 1);
                        posTriangles.Add(vertex2 + 1);

                        posTriangles.Add(i2 + 1);
                        posTriangles.Add(vertex2 + 1);
                        posTriangles.Add(vertex1 + 1);
                    }
                }
                else if (side2 == odd)
                {
                    vertex1 = findNewVertex(i2, i3, plane);
                    vertex2 = findNewVertex(i2, i1, plane);
                    if (side2 == true)
                    {                                           //               i2 /\                  Positive Side
                        posTriangles.Add(i2 + 1);               //                 /  \
                        posTriangles.Add(vertex1 + 1);          //         vertex1/____\vertex2
                        posTriangles.Add(vertex2 + 1);          //               /   _-'\
                                                                //            i3/_.-'____\i1            Negative Side
                        negTriangles.Add(i3 + 1);
                        negTriangles.Add(i1 + 1);
                        negTriangles.Add(vertex2 + 1);

                        negTriangles.Add(i3 + 1);
                        negTriangles.Add(vertex2 + 1);
                        negTriangles.Add(vertex1 + 1);
                    }
                    else
                    {                                           //               i2 /\                  Negative Side
                        negTriangles.Add(i2 + 1);               //                 /  \
                        negTriangles.Add(vertex1 + 1);          //         vertex1/____\vertex2
                        negTriangles.Add(vertex2 + 1);          //               /   _-'\
                                                                //            i3/_.-'____\i1            Positive Side
                        posTriangles.Add(i3 + 1);
                        posTriangles.Add(i1 + 1);
                        posTriangles.Add(vertex2 + 1);

                        posTriangles.Add(i3 + 1);
                        posTriangles.Add(vertex2 + 1);
                        posTriangles.Add(vertex1 + 1);
                    }
                }
                else
                {
                    vertex1 = findNewVertex(i3, i1, plane);
                    vertex2 = findNewVertex(i3, i2, plane);
                    if (side3 == true)
                    {                                           //               i3 /\                  Positive Side
                        posTriangles.Add(i3 + 1);               //                 /  \
                        posTriangles.Add(vertex1 + 1);          //         vertex1/____\vertex2
                        posTriangles.Add(vertex2 + 1);          //               /   _-'\
                                                                //            i1/_.-'____\i2            Negative Side
                        negTriangles.Add(i1 + 1);
                        negTriangles.Add(i2 + 1);
                        negTriangles.Add(vertex2 + 1);

                        negTriangles.Add(i1 + 1);
                        negTriangles.Add(vertex2 + 1);
                        negTriangles.Add(vertex1 + 1);
                    }
                    else
                    {                                           //               i3 /\                  Negative Side
                        negTriangles.Add(i3 + 1);               //                 /  \
                        negTriangles.Add(vertex1 + 1);          //         vertex1/____\vertex2
                        negTriangles.Add(vertex2 + 1);          //               /   _-'\
                                                                //            i1/_.-'____\i2            Positive Side
                        posTriangles.Add(i1 + 1);
                        posTriangles.Add(i2 + 1);
                        posTriangles.Add(vertex2 + 1);

                        posTriangles.Add(i1 + 1);
                        posTriangles.Add(vertex2 + 1);
                        posTriangles.Add(vertex1 + 1);
                    }
                }

                if (odd == true)
                {
                    //add inner triangles
                    posInnerTriangles.Add(vertex1 + 2);
                    posInnerTriangles.Add(0);
                    posInnerTriangles.Add(vertex2 + 2);

                    negInnerTriangles.Add(vertex1 + 2);
                    negInnerTriangles.Add(vertex2 + 2);
                    negInnerTriangles.Add(0);
                }
                else
                {
                    negInnerTriangles.Add(vertex1 + 2);
                    negInnerTriangles.Add(0);
                    negInnerTriangles.Add(vertex2 + 2);

                    posInnerTriangles.Add(vertex1 + 2);
                    posInnerTriangles.Add(vertex2 + 2);
                    posInnerTriangles.Add(0);
                }

                /*if (odd == true)
                {
                    //add inner triangles
                    posTriangles.Add(vertex1 + 1);
                    posTriangles.Add(0);
                    posTriangles.Add(vertex2 + 1);

                    negTriangles.Add(vertex1 + 1);
                    negTriangles.Add(vertex2 + 1);
                    negTriangles.Add(0);
                }
                else
                {
                    negTriangles.Add(vertex1 + 1);
                    negTriangles.Add(0);
                    negTriangles.Add(vertex2 + 1);

                    posTriangles.Add(vertex1 + 1);
                    posTriangles.Add(vertex2 + 1);
                    posTriangles.Add(0);
                }*/
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
        Vector2[] uvs = uv.ToArray();

        if (posTriangles.Count != 0 && negTriangles.Count != 0)//dont bother creating a gameobject if there are no triangles
        {
            Vector3 force = plane.normal * 50f;
            CreateNewSplit(doneVertices, posTriangles.ToArray(), posInnerTriangles.ToArray(), uvs, posNormals.ToArray(), force);
            CreateNewSplit(doneVertices, negTriangles.ToArray(), negInnerTriangles.ToArray(), uvs, negNormals.ToArray(), -force);
        }
        else
        {
            return;
        }

		splitting = false;
        PlaneGenerator.OnGeneration -= Split;
        Destroy(gameObject);
    }

    void CreateNewSplit(Vector3[] verts, int[] tris, int[] innerTris, Vector2[] uvs, Vector3[] normals, Vector3 force)
    {
        var split = new GameObject();
        split.transform.position = transform.position;
        split.transform.rotation = transform.rotation;
        split.transform.parent = transform.parent;

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.subMeshCount += 1;
        mesh.SetTriangles(innerTris, 1);
        mesh.uv = uvs;
        //mesh.RecalculateNormals();
        mesh.normals = normals;
        mesh.colors = this.mesh.colors;
        mesh.Optimize();

        split.AddComponent<MeshFilter>().mesh = mesh;

        var rend = split.AddComponent<MeshRenderer>();
        Material[] materials = new Material[2]{
            GetComponent<MeshRenderer>().material,
            GetComponent<MeshRenderer>().material
        };
        rend.materials = materials;
        //rend.material = GetComponent<MeshRenderer>().material;

        split.AddComponent<Splitable>();

        var convex = new SimpleConvex(mesh);
        var simpleMesh1 = convex.BuildSimplifiedConvexMesh();
        var collider = split.AddComponent<MeshCollider>();
        collider.sharedMesh = simpleMesh1;
        collider.convex = true;

        var rigidBody = split.AddComponent<Rigidbody>();
        rigidBody.AddForce(force);
        /*split.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;*/
    }

    int findNewVertex(int vertex1, int vertex2, Plane plane)//returns index of new vertex, creates new vertex if it doesnt already exist
    {
        //check if new vertex already exists
        for (int i = 0; i < trackSplitEdges.Count; i += 3)
        {
            if (vertex1 == trackSplitEdges[i] && vertex2 == trackSplitEdges[i + 1])
            {
                int seamvertex = trackSplitEdges[i + 2];
                trackSplitEdges.RemoveRange(i, 3);// since an edge is only used twice it can be removed after it is used the second time
                return seamvertex;
            }
        }
        //if it doesnt already exist
        Vector3 direction = Vector3.Normalize(vertices[vertex2] - vertices[vertex1]);
        float distance;
        plane.Raycast(new Ray(vertices[vertex1], direction), out distance);
        Vector3 newVertex = vertices[vertex1] + distance * direction;
        seamVertices.Add(newVertex);
        seamVertices.Add(newVertex);//double vertices so that the inner submesh can have a separate normal

        //generate new uv coordinates
        Vector2 uv1 = uv[vertex1 + 1];
        Vector2 uv2 = uv[vertex2 + 1];
        Vector2 uv3 = Vector2.Lerp(uv1, uv2, distance / Vector3.Distance(vertices[vertex2], vertices[vertex1]));
        uv.Add(uv3);
        uv.Add(new Vector2(newVertex.x, newVertex.z));// add uv for inner submesh vertices

        //generate new normals
        Vector3 normal1 = posNormals[vertex1 + 1];
        Vector3 normal2 = posNormals[vertex2 + 1];
        Vector3 normal3 = Vector3.Lerp(normal1, normal2, distance / Vector3.Distance(vertices[vertex2], vertices[vertex1]));
        posNormals.Add(normal3);
        posNormals.Add(posNormals[0]);//add normal for inner submesh vertices
        negNormals.Add(normal3);
        negNormals.Add(negNormals[0]);

        //add second before first because next time we check trackSplitEdges, the second vertex will be the first and vice versa
        trackSplitEdges.Add(vertex2); trackSplitEdges.Add(vertex1); trackSplitEdges.Add(verticesIndex);

        verticesIndex += 2;
        return verticesIndex - 2;
    }

	void HitByRay(){
		splitting = true;
	}
}
