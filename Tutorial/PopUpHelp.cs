using UnityEngine;
using System.Collections;

public class PopUpHelp : MonoBehaviour {

	public bool showText;
	
	public bool showObject;
	
	public string text;
	
	public UILabel tutorialText;
	
	public GameObject helpMark;
	
	
	
	void OnTriggerEnter(Collider other) {
        Pawn pawn = other.GetComponent<Pawn>();
		if(pawn!=null&&!pawn.isAi&&pawn.player!=null&&pawn.player.isMine){
			TurnOn();	
		}
    }
	
	void OnTriggerExit(Collider other) {
        Pawn pawn = other.GetComponent<Pawn>();
		if(pawn!=null&&!pawn.isAi&&pawn.player!=null&&pawn.player.isMine){
			TurnOff();	
		}
    }
	void TurnOn(){
		if(showText){
			if(tutorialText==null){
				tutorialText =  FindObjectOfType<PlayerHudNgui>().tutorialText;
			}
			tutorialText.text = text;
			tutorialText.alpha= 1.0f;
		}
		if(showObject){
			helpMark.SetActive(true);
		}
	}
	void TurnOff(){
		if(showText){
			if(tutorialText==null){
				tutorialText =  FindObjectOfType<PlayerHudNgui>().tutorialText;
			}
			tutorialText.text = "";
			tutorialText.alpha= 0.0f;
		}
		if(showObject){
			helpMark.SetActive(false);
		}
	}
}