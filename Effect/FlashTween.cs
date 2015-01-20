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

    public Tonemapping mapping;

    void Awake()
    {
        mapping = GetComponent<Tonemapping>();
    }

	void Update(){
		if(time<coolTime){
			time += Time.deltaTime;
			value =Mathf.Lerp(fromTweenValue, toTweenValue,time/coolTime);
            mapping.middleGrey = value;
            if (time >= coolTime)
            {
                mapping.enabled = false;
           
            }
		}
		
	}
	
	public void Restart(){
		
		value= fromTweenValue;
		time = 0;
        if (mapping != null)
        {
            mapping.enabled = true;
        }
	}


}