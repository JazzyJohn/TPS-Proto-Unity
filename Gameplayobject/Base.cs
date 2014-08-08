using UnityEngine;
using System.Collections;
using nstuff.juggerfall.extension.models;

public class Base : DamagebleObject {

    public int team;

    public float startHealth;

    void Awake()
    {
        health = startHealth;
    }

    public void StartBase(){

        BaseModel  model = new BaseModel();
        model.health = health;
        model.team  = team;
        NetworkController.Instance.BaseSpawnedRequest(model);
    }


    public override void Damage(BaseDamage damage, GameObject killer)
    {
        if (destructableObject)
        {
            Pawn killerPawn = killer.GetComponent<Pawn>();
            if (killerPawn != null)
            {
                if (killerPawn.team == team)
                {
                    return;
                }
                if (!killerPawn.foxView.isMine)
                {
                    return;
                }
            }
        }
        NetworkController.Instance.BaseDamageRequest(team,(int)damage.Damage);
        base.Damage(damage, killer);
    }
}
