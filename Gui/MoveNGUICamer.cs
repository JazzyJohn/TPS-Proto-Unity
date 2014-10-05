using UnityEngine;
using System.Collections;

public class MoveNGUICamer : MonoBehaviour {

	public Camera cam;
    Transform cameraTransform;
	Transform ThisObject;
	UIWidget Panel;
	public float MaxAndMinRotate = 3;
	public bool inverse;

    Vector3 startPosition;
    public Vector3 oneClickDifference;
    Vector3 targetPosition;
    int curPoint = 0;

	void Awake()
	{
		ThisObject = this.transform;
		Panel = GetComponent<UIWidget>();
        cameraTransform = cam.transform;
        startPosition = cameraTransform.localPosition;
        targetPosition = startPosition;
	}

	// Use this for initialization
	void Start () 
	{
	
	}
     void OneClickMore(){
        targetPosition += oneClickDifference;
        curPoint++;
    }
    void OneClickLess()
    {
        targetPosition -= oneClickDifference;
        curPoint--;
    }

    public void RideTo(int i){
        if (curPoint < i) {
            while (curPoint != i)
            {
                OneClickMore();
            }
        }
        if (curPoint > i)
        {
            while (curPoint != i)
            {
                OneClickLess();
            }
        }
    }
    public void Reset()
    {
        targetPosition = startPosition;
        curPoint = 0;
    }
	Vector3 posMouse;
	// Update is called once per frame
	void Update ()  //
	{

        if ((targetPosition - cameraTransform.localPosition).sqrMagnitude > 1)
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, Time.deltaTime);
        }
		if(Panel.alpha != 1)
			return;

		posMouse = cam.ScreenToViewportPoint(Input.mousePosition);

		float Z = MaxAndMinRotate;
		if(inverse)
			Z = -Z;
		ThisObject.localEulerAngles = new Vector3(((posMouse.y*2)-1)*Z,((posMouse.x*2)-1)*-Z, 0f); 
	}
}
