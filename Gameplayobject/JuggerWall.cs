using UnityEngine;
using System.Collections;
using nstuff.juggerfall.extension.models;

public class JuggerWall : DamagebleObject {

    public int team;

    public float startHealth;

    SimpleDestroyableModel model;

    private bool isOpen;

    public GameObject wall;

    void Awake()
    {
        foxView = GetComponent<FoxView>();

        health = startHealth;

        model = new SimpleDestroyableModel();
    }

    public void StartWall(){



        foxView.UpdateSimpleDestroyableObject(GetModel());
    }

    public SimpleDestroyableModel GetModel()
    {
        model.health = health;
        model.id  = foxView.viewID;
        return model;
    }
    public override void SetHealth(float p)
    {
        health = p;
    }

    public override void Damage(BaseDamage damage, GameObject killer)
    {
        Debug.Log("damage");
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
    void Update()
    {
        if (isOpen)
        {

            isOpen = false;
            if (wall.activeSelf)
            {
                wall.SetActive(false);
            }
        }
        else
        {
            if (!wall.activeSelf)
            {
                wall.SetActive(true);
            }
        }

    }
    public void OnTriggerStay(Collider other)
    {

        BattleJugger pawn = other.GetComponent<BattleJugger>();
        if ( pawn!=null&&pawn.team==team)
        {
            isOpen=true;
        }
    }
}
