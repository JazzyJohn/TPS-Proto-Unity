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
                Destroy(gameObject, 0.1f);
                return;
            }
            //Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);

        }
        obj.Damage(damage, owner);
        //Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);
        Destroy(gameObject, 0.1f);
    }


}
