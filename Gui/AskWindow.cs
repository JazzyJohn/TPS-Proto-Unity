using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

delegate void ConfirmAction();

public class AskWindow  : MonoBehaviour{

	public MainMenuGUI MainMenu;
	
	public UILabel text;
	
	public UIPanel panel;
	
	public ConfirmAction action;
	
	public int backPosition;
	
	public void Accept(){
	
		panel.alpha = 0.0f;
		MainMenu.CamMove.RideTo(backPosition);
		ConfirmAction();
	}
	
	
	public void Decline(){
		panel.alpha = 0.0f;
		MainMenu.CamMove.RideTo(backPosition);
	}
	
}