using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AiGraphTransition : MonoBehaviour {

	public AIGraphState EnterState;
	public AIGraphState ExitState;
	//добавить список условий
	public enum AnchorX {left=0, center, right}
	public enum AnchorY {bottom=0, center, top}
	private Vector3 startVector;
	private Vector3 finishVector;
	public bool maketransition = false;

	[HideInInspector] public UISprite usp;
	// Use this for initialization
	void Start () {
	
	}
	
	bool CheckCurrentTransition()
	{
		return GUIAI.Instance.CurrentTransition == this;
	}
	
	// Update is called once per frame
	void Update () {
		if (usp) {usp.color = new Color32(255, 255, 255, (byte) (CheckCurrentTransition()?255:160));} else {usp = this.GetComponent<UISprite>();}
		
		UpdatePosition();
	}

	// Возвращает координаты узла
	Vector3 GetAnchorVector(UISprite sp, AnchorX ancX, AnchorY ancY)
	{
		Vector3 result = sp.transform.localPosition;
		result.x += (ancX == AnchorX.left ? -sp.width/2 : (ancX == AnchorX.right ? sp.width/2:0));
		result.y += (ancY == AnchorY.bottom ? -sp.height/2 : (ancY == AnchorY.top ? sp.height/2:0));
		return result;
	}

	// возвращает ближаший угол к данной точке
	Vector3 GetCloserPoint (UISprite sp, Vector3 center)
	{
		List<Vector3> vectors = new List<Vector3>();
		vectors.Add(GetAnchorVector(sp, AnchorX.left, AnchorY.bottom));
		vectors.Add(GetAnchorVector(sp, AnchorX.center, AnchorY.bottom));
		vectors.Add(GetAnchorVector(sp, AnchorX.right, AnchorY.bottom));
		vectors.Add(GetAnchorVector(sp, AnchorX.left, AnchorY.center));
		vectors.Add(GetAnchorVector(sp, AnchorX.right, AnchorY.center));
		vectors.Add(GetAnchorVector(sp, AnchorX.left, AnchorY.top));
		vectors.Add(GetAnchorVector(sp, AnchorX.center, AnchorY.top));
		vectors.Add(GetAnchorVector(sp, AnchorX.right, AnchorY.top));
		Vector3 result=sp.transform.localPosition;
		float distance = Vector3.Distance(result, center);
		foreach (Vector3 v3 in vectors)
		{
			if (distance > Vector3.Distance(center, v3)) 
			{
				result = v3;
				distance = Vector3.Distance(center, v3);
			}
		}
		return result;
	}

	// Обновляем положение и угол наклона стрелки
	void UpdatePosition ()
	{
		if (ExitState&&(EnterState||maketransition))
		{ 
			if (maketransition)
			{
				finishVector = new Vector3(Input.mousePosition.x-Screen.width/2, Input.mousePosition.y-Screen.height/2,0f);
				startVector = GetCloserPoint (ExitState.GetComponent<UISprite>(), finishVector);
			}
			else
			{
				startVector = GetCloserPoint (ExitState.GetComponent<UISprite>(), EnterState.transform.localPosition);
				finishVector = GetCloserPoint (EnterState.GetComponent<UISprite>(), startVector);
			}
			transform.localPosition = new Vector3(startVector.x - (startVector.x - finishVector.x)/2f, startVector.y - (startVector.y - finishVector.y)/2f,0f);
			this.GetComponent<UISprite>().width =(int) Vector3.Distance(startVector, finishVector);
			float grad = (Mathf.Acos((startVector.x - finishVector.x)/Vector3.Distance(startVector, finishVector))*60f);
			transform.localRotation = Quaternion.Euler(0f, 0f,  (startVector.y>finishVector.y?grad:360f-grad));

		}
	}

	void OnClick ()
	{
		GUIAI.Instance.LastClick = GUIAI.ClickType.transition;
		GUIAI.Instance.ChangeCurrentTransition (this);
	}
}
