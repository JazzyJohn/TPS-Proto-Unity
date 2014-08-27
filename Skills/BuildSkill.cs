using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;



public class BuildSkill : SkillBehaviour
{
	public string buildingPrefab;

    private Building building;

	protected override void ActualUse(Vector3 target){
        if (foxView.isMine)
        {

            if (building != null)
            {
                building.RequestKillMe();
            }
            building = NetworkController.Instance.SimplePrefabSpawn(buildingPrefab, target , owner.myTransform.rotation, new SFSObject(), false, NetworkController.PREFABTYPE.PLAYERBUILDING).GetComponent<Building>();
            building.SetOwner(owner.player);
            
        }
	}

}