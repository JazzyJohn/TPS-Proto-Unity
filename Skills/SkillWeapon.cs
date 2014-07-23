using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class SkillWeapon : BaseWeapon{

	public SkillBehaviour skill;
	
	
	public override void AttachWeapon(Transform weaponSlot,Vector3 Offset, Quaternion weaponRotator,Pawn inowner){
		skill.Init(inonwer);
		base.AttachWeapon(weaponSlot,Offset, weaponRotator,inowner);
		
	}
	/// <summary>
    /// Hit logic for 
	protected override void HitEffect(RaycastHit hitInfo){
			Pawn target =hitInfo.collider.GetComponent<Pawn>();
			switch(skill.type){
				case TargetType.SELF:
				case TargetType.GROUPOFPAWN_BYSELF:	
					skill.Use(owner);	
				break;
				case TargetType.PAWN:
				case TargetType.GROUPOFPAWN_BYPAWN:
					skill.Use(target);		
				break;
				case TargetType.POINT:
				case TargetType.GROUPOFPAWN_BYPOINT:
						skill.Use(hitInfo.point);				
				break;				
			}
	}
}