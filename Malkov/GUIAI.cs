using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GUIAI : Singleton<GUIAI> {


	public GameObject PopupMenu; // Основное контекстное меню (выпадает при нажатии на пустое место) [Добавление/удаление состояния]
	public GameObject PopupStateMenu; // Контекстное меню состояния (выпадает при нажатии на одно из состояний) [Добавление перехода]
	public GameObject PopupTransitionMenu; // Контекстное меню перехода (выпадает при нажатии на одно из состояний) [Удаление перехода]
	public GameObject CurrentPopupMenu;
	public UIInput InputTitle;
	public UIInput InputDesc;

	public Transform StatesRoot;
	public Transform TransitionsRoot;
	public UITable TransTable;
	public GameObject PrefabState;
	public GameObject PrefabTransition;
	public GameObject PrefabSubTrans;

	public enum ClickType {none, state, transition};
	public ClickType LastClick = ClickType.none;

	public List<AIGraphState> VisibleStates= new List<AIGraphState>();
	public List<AiGraphTransition> VisibleTransitions= new List<AiGraphTransition>();

	public AIGraphState CurrentState;
	public AiGraphTransition CurrentTransition;

	// Use this for initialization
	void Start () {
		PopupMenu.SetActive(false);
		PopupStateMenu.SetActive(false);
		VisibleStates.Clear();
		foreach (AIGraphState ags in this.GetComponentsInChildren<AIGraphState>()) {VisibleStates.Add(ags);}
		VisibleTransitions.Clear();
		foreach (AiGraphTransition agt in this.GetComponentsInChildren<AiGraphTransition>()) {VisibleTransitions.Add(agt);}

	}

	bool PointIntoRect(Vector3 point, UISprite rect)
	{
		return	(point.x >= rect.transform.localPosition.x-rect.width/2)&&
			(point.x <= rect.transform.localPosition.x+rect.width/2)&&
				(point.y >= rect.transform.localPosition.y-	rect.height/2)&&
				(point.y <= rect.transform.localPosition.y+rect.height/2);
	}

	// Проверка клавиш мыши
	public void CheckMenuButton()
	{
		GameObject CM=null;
		switch (LastClick)
		{
		case ClickType.none:
			CM = PopupMenu;
			break;
		case ClickType.state:
			CM = PopupStateMenu;
			break;
		case ClickType.transition:
			CM = PopupTransitionMenu;
			break;
		}
		Vector3 mousepos = new Vector3(Input.mousePosition.x-Screen.width/2, Input.mousePosition.y-Screen.height/2,0f);

		if (maketrans)
		{
			if (lastrightclick||CurrentState==VisibleTransitions[VisibleTransitions.Count-1].ExitState) 
			{
				DestroyObject(VisibleTransitions[VisibleTransitions.Count-1].gameObject);
				VisibleTransitions.RemoveAt(VisibleTransitions.Count-1);
			}
			else 
			{
				VisibleTransitions[VisibleTransitions.Count-1].EnterState = CurrentState;
				CurrentState.InputTransitions.Add(VisibleTransitions[VisibleTransitions.Count-1]);
				VisibleTransitions[VisibleTransitions.Count-1].ExitState.OutputTransitions.Add(VisibleTransitions[VisibleTransitions.Count-1]);
				foreach (AiGraphTransition agt in VisibleTransitions) {agt.maketransition = false;}
			}
			maketrans=false;
		}
		else
		{
			if (CurrentPopupMenu.activeSelf&&(!PointIntoRect(mousepos, CurrentPopupMenu.GetComponent<UISprite>()))) 
			{
				if (CurrentPopupMenu) {CurrentPopupMenu.SetActive(false);}
				CurrentPopupMenu = CM;

			}
			if (!CurrentPopupMenu.activeSelf)
			{
				if (CurrentPopupMenu != CM) 
				{
					if (CurrentPopupMenu) {CurrentPopupMenu.SetActive(false);}
					CurrentPopupMenu = CM;
				}
				CurrentPopupMenu.transform.localPosition = mousepos;
				CurrentPopupMenu.SetActive(lastrightclick);
			}
		}
	}

	public void UpdateAIState ()
	{
		CurrentState.NameState = InputTitle.value;
		CurrentState.DescState = InputDesc.value;
		print ("Update State");
	}

	private List<GameObject> tempGOtable = new List<GameObject>();

	public void UpdateGUIStateWindow ()
	{
		if (CurrentState)
		{
			InputTitle.value = CurrentState.NameState;
			InputDesc.value = CurrentState.DescState;
			foreach (GameObject go in tempGOtable) {DestroyObject(go);}
			tempGOtable.Clear();
			if (CurrentState.OutputTransitions.Count>0)
			{
				for (int i = 0; i <CurrentState.OutputTransitions.Count; i++)
				{
					GameObject NewSubTransition = Instantiate (PrefabSubTrans) as GameObject;
					NewSubTransition.gameObject.SetActive(true);
					NewSubTransition.transform.parent = TransTable.transform;
					NewSubTransition.transform.localPosition = TransTable.transform.localPosition;
					NewSubTransition.transform.localScale = Vector3.one;
					NewSubTransition.GetComponent<UIForwardEvents>().target = CurrentState.OutputTransitions[i].gameObject;
					NewSubTransition.name = "Sub Transition "+i;
					NewSubTransition.GetComponent<UILabel>().text = "Transition "+i;
					tempGOtable.Add(NewSubTransition);
				}
				TransTable.Reposition();
			}
		}
		else
		{
			InputTitle.value = "";
			InputDesc.value = "";
			foreach (GameObject go in tempGOtable) {DestroyObject(go);}
			tempGOtable.Clear();
		}
	}

	private bool lastrightclick=false;
	// Update is called once per frame
	void Update () {
		//if (freepress) {CheckMenuButton(PopupMenu);}
		lastrightclick = Input.GetMouseButton(1);
	}

	private bool maketrans = false;
	public void AddTransition ()
	{
		print ("AddTransition");
		maketrans = true;
		GameObject NewTransition = Instantiate (PrefabTransition) as GameObject;
		NewTransition.gameObject.SetActive(true);
		NewTransition.transform.parent = TransitionsRoot;
		NewTransition.transform.localPosition = PopupStateMenu.transform.localPosition;
		NewTransition.transform.localScale = Vector3.one;
		NewTransition.GetComponent<AiGraphTransition>().ExitState = CurrentState;
		//UpdateGUIStateWindow ();
		PopupStateMenu.SetActive(false);
		VisibleTransitions.Add(NewTransition.GetComponent<AiGraphTransition>());
		NewTransition.name = "Transition "+VisibleTransitions.Count.ToString();
		NewTransition.GetComponent<AiGraphTransition>().maketransition=true;

	}

	public void AddState()
	{
		print ("AddState");
		GameObject NewState = Instantiate (PrefabState) as GameObject;
		NewState.gameObject.SetActive(true);
		NewState.transform.parent = StatesRoot;
		NewState.transform.localPosition = PopupMenu.transform.localPosition;
		NewState.transform.localScale = Vector3.one;
		CurrentState = NewState.GetComponent<AIGraphState>();
		UpdateGUIStateWindow ();
		PopupMenu.SetActive(false);
		VisibleStates.Add(NewState.GetComponent<AIGraphState>());
		NewState.name = "State "+VisibleStates.Count.ToString();
	}

	public void DeleteState(AIGraphState state)
	{
		print ("DeleteState");
		while (state.InputTransitions.Count>0)  {DeleteTransition(state.InputTransitions[0]);}
		while (state.OutputTransitions.Count>0)  {DeleteTransition(state.OutputTransitions[0]);}
		VisibleStates.Remove(state);
		DestroyObject(state.gameObject);
	}

	public void DeleteCurrentState ()
	{
		print ("DeleteCurrentState");
		DeleteState (CurrentState);
		CurrentPopupMenu.SetActive(false);
		CurrentState = null;
		UpdateGUIStateWindow ();	
	}

	void DeleteTransition (AiGraphTransition trans)
	{
		trans.EnterState.InputTransitions.Remove(trans);
		trans.ExitState.OutputTransitions.Remove(trans);
		VisibleTransitions.Remove(trans);
		DestroyObject(trans.gameObject);
	}

	public void DeleteCurrentTransition ()
	{
		print ("DeleteTransition");
		DeleteTransition (CurrentTransition);
		CurrentPopupMenu.SetActive(false);
		CurrentTransition = null;
		UpdateGUIStateWindow ();		
	}

	public void ChangeCurrentState (AIGraphState newstate)
	{
		CurrentState = newstate;
		UpdateGUIStateWindow ();
		CheckMenuButton ();

	}

	public void ChangeCurrentTransition (AiGraphTransition newtrans)
	{
		CurrentTransition = newtrans;
		ChangeCurrentState (newtrans.ExitState);
	}

	void OnClick ()
	{
		LastClick = ClickType.none;
		CheckMenuButton();
		//AI.ConditionType
	}
}
