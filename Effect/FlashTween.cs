using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;



public class FlashTween : MonoBehaviour
{

	public float coolTime;
	
	float time;

	public float fromTweenValue;
	
	public float toTweenValue;
	
	float value;
	
	
	void Update(){
		if(time<coolTime){
			time += Time.deltaTime;
			value =Mathf.Lerp(fromTweenValue, toTweenValue,time/coolTime);
			
		}
		
	}
	
	public Restart(){
		
		value= fromTweenValue;
		time = 0;
	}


}