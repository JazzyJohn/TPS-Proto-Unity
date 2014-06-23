using UnityEngine;
using System.Collections;

public class HealingProjectile : BaseProjectile {
    public override void DamageLogic(DamagebleObject obj)
    {
        Pawn pawn = (Pawn)obj;
        if (pawn != null)
        {
            if (pawn.team == owner.GetComponent<Pawn>().team)
            {
                pawn.Heal(damage.Damage, owner);
               
                return;
            }
            //Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);

        }
		base.DamageLogic(obj);
        //Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);

    }


}
