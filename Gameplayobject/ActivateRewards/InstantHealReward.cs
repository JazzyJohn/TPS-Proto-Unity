using UnityEngine;
using System.Collections;

public class InstantHealReward : ActivateReward
{

    public override void Activate(Pawn pawn)
    {
        base.Activate(pawn);
        pawn.Heal(10000f, pawn.gameObject);        
    }
}
