using UnityEngine;
using System.Collections;

public class AIDirector_Expedition : AIDirector {

    public Pawn VipPawn;

    public override void StartDirector()
    {
        Debug.Log("Director Start");
        VipPawn.StartPawn();
        base.StartDirector();

    }
	

}
