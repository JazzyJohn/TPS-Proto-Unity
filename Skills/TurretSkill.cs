using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TargetType{PAWN,POINT,GROUPOFPAWN_BYPAWN,GROUPOFPAWN_BYPOINT  }

public class TurretSkill : SkillBehaviour
{
	public GameObject turret;

	protected override void ActualUse(Vector3 target){
	
		Pawn _turret =  PhotonNetwork.Instantiate (GameObject,	target+ Vector3.up*2, owner.myTransform.rotation, 0).GetComponent<Pawn>();
		_turret.SetTeam(owner.team);
	}

}