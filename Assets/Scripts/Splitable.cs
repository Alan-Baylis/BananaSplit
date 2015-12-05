using UnityEngine;
using System.Collections;

public class Splitable : MonoBehaviour {

    Mesh mesh;

	// Use this for initialization
	void Start () {
       mesh  = gameObject.GetComponent<MeshFilter>().mesh;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void split(Plane plane){

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        ArrayList posTriangles = new ArrayList();
        ArrayList negTriangles = new ArrayList();

        ArrayList seamVertices = new ArrayList();
        ArrayList seamTriangles = new ArrayList();

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
                Vector3 direction1, direction2;
                Vector3 vertice1, vertice2;
                if (side1 == odd)
                {
                    direction1 = Vector3.Normalize(vertices[triangles[i + 1]] - vertices[triangles[i]]);
                    direction2 = Vector3.Normalize(vertices[triangles[i + 2]] - vertices[triangles[i]]);
                    float distance1, distance2;
                    plane.Raycast(new Ray(vertices[triangles[i]], direction1), out distance1);
                    plane.Raycast(new Ray(vertices[triangles[i]], direction2), out distance2);
                    vertice1 = vertices[triangles[i]] + distance1 * direction1;
                    vertice2 = vertices[triangles[i]] + distance2 * direction2;
                }
                else if (side2 == odd)
                {
                    direction1 = Vector3.Normalize(vertices[triangles[i]] - vertices[triangles[i + 1]]);
                    direction2 = Vector3.Normalize(vertices[triangles[i + 2]] - vertices[triangles[i + 1]]);
                    float distance1, distance2;
                    plane.Raycast(new Ray(vertices[triangles[i + 1]], direction1), out distance1);
                    plane.Raycast(new Ray(vertices[triangles[i + 1]], direction2), out distance2);
                    vertice1 = vertices[triangles[i + 1]] + distance1 * direction1;
                    vertice2 = vertices[triangles[i + 1]] + distance2 * direction2;
                }else
                {
                    direction1 = Vector3.Normalize(vertices[triangles[i]] - vertices[triangles[i + 2]]);
                    direction2 = Vector3.Normalize(vertices[triangles[i + 1]] - vertices[triangles[i + 2]]);
                    float distance1, distance2;
                    plane.Raycast(new Ray(vertices[triangles[i + 2]], direction1), out distance1);
                    plane.Raycast(new Ray(vertices[triangles[i + 2]], direction2), out distance2);
                    vertice1 = vertices[triangles[i + 2]] + distance1 * direction1;
                    vertice2 = vertices[triangles[i + 2]] + distance2 * direction2;
                }
                //add intersection to end of vertices array

            }
        }

        /*while(index < vertices.Count)
        for (int i = 0; i < triangles.Length; i += 3)
        {
            float vertice1 = Vector3.Dot((Vector3)vertices[i] - planePoint, planeNormal);
            float vertice2 = Vector3.Dot((Vector3)vertices[i + 1] - planePoint, planeNormal);
            float vertice3 = Vector3.Dot((Vector3)vertices[i + 2] - planePoint, planeNormal);

            if (vertice1 < 0 && vertice2 < 0 && vertice3 < 0)
            {
                leftTriangles.Add(i); leftTriangles.Add(i + 1); leftTriangles.Add(i + 2);
            }
            else if (vertice1 > 0 && vertice2 > 0 && vertice3 > 0)
            {
                rightTriangles.Add(i); rightTriangles.Add(i + 1); rightTriangles.Add(i + 2);
            }
            else // triangles are split along plane
            {
                float sign = vertice1 * vertice2 * vertice3;
                Vector3 newVertex1;
                Vector3 newVertex2;
                if (sign < 0) // find the 2 vertices that cut the triangle 
                {
                    if (vertice1 < 0)
                    {
                        newVertex1 = findNewVertex((Vector3)vertices[i + 1], (Vector3)vertices[i] - (Vector3)vertices[i + 1], planePoint, planeNormal);
                        newVertex2 = findNewVertex((Vector3)vertices[i + 2], (Vector3)vertices[i] - (Vector3)vertices[i + 2], planePoint, planeNormal);
                    }
                    else if (vertice2 < 0)
                    {
                        newVertex1 = findNewVertex((Vector3)vertices[i], (Vector3)vertices[i + 1] - (Vector3)vertices[i], planePoint, planeNormal);
                        newVertex2 = findNewVertex((Vector3)vertices[i + 2], (Vector3)vertices[i + 1] - (Vector3)vertices[i + 2], planePoint, planeNormal);
                    }
                    else if (vertice3 < 0)
                    {
                        newVertex1 = findNewVertex((Vector3)vertices[i], (Vector3)vertices[i + 2] - (Vector3)vertices[i], planePoint, planeNormal);
                        newVertex2 = findNewVertex((Vector3)vertices[i + 1], (Vector3)vertices[i + 2] - (Vector3)vertices[i + 1], planePoint, planeNormal);
                    }
                }
                else
                {
                    if (vertice1 > 0)
                    {
                        newVertex1 = findNewVertex((Vector3)vertices[i + 1], (Vector3)vertices[i] - (Vector3)vertices[i + 1], planePoint, planeNormal);
                        newVertex2 = findNewVertex((Vector3)vertices[i + 2], (Vector3)vertices[i] - (Vector3)vertices[i + 2], planePoint, planeNormal);
                    }
                    else if (vertice2 > 0)
                    {
                        newVertex1 = findNewVertex((Vector3)vertices[i], (Vector3)vertices[i + 1] - (Vector3)vertices[i], planePoint, planeNormal);
                        newVertex2 = findNewVertex((Vector3)vertices[i + 2], (Vector3)vertices[i + 1] - (Vector3)vertices[i + 2], planePoint, planeNormal);
                    }
                    else if (vertice3 > 0)
                    {
                        newVertex1 = findNewVertex((Vector3)vertices[i], (Vector3)vertices[i + 2] - (Vector3)vertices[i], planePoint, planeNormal);
                        newVertex2 = findNewVertex((Vector3)vertices[i + 1], (Vector3)vertices[i + 2] - (Vector3)vertices[i + 1], planePoint, planeNormal);
                    }
                }

            }
            index += 3;
        }*/
    }

    Vector3 findNewVertex(Vector3 linePoint, Vector3 line, Vector3 planePoint, Vector3 planeNormal)
    {
        float d = Vector3.Dot(planePoint - linePoint, planeNormal) / Vector3.Dot(line, planeNormal);
        return d * line + linePoint;
    }
}
