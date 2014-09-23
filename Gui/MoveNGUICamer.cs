using UnityEngine;
using System.Collections;

public class MoveNGUICamer : MonoBehaviour {

	public Camera cam;
	Transform ThisObject;
	UIWidget Panel;
	public float MaxAndMinRotate = 3;
	public bool inverse;

	void Awake()
	{
		ThisObject = this.transform;
		Panel = GetComponent<UIWidget>();
	}

	// Use this for initialization
	void Start () 
	{
	
	}

	Vector3 posMouse;
	// Update is called once per frame
	void Update ()  //
	{
		if(Panel.alpha != 1)
			return;

		posMouse = cam.ScreenToViewportPoint(Input.mousePosition);

		float Z = MaxAndMinRotate;
		if(inverse)
			Z = -Z;
		ThisObject.localEulerAngles = new Vector3(((posMouse.y*2)-1)*Z,((posMouse.x*2)-1)*-Z, 0f); 
	}
}
