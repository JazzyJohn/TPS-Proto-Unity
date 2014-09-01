using UnityEngine;
using System.Collections;

public class Building : DamagebleObject {

    public Player player;

    public int team;

    public float startHealth;

    public ShowOnGuiComponent onGui;
    protected void Awake()
    {
        health = startHealth;
        foxView= GetComponent<FoxView>();
    }

    public void SetOwner(Player p)
    {
        team = p.team;
        player = p;
        onGui.team = team;
    }

    public override void KillIt(GameObject killer)
    {
        RequestKillMe();
    }
    public override void Damage(BaseDamage damage, GameObject killer)
    {
        Pawn killerPawn = killer.GetComponent<Pawn>();
        if (killerPawn != null && killerPawn.team != 0 && killerPawn.team == team && !PlayerManager.instance.frendlyFire )
        {

            return;
        }
        base.Damage(damage, killer);
    }
    void Update()
    {
        if (destructableObject) { 
            onGui.SetTitle(health.ToString("0"));
        }
    }
}
