using UnityEngine;
using System.Collections;

// Пример модицикации класса
public class mSkillHealNear : basicSkill
{
	void Start () 
    {
        cooldownSkill.x = cooldownSkill.y = SkillConstants.instance.medicSkillStat[1, levelSkill, 1];
        cooldownOnChange = SkillConstants.instance.cooldownOnChange;
        isPassive = true;
    }

    public override bool UseSkill()
    {
        print("HealNearSkill");
        return true;
    }
}
