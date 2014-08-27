using UnityEngine;
using System.Collections;

public class DispenserHeal : Building
{
    public float healthRate;

    public float overHealMax;

    public GameObject go;

    void Awake() {
        base.Awake();
        go = gameObject;
    }
    void OnTriggerStay(Collider other)
    {
        Pawn pawn = other.GetComponent<Pawn>();
        if (pawn != null && pawn.foxView.isMine && !pawn.isAi)
        {
            if (pawn.IsMaxHP())
            {
                   if(pawn.team==team){
                       pawn.OverHeal(healthRate * Time.fixedDeltaTime,overHealMax);
                   }
            }
            else
            {
                pawn.Heal(healthRate * Time.fixedDeltaTime, go);
            }
        }
    }
}
