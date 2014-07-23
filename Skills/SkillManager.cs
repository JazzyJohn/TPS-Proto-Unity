using UnityEngine;
using System.Collections;

public class SkillManager : MonoBehaviour 
{
	SkillBehaviour[] allskill; 
	void Awake(){
		Pawn owner=  GetComponentn<Pawn>();
		allskill =GetComponentsInChildren<SkillBehaviour>();
		foreach(SkillBehaviour skill in allskill){
			skill.Init(owner);
		}
		
	}
	
	public void ActivateSkill(int i){
		if(allskill.Lenght>i){
			allskill[i].Use();
		}
	}
	
	
}