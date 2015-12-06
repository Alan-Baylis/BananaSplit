using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectPlacer : MonoBehaviour {

	public GameObject pairedPlaneGenerator;
	public Dropdown dd;
	private MeshRenderer meshRenderer;


	// Use this for initialization
	void Start () {
		meshRenderer = GetComponent<MeshRenderer> ();
		dd.onValueChanged.AddListener( delegate {
			enable();
		});
	}
	
	// Update is called once per frame
	void Update () {
		if (!meshRenderer.enabled) {
			return;
		}
		Vector3 mousePoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 0.5f);
		mousePoint = Camera.main.ScreenToWorldPoint (mousePoint);
		Vector3 direction = mousePoint - Camera.main.transform.position;

		int layerMask = 1 << 8;
		RaycastHit hit;
		if (Physics.Raycast(Camera.main.transform.position, direction, out hit,1000,layerMask)){
			transform.position = hit.point + new Vector3(0, 0.01f, 0);
		}

		if (Input.GetMouseButtonDown (0)) {
			GameObject obj;
			switch (dd.value){
			case 0:
				//precaution only. should not be able to occur.
				disable ();
				return;
			case 1:
				obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
				break;
			case 2:
				obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				break;
			case 3:
				obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
				break;
			case 4:
				obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				break;
			default:
				obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				break;
			}
			obj.transform.position = transform.position + new Vector3(0, 2, 0);
			obj.AddComponent<Splitable>();
			obj.AddComponent<Rigidbody>();
			dd.value = 0;
			disable ();
		}

		if (Input.GetMouseButtonDown (1)) {
			disable ();
		}

	}

	void enable() {
		meshRenderer.enabled = true;
		pairedPlaneGenerator.SendMessage ("disable");
	}

	void disable() {
		pairedPlaneGenerator.SendMessage ("enable");
		meshRenderer.enabled = false;
	}
}
