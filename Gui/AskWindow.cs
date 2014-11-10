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
	
		panel.alpha = 0.0f;
		MainMenu.CamMove.RideTo(backPosition);
        action();
	}
	
	
	public void Decline(){
		panel.alpha = 0.0f;
		MainMenu.CamMove.RideTo(backPosition);
	}
	
}