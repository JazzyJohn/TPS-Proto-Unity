using UnityEngine;
using System.Collections;

public class InstantHealReward : ActivateReward
{

    public override int Activate(Pawn pawn)
    {
     
        pawn.Heal(10000f, pawn.gameObject);
        GA.API.Design.NewEvent("Game:Reward:Activate:HealSkill" );
        return base.Activate(pawn);
    }
}
