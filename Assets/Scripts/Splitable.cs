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
       vertices = mesh.vertices;
       verticesIndex = vertices.Length;
       triangles = mesh.triangles;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void split(Plane plane){
        for (int i = 0; i < triangles.Length; i += 3)
        {
            bool side1 = plane.GetSide(vertices[triangles[i]]);
            bool side2 = plane.GetSide(vertices[triangles[i + 1]]);
            bool side3 = plane.GetSide(vertices[triangles[i + 2]]);

            if (side1 == side2 == side3 == true)
            {
                posTriangles.Add(triangles[i]);
                posTriangles.Add(triangles[i + 1]);
                posTriangles.Add(triangles[i + 2]);
            }
            else if (side1 == side2 == side3 == false)
            {
                negTriangles.Add(triangles[i]);
                negTriangles.Add(triangles[i + 1]);
                negTriangles.Add(triangles[i + 2]);
            }
            else
            {
                //find odd boolean value(expression found using karnaugh map)
                bool odd = (!side1 && !side2) || (!side1 && !side3) || (!side2 && !side3);

                //find vertice with odd boolean value
                int vertice1, vertice2;
                if (side1 == odd)
                {
                    vertice1 = findNewVertex(triangles[i], triangles[i + 1], plane);
                    vertice2 = findNewVertex(triangles[i], triangles[i + 2], plane);

                }
                else if (side2 == odd)
                {
                    vertice1 = findNewVertex(triangles[i + 1], triangles[i + 2], plane);
                    vertice2 = findNewVertex(triangles[i + 1], triangles[i], plane);
                }
                else
                {
                    vertice1 = findNewVertex(triangles[i + 2], triangles[i], plane);
                    vertice2 = findNewVertex(triangles[i + 2], triangles[i + 1], plane);
                }
                
                //add intersection to end of vertices array

            }
        }

        
    }

    int findNewVertex(int vertex1, int vertex2, Plane plane)//returns index of new vertice, creates new vertice if it doesnt already exist
    {
        //check if new vertice already exists
        for (int i = 0; i < trackSplitEdges.Count; i += 3)
        {
            if (vertex1 == (int)trackSplitEdges[i] && vertex2 == (int)trackSplitEdges[i + 1])
            {
                int seamVertice = (int)trackSplitEdges[i + 2];
                trackSplitEdges.RemoveRange(i, 3);
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
