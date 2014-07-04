using UnityEngine;
using System.Collections;

public class AIDirector_Expedition : AIDirector {

    public Pawn VipPawn;

    public override void StartDirector()
    {

        VipPawn.StartPawn();
        base.StartDirector();

    }
	

}
