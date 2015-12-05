using UnityEngine;
using System.Collections;

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
        Plane plane = new Plane();
        plane.SetNormalAndPosition(worldPlane.normal, worldPlane.normal * distance);//ignores model rotation

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i1 = triangles[i];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];
            bool side1 = plane.GetSide(vertices[i1]);
            bool side2 = plane.GetSide(vertices[i2]);
            bool side3 = plane.GetSide(vertices[i3]);

            if (side1 == true && side2 == true && side3 == true)
            {
                posTriangles.Add(i1);
                posTriangles.Add(i2);
                posTriangles.Add(i3);
            }
            else if (side1 == false && side2 == false && side3 == false)
            {
                negTriangles.Add(i1);
                negTriangles.Add(i2);
                negTriangles.Add(i3);
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
                    {                                       //               i1 /\                  Positive Side
                        posTriangles.Add(i1);               //                 /  \
                        posTriangles.Add(vertice1);         //        vertice1/____\vertice2
                        posTriangles.Add(vertice2);         //               /   _-'\
                                                            //            i2/_.-'____\i3            Negative Side
                        negTriangles.Add(i2);
                        negTriangles.Add(i3);
                        negTriangles.Add(vertice2);

                        negTriangles.Add(i2);
                        negTriangles.Add(vertice2);
                        negTriangles.Add(vertice1);
                    }
                    else
                    {                                       //               i1 /\                  Negative Side
                        negTriangles.Add(i1);               //                 /  \
                        negTriangles.Add(vertice1);         //        vertice1/____\vertice2
                        negTriangles.Add(vertice2);         //               /   _-'\
                                                            //            i2/_.-'____\i3            Positive Side
                        posTriangles.Add(i2);
                        posTriangles.Add(i3);
                        posTriangles.Add(vertice2);

                        posTriangles.Add(i2);
                        posTriangles.Add(vertice2);
                        posTriangles.Add(vertice1);
                    }
                }
                else if (side2 == odd)
                {
                    vertice1 = findNewVertex(i2, i3, plane);
                    vertice2 = findNewVertex(i2, i1, plane);
                    if (side2 == true)
                    {                                       //               i2 /\                  Positive Side
                        posTriangles.Add(i2);               //                 /  \
                        posTriangles.Add(vertice1);         //        vertice1/____\vertice2
                        posTriangles.Add(vertice2);         //               /   _-'\
                                                            //            i3/_.-'____\i1            Negative Side
                        negTriangles.Add(i3);
                        negTriangles.Add(i1);
                        negTriangles.Add(vertice2);

                        negTriangles.Add(i3);
                        negTriangles.Add(vertice2);
                        negTriangles.Add(vertice1);
                    }
                    else
                    {                                       //               i2 /\                  Negative Side
                        negTriangles.Add(i2);               //                 /  \
                        negTriangles.Add(vertice1);         //        vertice1/____\vertice2
                        negTriangles.Add(vertice2);         //               /   _-'\
                                                            //            i3/_.-'____\i1            Positive Side
                        posTriangles.Add(i3);
                        posTriangles.Add(i1);
                        posTriangles.Add(vertice2);

                        posTriangles.Add(i3);
                        posTriangles.Add(vertice2);
                        posTriangles.Add(vertice1);
                    }
                }
                else
                {
                    vertice1 = findNewVertex(i3, i1, plane);
                    vertice2 = findNewVertex(i3, i2, plane);
                    if (side3 == true)
                    {                                       //               i3 /\                  Positive Side
                        posTriangles.Add(i3);               //                 /  \
                        posTriangles.Add(vertice1);         //        vertice1/____\vertice2
                        posTriangles.Add(vertice2);         //               /   _-'\
                                                            //            i1/_.-'____\i2            Negative Side
                        negTriangles.Add(i1);
                        negTriangles.Add(i2);
                        negTriangles.Add(vertice2);

                        negTriangles.Add(i1);
                        negTriangles.Add(vertice2);
                        negTriangles.Add(vertice1);
                    }
                    else
                    {                                       //               i3 /\                  Negative Side
                        negTriangles.Add(i3);               //                 /  \
                        negTriangles.Add(vertice1);         //        vertice1/____\vertice2
                        negTriangles.Add(vertice2);         //               /   _-'\
                                                            //            i1/_.-'____\i2            Positive Side
                        posTriangles.Add(i1);
                        posTriangles.Add(i2);
                        posTriangles.Add(vertice2);

                        posTriangles.Add(i1);
                        posTriangles.Add(vertice2);
                        posTriangles.Add(vertice1);
                    }
                }
                /*//now average all seam vertices to find center of inner face
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
                seamVertices.Add(center);
                //Now create triangle interior triangles*/
            }
        }
        ArrayList newVertices = new ArrayList();
        newVertices.AddRange(vertices);
        newVertices.AddRange(seamVertices);
        Vector3[] doneVertices = (Vector3[])newVertices.ToArray(typeof(Vector3));

        GameObject split1 = new GameObject();
        GameObject split2 = new GameObject();

        split1.transform.position = transform.position;
        split2.transform.position = transform.position;

        Mesh mesh1 = new Mesh();
        Mesh mesh2 = new Mesh();

        split1.AddComponent<MeshFilter>();
        split2.AddComponent<MeshFilter>();
        split1.GetComponent<MeshFilter>().mesh = mesh1;
        split2.GetComponent<MeshFilter>().mesh = mesh2;

        mesh1.vertices = doneVertices;
        mesh2.vertices = doneVertices;

        /*mesh1.uv = mesh.uv;
        mesh2.uv = mesh.uv;*/

        mesh1.RecalculateNormals();
        mesh2.RecalculateNormals();

        mesh1.triangles = posTriangles.ToArray(typeof(int)) as int[];
        mesh2.triangles = negTriangles.ToArray(typeof(int)) as int[];
        split1.AddComponent<MeshRenderer>();
        split2.AddComponent<MeshRenderer>();
        MeshRenderer mr = GetComponent<MeshRenderer>();
        MeshRenderer mr1 = split1.GetComponent<MeshRenderer>();
        MeshRenderer mr2 = split2.GetComponent<MeshRenderer>();
        mr1.material = mr.material;
        mr2.material = mr.material;
        split1.AddComponent<Splitable>();
        split2.AddComponent<Splitable>();
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
        trackSplitEdges.Add(vertex2); trackSplitEdges.Add(vertex1); trackSplitEdges.Add(verticesIndex);
        return verticesIndex++;
    }
}
