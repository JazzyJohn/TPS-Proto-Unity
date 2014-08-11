using UnityEngine;
using System.Collections;
using nstuff.juggerfall.extension.models;

public class JuggerWall : DamagebleObject {

    public int team;

    public float startHealth;

    SimpleDestroyableModel model;

    void Awake()
    {
        foxView = GetComponent<FoxView>();

        health = startHealth;

        model = new SimpleDestroyableModel();
    }

    public void StartBase(){



        foxView.UpdateSimpleDestroyableObject(model);
    }

    public SimpleDestroyableModel GetModel()
    {
        model.health = health;
        model.id  = foxView.viewID;
        return model;
    }

    public override void Damage(BaseDamage damage, GameObject killer)
    {
        if (destructableObject)
        {
            BattleJugger killerPawn = killer.GetComponent<BattleJugger>();
            if (killerPawn != null)
            {
                if (killerPawn.team == team)
                {
                    return;
                }
                if (!killerPawn.foxView.isMine)
                {
                    Debug.Log("Not Mine" + killerPawn);
                    return;
                }
            }
            else
            {
                return;
            }
        }
        //NetworkController.Instance.BaseDamageRequest(team,(int)damage.Damage);
        base.Damage(damage, killer);
        Debug.Log("wall health" + health);
        if(health>0){
            foxView.UpdateSimpleDestroyableObject(model);
        }
    }
    public override void KillIt(GameObject killer){
        RequestKillMe();
		

	}

}
