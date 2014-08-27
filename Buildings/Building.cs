﻿using UnityEngine;
using System.Collections;

public class Building : DamagebleObject {

    public Player player;

    public int team;

    public float startHealth;

    public ShowOnGuiComponent onGui;
    protected void Awake()
    {
        health = startHealth;
        onGui = GetComponent<ShowOnGuiComponent>();
		foxView= GetCompoennt<FoxView>();
    }

    public void SetOwner(Player p)
    {
        team = p.team;
        player = p;
    }

    public override void KillIt(GameObject killer)
    {
        RequestKillMe();
    }
    void Update()
    {
        if (destructableObject) { 
            onGui.SetTitle(health.ToString("0"));
        }
    }
}
