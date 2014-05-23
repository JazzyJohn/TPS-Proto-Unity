using UnityEngine;
using System.Collections;

public class HTHWeapon : BaseWeapon {

	public int attackId;

	void Start()
	{
		owner = GetComponent<AlphaDogPawn>();
	}

	public override bool CanShoot (){

		return true;
	}
	

}
