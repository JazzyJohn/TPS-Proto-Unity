using UnityEngine;
using System.Collections;

public class MoveNGUICamer : MonoBehaviour {

	Camera cam;
	Transform ThisObject;
	public float MaxAndMinRotate = 3;

	void Awake()
	{
		ThisObject = this.transform;
		cam = GetComponent<Camera>();
	}

	// Use this for initialization
	void Start () 
	{
	
	}

	Vector3 posMouse;
	// Update is called once per frame
	void Update ()  //
	{
		posMouse = cam.ScreenToViewportPoint(Input.mousePosition);
		ThisObject.localEulerAngles = new Vector3(((posMouse.y*2)-1)*-MaxAndMinRotate,((posMouse.x*2)-1)*MaxAndMinRotate, 0f); 
	}
}
