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
	public Skill GetSkill(string name){
		foreach(SkillBehaviour skill in allskill){
			if(skill.name == name){
				return skill;
			}
		}
		return null;
	}
	
}