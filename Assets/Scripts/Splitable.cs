using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Splitable : MonoBehaviour {

    Mesh mesh;
    Vector3[] vertices;
    int verticesIndex;//the index of the next vertice to be added
    int[] triangles;

    ArrayList posTriangles = new ArrayList();
    ArrayList negTriangles = new ArrayList();

    ArrayList seamVertices = new ArrayList();

    ArrayList trackSplitEdges = new ArrayList();//holds index of first vertice on edge, then second, then new vertice that splits edge

	// Use this for initialization
	void Start () {
       mesh  = gameObject.GetComponent<MeshFilter>().mesh;
       PlaneGenerator.OnGeneration += split;
       vertices = mesh.vertices;
       verticesIndex = vertices.Length;
       triangles = mesh.triangles;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void split(Plane worldPlane){
        float distance = worldPlane.GetDistanceToPoint(transform.position);
        if (distance > 2.0f) return;//arbitrary value
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
            Vector3 current = (Vector3)seamVertices[j];
            x += current.x;
            y += current.y;
            z += current.z;
        }
        Vector3 center = new Vector3(x / n, y / n, z / n);
        int index = verticesIndex++;

        ArrayList newVertices = new ArrayList();
        newVertices.Add(center);//at index 0
        newVertices.AddRange(vertices);// index 1 to vertices.length
        newVertices.AddRange(seamVertices);// then add the new seam vertices
        Vector3[] doneVertices = (Vector3[])newVertices.ToArray(typeof(Vector3));

        Vector2[] uvs = new Vector2[doneVertices.Length];//might be why mesh is black
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(doneVertices[i].x, doneVertices[i].z);
        }

        if (posTriangles.Count != 0)//dont bother creating a gameobject if there are no triangles
        {
            GameObject split1 = new GameObject();
            split1.transform.position = transform.position;
            Mesh mesh1 = new Mesh();
            split1.AddComponent<MeshFilter>();
            split1.GetComponent<MeshFilter>().mesh = mesh1;
            mesh1.vertices = doneVertices;
            mesh1.uv = uvs;
            mesh1.RecalculateNormals();
            mesh1.colors = mesh.colors;
            mesh1.triangles = posTriangles.ToArray(typeof(int)) as int[];
            mesh1.Optimize();
            split1.AddComponent<MeshRenderer>();
            MeshRenderer mr1 = split1.GetComponent<MeshRenderer>();
            mr1.material = GetComponent<MeshRenderer>().material;
            split1.AddComponent<Splitable>();
        }
        else return;
        if (negTriangles.Count != 0)
        {
            GameObject split2 = new GameObject();
            split2.transform.position = transform.position;
            Mesh mesh2 = new Mesh();
            split2.AddComponent<MeshFilter>();
            split2.GetComponent<MeshFilter>().mesh = mesh2;
            mesh2.vertices = doneVertices;
            mesh2.uv = uvs;
            mesh2.RecalculateNormals();
            mesh2.colors = mesh.colors;
            mesh2.triangles = negTriangles.ToArray(typeof(int)) as int[];
            mesh2.Optimize();
            split2.AddComponent<MeshRenderer>();
            MeshRenderer mr2 = split2.GetComponent<MeshRenderer>();
            mr2.material = GetComponent<MeshRenderer>().material;
            split2.AddComponent<Splitable>();
        }
        else return;
        PlaneGenerator.OnGeneration -= split;
        Destroy(gameObject);
    }

    int findNewVertex(int vertex1, int vertex2, Plane plane)//returns index of new vertice, creates new vertice if it doesnt already exist
    {
        //check if new vertice already exists
        for (int i = 0; i < trackSplitEdges.Count; i += 3)
        {
            if (vertex1 == (int)trackSplitEdges[i] && vertex2 == (int)trackSplitEdges[i + 1])
            {
                int seamVertice = (int)trackSplitEdges[i + 2];
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
