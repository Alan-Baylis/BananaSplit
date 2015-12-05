using UnityEngine;
using System.Collections;

public class Splitable : MonoBehaviour {

    Mesh mesh;



	// Use this for initialization
	void Start () {
       mesh  = gameObject.GetComponent<MeshFilter>().mesh;
		PlaneGenerator.OnGeneration += split;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void split(Plane plane){

		Debug.Log (plane.normal);
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

            if (side1 == side2 && side2 == side3 && side3 == true)
            {
                posTriangles.Add(triangles[i]);
                posTriangles.Add(triangles[i + 1]);
                posTriangles.Add(triangles[i + 2]);
            }
			else if (side1 == side2 && side2 == side3 && side3 == false)
			{
                negTriangles.Add(triangles[i]);
                negTriangles.Add(triangles[i + 1]);
                negTriangles.Add(triangles[i + 2]);
            }
            else
            {
                //find odd boolean value
                //find vertice with odd boolean value
                //find rays between odd vertice and other vertices
                //find intersection between rays and plane
                //add intersection to end of vertices array

            }

        }
		/*GameObject split1 = new GameObject();
		GameObject split2 = new GameObject();
		
		Mesh mesh1 = new Mesh();
		Mesh mesh2 = new Mesh();
		
		split1.AddComponent<MeshFilter>();
		split2.AddComponent<MeshFilter>();
		split1.GetComponent<MeshFilter>().mesh = mesh1;
		split2.GetComponent<MeshFilter>().mesh = mesh2;
		
		mesh1.vertices = vertices;
		mesh2.vertices = vertices;
		
		mesh1.uv = mesh.uv;
		mesh2.uv = mesh.uv;
		
		mesh1.triangles = posTriangles.ToArray(typeof(int)) as int[];
		mesh2.triangles = negTriangles.ToArray(typeof(int)) as int[];

		split1.AddComponent<MeshRenderer>();
		split2.AddComponent<MeshRenderer>();

		MeshRenderer mr = GetComponent<MeshRenderer> ();
		MeshRenderer mr1 = split1.GetComponent<MeshRenderer> ();
		MeshRenderer mr2 = split2.GetComponent<MeshRenderer> ();

		mr1.material = mr.material;
		mr2.material = mr.material;

		split1.AddComponent<Splitable>();
		split2.AddComponent<Splitable>();*/

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
