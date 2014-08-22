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

	private Pawn owner;
	
	void Awake(){
		owner=  GetComponent<Pawn>();
	
		foreach(SkillTrigger trigger in allSkills){
			trigger.skill.Init(owner);
		}
		
	}
	
	public bool IsActive(){
        if (activeSkill != null)
        {
            if (activeSkill.IsActivating())
            {
				return true;
			}else{
                activeSkill = null;
				return false;
			}
		}
		return false;
	}
	public void Use(SkillBehaviour skill,Pawn enemy){
		activeSkill=skill;
		switch(skill.type){
				case TargetType.SELF:
				case TargetType.GROUPOFPAWN_BYSELF:	
					 Use(owner);
                    break;
				case TargetType.PAWN:
				case TargetType.GROUPOFPAWN_BYPAWN:
					 Use(enemy);			
				break;
				case TargetType.POINT:
				case TargetType.GROUPOFPAWN_BYPOINT:
					Use(enemy.myTransform.position);	
				break;				
			}
	}
}