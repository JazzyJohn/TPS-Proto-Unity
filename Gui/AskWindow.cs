using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void ConfirmAction();

public class AskWindow  : MonoBehaviour{

	public MainMenuGUI MainMenu;
	
	public UILabel text;
	
	public UIPanel panel;
	
	public ConfirmAction action;
	
	public int backPosition;
	
	public void Accept(){
        GA.API.Design.NewEvent("GUI:AskWindow:" + action.ToString()+":Yes", 1); 
		panel.alpha = 0.0f;
		MainMenu.CamMove.RideTo(backPosition);
        action();
	}
	
	
	public void Decline(){
        GA.API.Design.NewEvent("GUI:AskWindow:" + action.ToString()+":No", 1); 
		panel.alpha = 0.0f;
		MainMenu.CamMove.RideTo(backPosition);
	}
	
}