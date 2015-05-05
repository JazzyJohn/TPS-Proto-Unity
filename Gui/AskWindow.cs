using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void ConfirmAction();

public class AskWindow  : MonoBehaviour{

	public MainMenuGUI MainMenu;
	
	public UILabel text;
	
	public UIRect panel;
	
	public ConfirmAction action;
	
	public int backPosition;

    public virtual void Accept()
    {
        GA.API.Design.NewEvent("GUI:AskWindow:" + action.ToString()+":Yes", 1); 
		panel.alpha = 0.0f;
		action();
		MainMenu.CamMove.RideTo(backPosition);
       
	}
	
	
	public virtual void Decline(){
        GA.API.Design.NewEvent("GUI:AskWindow:" + action.ToString()+":No", 1); 
		panel.alpha = 0.0f;
		MainMenu.CamMove.RideTo(backPosition);
	}


    public  void Show(string text)
    {
        panel.alpha = 1.0f;
        this.text.text = text;
    }
}