using UnityEngine;
using System.Collections;

public class SmallAsk : MonoBehaviour {

    public UILabel text;

    public UIPanel panel;

    public ConfirmAction action;

   

    public void Accept()
    {

        panel.alpha = 0.0f;
    
        action();
    }


    public void Decline()
    {
        panel.alpha = 0.0f;

    }
	
}
