using UnityEngine;
using System;
using System.Collections;

public class JetPackController : MonoBehaviour {
	public 	ParticleSystem leftExhaust,
						   middleExhaust,
						   rightExhaust;
						   
	public void StartLeft(){
		StopAll();
		if(!leftExhaust.isPlaying){
			leftExhaust.Play();
		}
	}
	public void StartRight(){
		StopAll();
		if(!rightExhaust.isPlaying){
			rightExhaust.Play();
		}
	}
	public void StartMiddle(){
		StopAll();
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
		leftExhaust.Pause();
		middleExhaust.Pause();
		rightExhaust.Pause();
	}
}