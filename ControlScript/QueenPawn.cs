using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class QueenPawn : Pawn {

	public void StartEggLay(){
        foxView.CustomAnimStart("Egg");
		animator.SetSome("Egg");		
	}

}