using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponOfExtremities : MonoBehaviour {

	
	public Pawn owner;

    public bool isKicking = false;

	private bool shootAfterReload;

	public List<HTHHitter> Weapon = new List<HTHHitter>();
	
	public float WeaponDistance;

	// Use this for initialization
	void Start () 
	{


		owner = GetComponent<Pawn>();
        HTHHitter[] hitter =  GetComponentsInChildren<HTHHitter>();
		for (int i = 0; i <hitter.Length; i++)
		{
            if (hitter[i] != null)
            {
                Weapon.Add(hitter[i]);

                Weapon[i].SetOwner(this, owner);
            }
		}
	}

	
	// Update is called once per frame
	void Update () 
	{
		foreach(HTHHitter Attack in Weapon)
		{
            if (Attack != null)
            {
                if (Attack.isKick)
                {
                    //Attack.isKick=false;
                    if (!Attack.IsPlaying())
                    {
                        Kick(Attack);

                    }
                }
            }
		}
	}

	public virtual bool StartKick(HTHHitter Attack)
	{
        if (isKicking) {
            return false;
        }
        if (!Attack.OnMove) {
            owner.StopMovement();
        }
      
		Kick(Attack);
        isKicking = true;
		return true;
	}
	
	public virtual void StopKick()
	{
        if (owner != null)
        {
            owner.StartMovement();
        }
      
		foreach(HTHHitter Attack in Weapon)
		{
            if (Attack != null)
            {
                Attack.StopKick();
            }
					
		}
        isKicking = false;
	}

	void Kick(HTHHitter Attack)
	{
	
		
		switch (Attack.AttakType) 
		{
		case HTHHitter.KickType.Aim:
			owner.animator.StartAttackAnim(Attack.NameAttack);
			break;
		}
       
		Attack.StartKick();
	}
    public virtual void KickFinish()
	{
		owner.KickFinish();
	}
}
