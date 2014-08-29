using UnityEngine;
using System.Collections;

public class SkillManager : MonoBehaviour 
{
	SkillBehaviour[] allskill; 
	void Awake(){
		Pawn owner=  GetComponent<Pawn>();
		allskill =GetComponentsInChildren<SkillBehaviour>();
		foreach(SkillBehaviour skill in allskill){
			skill.Init(owner);
		}
		
	}
	public SkillBehaviour GetSkill(int i){
		return allskill[i];
	}
	
	public void ActivateSkill(int i){
      
		if(allskill.Length>i){
			allskill[i].Use();
		}
	}
    public void DeActivateSkill(int i)
    {
        if (allskill.Length > i)
        {
            allskill[i].UnUse();
        }
    }
	public SkillBehaviour GetSkill(string name){
		foreach(SkillBehaviour skill in allskill){
			if(skill.name == name){
				return skill;
			}
		}
		return null;
	}
	
}