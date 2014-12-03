using UnityEngine;
using System.Collections;

public class SmallAsk : MonoBehaviour {

    public UILabel text;

    public UIPanel panel;

    public ConfirmAction action;

   

    public void Accept()
    {
        GA.API.Design.NewEvent("GUI:SmallAsk:" + action.ToString() + ":Yes", 1); 
        panel.alpha = 0.0f;
    
        action();
    }


    public void Decline()
    {
        GA.API.Design.NewEvent("GUI:SmallAsk:" + action.ToString() + ":No", 1); 
        panel.alpha = 0.0f;

    }
	
}
