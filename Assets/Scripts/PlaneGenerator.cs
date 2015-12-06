using UnityEngine;

public class PlaneGenerator : MonoBehaviour
{
	public int casts;
    public delegate void Generate(Plane plane, Vector3 start, Vector3 end, int casts);
    public static event Generate OnGeneration;

    public bool showDrawnLines;
    public bool debug;

    public GameObject testObj1;
    public GameObject testObj2;

    //2 points on near clipping plane
    public Vector3 startPoint;
    public Vector3 endPoint;
    //point at camera depth;
    public Vector3 camPoint;

    public Vector3 normal;
    public float d;
    public Plane plane;

    private Renderer rend;

    // Use this for initialization
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MenuController.Paused)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            startPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 0.5f);
            startPoint = Camera.main.ScreenToWorldPoint(startPoint);
            endPoint = startPoint;

        }

        if (Input.GetMouseButton(0))
        {
            endPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 0.5f);
            endPoint = Camera.main.ScreenToWorldPoint(endPoint);

        }

        if (endPoint != startPoint)
        {
            rend.enabled = true;
            var v3 = endPoint - startPoint;
            transform.position = startPoint + (v3) / 2.0f;
            transform.localScale = new Vector3(transform.localScale.x, v3.magnitude / 2.0f, transform.localScale.z);
            transform.rotation = Quaternion.FromToRotation(Vector3.up, v3);
        }

        if (Input.GetMouseButtonUp(0))
        {
            createPlane();
            //next step will hide cylinder
            endPoint = startPoint;
            rend.enabled = false;
        }
    }

    void createPlane()
    {
        camPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        camPoint = Camera.main.ScreenToWorldPoint(camPoint);
        Vector3 v1 = startPoint - endPoint;
        Vector3 v2 = camPoint - endPoint;

        //find normal
        normal = Vector3.Cross(v1, v2);
        normal = Vector3.Normalize(normal);
        //solve for d
        d = -normal.x * endPoint.x - normal.y * endPoint.y - normal.z * endPoint.z;

        plane = new Plane(normal, d);
        if (debug)
        {
            test();
        }

		Vector3 line = endPoint - startPoint;
		line = line / casts;
		for (int i = 0; i < casts; i++) {
			RaycastHit hit;
			Vector3 castPoint = startPoint + i*line;
			Vector3 direction = castPoint - Camera.main.transform.position;

			if (Physics.Raycast(Camera.main.transform.position, direction, out hit)){
                hit.transform.SendMessage("HitByRay");
            }
		}

        if (OnGeneration != null)
            OnGeneration(plane, startPoint, endPoint, casts);
    }

    void test()
    {
        Vector3 pt1 = testObj1.transform.position;
        Vector3 pt2 = testObj2.transform.position;
        Debug.Log(plane.SameSide(pt1, pt2));
    }

}