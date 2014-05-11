using UnityEngine;
using System;
using System.Collections;

public class JetPackController : MonoBehaviour {
	public 	ParticleSystem leftExhaust,
						   middleExhaust,
						   rightExhaust;
						   
	public void StartLeft(){
		//StopAll();
		rightExhaust.Stop ();
		middleExhaust.Stop ();
		if(!leftExhaust.isPlaying){
			leftExhaust.Play();
		}
	}
	public void StartRight(){
		//StopAll();
		leftExhaust.Stop ();
		middleExhaust.Stop ();

		if(!rightExhaust.isPlaying){
			rightExhaust.Play();
		}
	}
	public void StartMiddle(){
		//StopAll();
		leftExhaust.Stop ();
		rightExhaust.Stop ();

		if(!middleExhaust.isPlaying){
			middleExhaust.Play();
		}
	}
	public void StartAll(){
		if(!leftExhaust.isPlaying){
			leftExhaust.Play();
		}
		if(!middleExhaust.isPlaying){
			middleExhaust.Play();
		}
		if(!rightExhaust.isPlaying){
			rightExhaust.Play();
		}
	}
	public void StopAll(){
		leftExhaust.Stop();
		middleExhaust.Stop();
		rightExhaust.Stop();
	}
}