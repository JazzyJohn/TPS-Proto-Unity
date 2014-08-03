using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class TurretSkill : SkillBehaviour
{
	public string turret;

	protected override void ActualUse(Vector3 target){
        if (foxView.isMine)
        {
            Pawn _turret =NetworkController.Instance.PawnSpawnRequest(turret, target + Vector3.up * 2, owner.myTransform.rotation, false,new int[0]).GetComponent<Pawn>();
            _turret.SetTeam(owner.team);
            _turret.player = owner.player;
        }
	}

}