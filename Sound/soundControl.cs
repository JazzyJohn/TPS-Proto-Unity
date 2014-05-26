using System;
using UnityEngine;

public class soundControl
{
	private AudioSource source;
//--------------------------------------------------------------------------------------------------------------
	//конструктор. Получает ссылку на источник
	public soundControl(AudioSource s){
		if (s != null) {
			source = s;
		} 
		else {
		//	Debug.LogError("soundControl:soundControl: Source not defined!!");
		}
	}
//--------------------------------------------------------------------------------------------------------------
	//проигрывает заданный клип
	public void playClip(AudioClip clip){
		if (source != null) {
			if (clip != null) {
				source.clip = clip;
				source.Play ();
			} 
			else {
				//Debug.LogError("soundControl:playClip: Clip not defined!!");
			}
		}
		else {
			//Debug.LogError("soundControl:playClip: Source not defined!!");
		}
	}
//--------------------------------------------------------------------------------------------------------------
	//проигрывает один клип из массива, выбирая случайным образом. Если клип уже звучит, то не прерывает
	public void playClipsRandomFull(Array array){
		if (array.Length == 0) {
			return;
		}
		AudioClip tmpClip =(AudioClip) array.GetValue(UnityEngine.Random.Range (0, array.GetLength (0)));
		if (tmpClip != null) {
			if(!source.isPlaying){//если клип уже играет, то пусть играет
				source.clip = tmpClip;
				source.Play ();
			}
		} 
		else {
			//Debug.LogError("soundControl:playClipsRandom: Clip not defined!!");
		}
	}
	//проигрывает один клип из массива, выбирая случайным образом. Если клип уже звучит, то  прерывает
	public void playClipsRandom(Array array){
		if (array.Length == 0) {
			return;
		}
		AudioClip tmpClip =(AudioClip) array.GetValue(UnityEngine.Random.Range (0, array.GetLength (0)));
		if (tmpClip != null) {

				source.clip = tmpClip;
				source.Play ();

		} 
		else {
			//Debug.LogError("soundControl:playClipsRandom: Clip not defined!!");
		}
	}
//--------------------------------------------------------------------------------------------------------------
	//останавливает воспроизведение(для зацикленых звуков)
	public void stopSound(){
		if(source!=null){
			source.Stop();
			source.clip= null;
		}
	}
//--------------------------------------------------------------------------------------------------------------
	//проигрывает заданный клип целиком, не прерывая
	public void playFullClip(AudioClip clip){
		if (source != null) {
			if(!source.isPlaying){
				if (clip != null) {
					source.clip = clip;
					source.Play ();
				} 
				else {
					//Debug.LogError("soundControl:playFullClip: Clip not defined!!");
				}
			}
		}
		else {
			//Debug.LogError("soundControl:playFullClip: Source not defined!!");
		}
	}
	//проигрывает заданный клип целиком, не прерывая, подменяет клиппы если они разные
	public void playFullAnotherClip(AudioClip clip){
		if (source != null) {
			if(!source.isPlaying){
				if (clip != null&&source.clip !=clip) {
					source.clip = clip;
					source.Play();
				} 
				else {
					//Debug.LogError("soundControl:playFullClip: Clip not defined!!");
				}
			}
		}
		else {
			//Debug.LogError("soundControl:playFullClip: Source not defined!!");
		}
	}
	public void setLooped(bool loop){
		source.loop= loop;

	}
}
//---------------------------------------------------------------------------------------------------------------

