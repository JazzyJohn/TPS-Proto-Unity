using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class TurretSkill : SkillBehaviour
{
    public string turret;

  
    protected override void ActualUse(Vector3 target)
    {
        if (foxView.isMine)
        {
         
            Pawn _turret = NetworkController.Instance.PawnSpawnRequest(turret, target + Vector3.up * 2, owner.myTransform.rotation, true, new int[0], false).GetComponent<Pawn>();
            _turret.SetTeam(owner.team);
            _turret.player = owner.player;
            if (owner.player.building != null)
            {
                owner.player.building.RequestKillMe();
            }
            owner.player.building = _turret;
        }
    }

}