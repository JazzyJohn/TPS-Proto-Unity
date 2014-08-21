using UnityEngine;
using System.Collections;

public enum SkillTriggerType{ ENEMY_SEE, ENEMY_COUNT, HEALTH_LEFT,TIME_PASS, ALLY_KILL, ALLY_COUNT};
public class AISkillManager : MonoBehaviour 
{
	[System.Serializable]
	public class SkillTrigger
	{
		public SkillTriggerType type;
		public int count  =0;
		public SkillBehaviour skill;
	}
	
	public SkillTrigger[] allSkills;
		
	private SkillBehaviour activeSkill;
		
	public bool IsActive(){
		if(activeskill!=null){
			if(activeskill.Activate()){
				return true;
			}else{
				activeskill = null;
				return false;
			}
		}
		return false;
	}
	public void Use(SkillBehaviour skill,Pawn enemy){
	
	}
}