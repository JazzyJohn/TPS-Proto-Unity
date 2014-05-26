using UnityEngine;
using System;
using System.Collections;

public class JetPackController : MonoBehaviour {
	public 	ParticleSystem leftExhaust,
						   middleExhaust,
						   rightExhaust;
	public AudioClip oneJetSound;
	public AudioClip fullPowerSound;
	private AudioSource aSource;
	protected soundControl sControl;//глобальный обьект контроллера звука

	public void Awake(){
		aSource = GetComponent<AudioSource> ();

		sControl = new soundControl (aSource);//создаем обьект контроллера звука
		sControl. stopSound();
		sControl.setLooped (true);
	}
	
	public void StartLeft(){
		//StopAll();
		rightExhaust.Stop ();
		middleExhaust.Stop ();
		sControl.playFullAnotherClip(oneJetSound);
		if(!leftExhaust.isPlaying){
			leftExhaust.Play();
		}
	}
	public void StartRight(){
		//StopAll();
		leftExhaust.Stop ();
		middleExhaust.Stop ();
		sControl.playFullAnotherClip(oneJetSound);
		if(!rightExhaust.isPlaying){
			rightExhaust.Play();
		}
	}
	public void StartMiddle(){
		//StopAll();
		leftExhaust.Stop ();
		rightExhaust.Stop ();
		sControl.playFullAnotherClip(oneJetSound);
		if(!middleExhaust.isPlaying){
			middleExhaust.Play();
		}
	}
	public void StartAll(){
		sControl.playFullAnotherClip(fullPowerSound);
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
		sControl. stopSound();
		leftExhaust.Stop();
		middleExhaust.Stop();
		rightExhaust.Stop();
	}
}