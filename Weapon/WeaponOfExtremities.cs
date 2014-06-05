using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponOfExtremities : MonoBehaviour {

	class ShootData{
		public double timeShoot;
		public Quaternion  direction;
		public Vector3 position;
		public void PhotonSerialization(PhotonStream stream){
			stream.SendNext (timeShoot);
			
			stream.SendNext( direction);
			ServerHolder.WriteVectorToShort (stream, position);
		}
		public void PhotonDeserialization(PhotonStream stream){
			timeShoot = (double)stream.ReceiveNext ();
			direction = (Quaternion)stream.ReceiveNext ();
			position = ServerHolder.ReadVectorFromShort (stream);
		}
	}

	private Queue<ShootData> shootsToSend = new Queue<ShootData>();
	
	private Queue<ShootData> shootsToSpawn = new Queue<ShootData>();

	public Pawn owner;
	
	public Transform curTransform;

	private bool shootAfterReload;

	public List<HTHHitter> Weapon = new List<HTHHitter>();
	
	public float WeaponDistance;

	// Use this for initialization
	void Start () 
	{


		owner = GetComponent<Pawn>();

		for (int i = 0; i < this.GetComponentsInChildren<HTHHitter>().Length; i++)
		{
			Weapon.Add(this.GetComponentsInChildren<HTHHitter>()[i]);
			Weapon[i].Luke_I_am_your_father(this, owner);
		}
	}

	
	// Update is called once per frame
	void Update () 
	{
		foreach(HTHHitter Attack in Weapon)
		{
			if (Attack.isKick) 
			{
				//Attack.isKick=false;
				if(Attack.timer<=0)
				{
					Kick(Attack);
					Attack.timer = Attack.KDTime;
				}
			}
		}
	}

	public virtual void StartKick(HTHHitter Attack)
	{
		Attack.isKick = true;
	}

	public virtual void StopKick()
	{
		foreach(HTHHitter Attack in Weapon)
		{
			Attack.StopKick();
					
		}
	}

	void Kick(HTHHitter Attack)
	{
		if (!Attack.CanShoot ()) 
		{
			return;		
		}

		switch (Attack.AttakType) 
		{
		case HTHHitter.KickType.Aim:
			owner.animator.StartAttackAnim(Attack.NameAttack);
			break;
		}
	}
}
