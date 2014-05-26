using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AIGraphState : MonoBehaviour {

	public enum TypeState {Strategic=0, Tactic};
	public TypeState type;
	public string NameState;
	public string DescState;
	public UILabel NameStateLabel;
	public UILabel DescStateLabel;

	
	public List<AiGraphTransition> InputTransitions = new List<AiGraphTransition>();
	public List<AiGraphTransition> OutputTransitions = new List<AiGraphTransition>();

	[HideInInspector] public UISprite usp;
	// Use this for initialization
	void Start () {
	
	}

	bool CheckCurrentState()
	{
		return GUIAI.Instance.CurrentState == this;
	}
	
	// Update is called once per frame
	void Update () {
		NameStateLabel.text = NameState;
		DescStateLabel.text = DescState;
		if (usp) {usp.color = new Color32(255, 255, 255, (byte) (CheckCurrentState()?255:160));} else {usp = this.GetComponent<UISprite>();}

	}

	void OnClick ()
	{
		GUIAI.Instance.LastClick = GUIAI.ClickType.state;
		GUIAI.Instance.ChangeCurrentState (this);
	}
}
