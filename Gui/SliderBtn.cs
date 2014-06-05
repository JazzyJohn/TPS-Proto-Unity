using UnityEngine;
using System.Collections;

public class SliderBtn : MonoBehaviour {
	private int myNumber;
	
	private SlaiderPanel mainPanel;

	public void Init(int number, SlaiderPanel panel){
		myNumber = number;
		mainPanel= panel;
 
		
	}
	
	public void ChangeNews(){
		mainPanel.SetNews(myNumber);
	
	}



}