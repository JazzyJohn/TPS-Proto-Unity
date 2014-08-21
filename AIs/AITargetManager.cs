using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AITargetManаger{

	private static Dictionary<Pawn , List<Pawn> > allAttackers;
	
	
	public static void Reload(){
		allAttackers = new Dictionary<Pawn , List<Pawn> >();

	}	
	
	public static void DeadPawn(Pawn dead){
		if(allAttackers.Contains(dead)){
			all.Remove(dead);
		}
	}
	public static void AddAttacker(Pawn target,Pawn attacker){
		if(!allAttackers.Contains(target)){
			allAttackers[target] = new  List<Pawn>();
		}
		allAttackers[target].Add(attacker);
	}
	public static void RemoveAttacker(Pawn target,Pawn attacker){
		if(allAttackers.Contains(target)){
			allAttackers[target].Remove(attacker);
		}
	}
	public static int GetAttackersCount(Pawn target){
		if(allAttackers.Contains(target)){
			return allAttackers[target].Count;
		}
		return 0;
	}
	
}