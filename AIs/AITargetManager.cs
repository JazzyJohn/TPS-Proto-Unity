using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AITargetManager{

	private static Dictionary<Pawn , List<Pawn> > allAttackers=new Dictionary<Pawn , List<Pawn> >();
	
	
	public static void Reload(){
		allAttackers.Clear();

	}	
	
	public static void DeadPawn(Pawn dead){
		if(allAttackers.ContainsKey(dead)){
            allAttackers.Remove(dead);
		}
	}
	public static void AddAttacker(Pawn target,Pawn attacker){
        if (target == null) return;
        if (!allAttackers.ContainsKey(target))
        {
			allAttackers[target] = new  List<Pawn>();
		}
		allAttackers[target].Add(attacker);
	}
	public static void RemoveAttacker(Pawn target,Pawn attacker){
        if (target == null) return;
        if (allAttackers.ContainsKey(target))
        {
			allAttackers[target].Remove(attacker);
		}
	}
	public static int GetAttackersCount(Pawn target){
        if (target == null) return 0;
        if (allAttackers.ContainsKey(target))
        {
			return allAttackers[target].Count;
		}
		return 0;
	}
	
}   